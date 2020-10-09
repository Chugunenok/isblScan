/*
 * Date: 10.08.2012
 * Time: 21:24
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Xml;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Блок типового маршрута.
	/// </summary>
	public class RouteBlock : LoaderCommon
	{
		public RouteBlock(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

		private List<IsbNode> LoadGroups(IsbNode rootNode)
		{
			var listGroups = new List<IsbNode>();
			if(this.CheckTableExist("SBRouteBlockGroup"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = Connection;
				command.CommandText = "select t.id, t.name from (select SBRouteBlockGroup.XrecID [id], Max(SBRouteBlockGroup.Name) [name] from SBRouteBlockGroup join SBRouteBlock on (SBRouteBlockGroup.XRecID = SBRouteBlock.BlockGroup)  group by SBRouteBlockGroup.XRecID) t order by t.name";
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
			if(this.CheckTableExist("SBRouteBlock"))
			{
				listNode = new IsbNode("Блок типового маршрута");
				
				var listGroups = LoadGroups(listNode);
				foreach(var groupNode in listGroups)
				{
					SqlCommand command = new SqlCommand();
					command.Connection = Connection;
					command.CommandText = "select XRecID, Name, Comment, Properties, LastUpdate from SBRouteBlock where BlockGroup=@groupID order by Name";
					SqlParameter paramGroupId = new SqlParameter("@groupID", SqlDbType.Int);
					paramGroupId.Value = groupNode.Id;
					command.Parameters.Add(paramGroupId);
					command.Prepare();
					SqlDataReader reader = command.ExecuteReader();
					if(reader.HasRows)
					{
						while(reader.Read())
						{
							var routeBlockNode = new IsbNode();
                            routeBlockNode.Type = IsbNodeType.RouteBlock;
                            //ИД
                            routeBlockNode.Id = reader.GetInt32(0);
							//Имя
							if(! reader.IsDBNull(1))
							{
								routeBlockNode.Name = reader.GetString(1);
							}
							//Комментарий
							if(! reader.IsDBNull(2))
							{
								routeBlockNode.Text = reader.GetString(2);
							}
							//Свойства
							if(! reader.IsDBNull(3))
							{
                                var schemaBytes = (byte[])reader.GetValue(3);
                                string schemaString = System.Text.Encoding.GetEncoding(1251).GetString(schemaBytes);
                                RouteParser.ParseBlockProperties(schemaString, routeBlockNode);
                            }
						    if (!reader.IsDBNull(4))
						    {
						        routeBlockNode.LastUpdate = reader.GetDateTime(4);
						    }
                            groupNode.Nodes.Add(routeBlockNode);
						}
					}
					reader.Close();
				}				
			}
			return listNode;
		}

        private string GetNodeString(XmlNode node)
        {
            string parsedString = "";
            parsedString = node.InnerText;
            parsedString = parsedString.Replace("{5314B05F-CF9F-4F66-99EC-24992A5FB114}", "");
            try
            {
                byte[] data = Convert.FromBase64String(parsedString);
                parsedString = System.Text.Encoding.GetEncoding(1251).GetString(data);
            }
            catch { }
            return parsedString;
        }
    }
}
