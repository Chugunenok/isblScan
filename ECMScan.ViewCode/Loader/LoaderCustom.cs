/*
 * Date: 10.08.2012
 * Time: 21:24
 */
using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Вычисления из справочников.
	/// </summary>
	public class CustomCalculations : LoaderCommon
	{
		public CustomCalculations(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

        private class Calculation
        {
            public string ReferenceName { get; set; }
            public string RequisiteName { get; set; }
            public string CalculationName { get; set; }
            public string TableName { get; set; }
            public bool IsTextRequisite { get; set; }
            public string FieldForUseAsCode { get; set; }
        }

        private class Config 
        {
            public List<Calculation> Calculations{ get; set; }
        }

        private Config LoadConfig()
        {
            var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CustomCalculationConfig.json");
            var jsonSettings = File.ReadAllText(configFilePath);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(jsonSettings);
        }

        public IsbNode Load()
		{
		    IsbNode listNode = null;
            listNode = new IsbNode("Custom Calculations");

            var config = LoadConfig();

            foreach (Calculation setting in config.Calculations)
            {
                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                var codeSelect = String.IsNullOrWhiteSpace(setting.FieldForUseAsCode) ? "''" : "tbl." + setting.FieldForUseAsCode;
                var addToNameSelect = String.IsNullOrWhiteSpace(setting.FieldForUseAsCode) && setting.FieldForUseAsCode != "NameAn" ? "" : " + ' (' + tbl." + setting.FieldForUseAsCode + "+ ')'";
                if (setting.IsTextRequisite)
                {
                    command.CommandText = @"
select ref.NameAn " + addToNameSelect + @"
    , " + codeSelect + @"
    , MBText." + setting.RequisiteName + @"
    , (select max(prot.DateAct)
        from XProtokol prot 
        where prot.SrcObjID = 119 and prot.SrcRecID = ref.Analit) as LastUpd
from MBAnalitSpr ref
    join " + setting.TableName + @" tbl on tbl.Analit = ref.Analit
    join MBText on tbl.XRecID = MBText.SrcRecID and MBText.SrcObjID = (select XRecID from XObj where TblName like '" + setting.TableName + @"')
where ref.Vid = (select Vid from MBVidAn where Kod = '" + setting.ReferenceName + "')";
                }
                else
                {
                    command.CommandText = @"
select ref.NameAn " + addToNameSelect + @"
    , " + codeSelect + @"
    , tbl." + setting.RequisiteName + @"
    , (select max(prot.DateAct)
        from XProtokol prot 
        where prot.SrcObjID = 119 and prot.SrcRecID = ref.Analit) as LastUpd
from MBAnalitSpr ref
    join " + setting.TableName + @" tbl on tbl.Analit = ref.Analit
where ref.Vid = (select Vid from MBVidAn where Kod = '" + setting.ReferenceName + "')";
                }
                
                command.Prepare();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    var refNode = new IsbNode(setting.CalculationName);
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(2))
                        {
                            string calculation = "";
                            if (setting.IsTextRequisite)
                            {
                                var bytes = (byte[])reader.GetValue(2);
                                calculation = System.Text.Encoding.UTF8.GetString(bytes);
                            }
                            else
                            {
                                calculation = reader.GetString(2);
                            }

                            var recordNode = new IsbNode(reader.GetString(0));
                            recordNode.Code = reader.GetString(1);
                            recordNode.Text = calculation;
                            if (!reader.IsDBNull(3))
                            {
                                refNode.LastUpdate = reader.GetDateTime(3);
                            }
                            refNode.Nodes.Add(recordNode);
                        }
                        
                    }
                    listNode.Nodes.Add(refNode);
                }
                reader.Close();
            }
            return listNode;
        }
	}
}
