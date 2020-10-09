/*
 * Date: 10.08.2012
 * Time: 21:22
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace ISBLScan.ViewCode
{
    /// <summary>
    /// Функция.
    /// </summary>
    public class Function : LoaderCommon
    {
        public Function(SqlConnection sqlConnect) : base(sqlConnect)
        {
        }

        private void LoadGroups(IsbNode rootNode)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = this.Connection;
            command.CommandText = @"
select NGroup [id], GrName [name]
from MBGrFunc
order by GrName";
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var node = new IsbNode();
                    //Номер группы функций
                    node.Id = reader.GetInt32(0);
                    //Имя группы функций
                    node.Name = reader.GetString(1);
                    rootNode.Nodes.Add(node);
                }
            }
            reader.Close();
        }

        override public IsbNode Load()
        {
            var functionsNode = new IsbNode("Функция");
            LoadGroups(functionsNode);
            SqlCommand command = new SqlCommand();
            command.Connection = Connection;
            command.CommandText = "select XRecID, FName, Txt, LastUpd, NGroup from MBFunc where Txt is not null order by FName";
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var functionNode = new IsbNode();
                    functionNode.Type = IsbNodeType.Function;
                    //ИД
                    functionNode.Id = reader.GetInt32(0);
                    //Имя функции
                    functionNode.Name = reader.GetString(1);
                    //Текст функции
                    functionNode.Text = reader.GetString(2);
                    //Дата и время последнего изменения
                    if (!reader.IsDBNull(3))
                    {
                        functionNode.LastUpdate = reader.GetDateTime(3);
                    }
                    functionsNode.Nodes.First(g => g.Id == reader.GetInt32(4)).Nodes.Add(functionNode);
                }
            }
            reader.Close();
            functionsNode.Nodes = functionsNode.Nodes.Where(n => n.Nodes.Count > 0).ToList();
            return functionsNode;
        }

    }
}