/*
 * Date: 10.08.2012
 * Time: 21:24
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ISBLScan.ViewCode
{
    /// <summary>
    /// Мастер действий.
    /// </summary>
    public class Wizard : LoaderCommon
    {
        public Wizard(SqlConnection sqlConnect) : base(sqlConnect)
        {
        }

        private List<IsbNode> LoadGroups(IsbNode rootNode)
        {
            var listGroups = new List<IsbNode>();
            Int32 vidAnalitId = GetVidAnalitId("WIZARD_GROUPS");
            if (vidAnalitId >= 0)
            {
                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandText = "select Analit, NameAn, Soder from MBAnalit where Vid=@vidAnalit";
                SqlParameter paramVidAnalit = new SqlParameter("@vidAnalit", SqlDbType.Int);
                paramVidAnalit.Value = vidAnalitId;
                command.Parameters.Add(paramVidAnalit);
                command.Prepare();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var node = new IsbNode();
                        node.Id = reader.GetInt32(0);
                        if (!reader.IsDBNull(1))
                        {
                            node.Name = reader.GetString(1);
                        }
                        if (!reader.IsDBNull(2))
                        {
                            node.Text = reader.GetString(2);
                        }
                        rootNode.Nodes.Add(node);
                        listGroups.Add(node);
                    }
                }
                reader.Close();
            }
            return listGroups;
        }

        override public IsbNode Load()
        {
            System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
            IsbNode listNode = null;
            Int32 vidAnalitId = GetVidAnalitId("WIZARDS");
            if (vidAnalitId >= 0)
            {
                listNode = new IsbNode();
                listNode.Name = "Мастер действий";
                listNode.Text = null;
                listNode.Id = vidAnalitId;

                var listGroups = LoadGroups(listNode);
                foreach (var groupNode in listGroups)
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = Connection;
                    command.CommandText = @"
select MBAnalit.Analit
    , MBAnalit.NameAn
    , MBAnalit.Soder 
    , MBText.SearchCondition 
    , (select max(prot.DateAct)
        from XProtokol prot 
        where prot.SrcObjID = 119 and prot.SrcRecID = MBAnalit.Analit) as LastUpd
from MBAnalit 
    join MBText on MBText.SrcRecID = MBAnalit.Analit and MBText.SrcObjID = 119
where Vid=@vidAnalit 
    and HighLvl=@groupID";
                    SqlParameter paramVidAnalit = new SqlParameter("@vidAnalit", SqlDbType.Int);
                    SqlParameter paramGroupId = new SqlParameter("@groupID", SqlDbType.Int);
                    paramVidAnalit.Value = vidAnalitId;
                    paramGroupId.Value = groupNode.Id;
                    command.Parameters.Add(paramVidAnalit);
                    command.Parameters.Add(paramGroupId);
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var wizardNode = new IsbNode();
                            wizardNode.Type = IsbNodeType.Wizard;

                            wizardNode.Id = reader.GetInt32(0);
                            if (!reader.IsDBNull(1))
                            {
                                wizardNode.Name = reader.GetString(1);
                            }
                            if (!reader.IsDBNull(2))
                            {
                                wizardNode.Text = reader.GetString(2);
                            }
                            if (!reader.IsDBNull(3))
                            {
                                SqlBytes sqlbytes = reader.GetSqlBytes(3);
                                try
                                {
                                    WizardParser.ParseWizardText(win1251.GetString(sqlbytes.Value), wizardNode);
                                }
                                catch (Exception e)
                                {
                                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                                    MessageBoxIcon icon = MessageBoxIcon.Information;
                                    MessageBox.Show("Ошибка парсинга схемы мастера " + wizardNode.Name, "Ошибка разбора схемы мастера", buttons, icon);
                                }
                            }
                            if (!reader.IsDBNull(4))
                            {
                                wizardNode.LastUpdate = reader.GetDateTime(4);
                            }
                            groupNode.Nodes.Add(wizardNode);
                        }
                    }
                    reader.Close();
                }
            }
            return listNode;
        }
    }
}
