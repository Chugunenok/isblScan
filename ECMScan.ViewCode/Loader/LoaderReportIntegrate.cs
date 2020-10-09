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
	public class ReportIntegrate : LoaderCommon
	{
		public ReportIntegrate(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

		override public IsbNode Load()
		{
		    IsbNode listNode = null;
			if(this.CheckTableExist("MBReports"))
			{
				listNode = new IsbNode();
				listNode.Name = "Интегрированный отчёт";
				
				{
					SqlCommand command = new SqlCommand();
					command.Connection = Connection;
					command.CommandText = "select XRecID, NameRpt, Comment, Exprn, Report, Viewer, LastUpd from MBReports where TypeRpt='MBAnalitV' order by NameRpt ASC";
					SqlDataReader reader = command.ExecuteReader();
					if(reader.HasRows)
					{
						while(reader.Read())
						{
							var reportNode = new IsbNode();
                            reportNode.Type = IsbNodeType.IntegratedReport;
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
                            listNode.Nodes.Add(reportNode);
						}
					}
					reader.Close();
				}				
			}
			return listNode;
		}

	}
}
