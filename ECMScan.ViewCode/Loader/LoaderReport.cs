/*
 * Date: 10.08.2012
 * Time: 21:23
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;
namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Отчёт (аналитический отчёт).
	/// </summary>
	public class Report : LoaderCommon
	{
		public Report(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

		private List<IsbNode> LoadGroups(IsbNode rootNode)
		{
			var listGroups = new List<IsbNode>();
			if(this.CheckTableExist("MBRegUnit"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = Connection;
				command.CommandText = "select t.id, t.name from (select MBRegUnit.RegUnit [id], Max(MBRegUnit.Name) [name] from MBRegUnit join MBReports on (MBRegUnit.RegUnit = MBReports.RegUnit) where MBReports.TypeRpt='MBAnAccRpt' group by MBRegUnit.RegUnit) t order by t.name";
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					while(reader.Read())
					{
						var node = new IsbNode();
						node.Id = reader.GetInt32(0);
						if(! reader.IsDBNull(1))
						{
							node.Name = reader.GetString(1);
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
		    IsbNode listNode = null;
			if(this.CheckTableExist("MBReports"))
			{
				listNode = new IsbNode();
				listNode.Name = "Аналитический отчёт";
				
				var listGroups = LoadGroups(listNode);
				foreach(var groupNode in listGroups)
				{
					SqlCommand command = new SqlCommand();
					command.Connection = Connection;
					command.CommandText = "select XRecID, NameRpt, Description, Exprn, Report, Viewer, LastUpd from MBReports where TypeRpt='MBAnAccRpt' and RegUnit=@groupID order by NameRpt ASC";
					SqlParameter paramGroupId = new SqlParameter("@groupID", SqlDbType.Int);
					paramGroupId.Value = groupNode.Id;
					command.Parameters.Add(paramGroupId);
					command.Prepare();
					SqlDataReader reader = command.ExecuteReader();
					if(reader.HasRows)
					{
						while(reader.Read())
						{
							var reportNode = new IsbNode();
                            reportNode.Type = IsbNodeType.Report;
                            //ИД отчёта
                            reportNode.Id = reader.GetInt32(0);
							//Имя отчёта
							if(! reader.IsDBNull(1))
							{
								reportNode.Name = reader.GetString(1);
							}
							//Описание отчёта
							if(! reader.IsDBNull(2))
							{
								reportNode.Text = reader.GetString(2);
							}
							//Шаблон отчёта
							if(! reader.IsDBNull(4))
							{
								SqlBytes sqlbytes = reader.GetSqlBytes(4);
								System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
								string scriptText = win1251.GetString(sqlbytes.Value);
								var reportTextNode = new IsbNode();
								reportTextNode.Name = "-=[ Шаблон ]=-";
								reportTextNode.Text = scriptText;
								reportNode.Nodes.Add(reportTextNode);
							}
							//Расчёт отчёта
							if(! reader.IsDBNull(3))
							{
								string templateText = reader.GetString(3);
								var reportTemplateNode = new IsbNode();
								reportTemplateNode.Name = "-=[ Расчёт ]=-";
								reportTemplateNode.Text = templateText;
								reportNode.Nodes.Add(reportTemplateNode);
							}
						    if (!reader.IsDBNull(6))
						    {
						        reportNode.LastUpdate = reader.GetDateTime(6);
						    }
                            groupNode.Nodes.Add(reportNode);
						}
					}
					reader.Close();
				}				
			}
			return listNode;
		}

	}
}
