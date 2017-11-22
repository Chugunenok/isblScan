/*
 * Date: 06.10.2012
 * Time: 10:54
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
    /// Загрузчик прикладной разработки для типов карточек электронных документов (события и вычисления карточки и реквизитов документов)
    /// </summary>
    public class EDocType : LoaderCommon
    {
        public EDocType(SqlConnection sqlConnect) : base(sqlConnect)
        {
        }

        private void LoadRecvisites(IsbNode eDocTypeNode)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = Connection;
            command.CommandText = "select [Name], [Kod], [Exprn], [InpExprn], [Razd] from MBEDocTypeRecv where [TypeID] = @eDocTypeID and (not([Exprn] is null) or not([InpExprn] is null)) order by [Name]";
            SqlParameter paramEDocTypeId = new SqlParameter("@eDocTypeID", SqlDbType.Int);
            paramEDocTypeId.Value = eDocTypeNode.Id;
            command.Parameters.Add(paramEDocTypeId);
            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                var sectionCodeToSectionNode = new Dictionary<Char, IsbNode>();
                while (reader.Read())
                {
                    var section = reader.GetString(4)[0];
                    IsbNode recvGroupNode = null;
                    if (sectionCodeToSectionNode.ContainsKey(section))
                    {
                        recvGroupNode = sectionCodeToSectionNode[section];
                    }
                    else
                    {
                        var sectionName = ReferenceEventsParser.SectionCodeToName.ContainsKey(section) ? ReferenceEventsParser.SectionCodeToName[section] : "Неизвестно [" + section + "]";
                        recvGroupNode = new IsbNode(sectionName);
                        eDocTypeNode.Nodes.Add(recvGroupNode);
                        sectionCodeToSectionNode.Add(section, recvGroupNode);
                    }

                    var eDocRecvNode = new IsbNode();
                    //Имя
                    if (!reader.IsDBNull(0))
                    {
                        eDocRecvNode.Name = reader.GetString(0);
                    }
                    //Код реквизита
                    if (!reader.IsDBNull(1))
                    {
                        eDocRecvNode.Code = reader.GetString(1);
                        eDocRecvNode.Name = string.Format("{0} ({1})", eDocRecvNode.Name, eDocRecvNode.Code);
                    }
                    //Вычисление для реквизита и действия
                    if (!reader.IsDBNull(2))
                    {
                        var exprnEDocRecvNode = new IsbNode();
                        exprnEDocRecvNode.Name = "-=[ Вычисление ]=-";
                        exprnEDocRecvNode.Text = reader.GetString(2);
                        eDocRecvNode.Nodes.Add(exprnEDocRecvNode);
                    }
                    //Выбор из справочника для реквизита типа "справочник" или "строка"
                    if (!reader.IsDBNull(3))
                    {
                        var eventEDocRecvNode = new IsbNode();
                        eventEDocRecvNode.Name = "-=[ Выбор из справочника ]=-";
                        eventEDocRecvNode.Text = reader.GetString(3);
                        eDocRecvNode.Nodes.Add(eventEDocRecvNode);
                    }
                    recvGroupNode.Nodes.Add(eDocRecvNode);
                }
            }
            reader.Close();
        }

        void LoadMethods(IsbNode eDocTypeNode)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = this.Connection;
            command.CommandText = "SELECT [Name], [Exprn] FROM [MBEDocTypeMethod] WHERE [TypeID]=@TypeID AND [Exprn] IS NOT NULL ORDER BY [Name]";
            SqlParameter paramVid = new SqlParameter("@TypeID", SqlDbType.Int);
            paramVid.Value = eDocTypeNode.Id;
            command.Parameters.Add(paramVid);
            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                var methodsNode = new IsbNode("Методы");
                while (reader.Read())
                {
                    var methodNode = new IsbNode();
                    if (!reader.IsDBNull(0))
                    {
                        methodNode.Name = reader.GetString(0).Trim();
                        methodNode.Code = methodNode.Name;
                    }
                    if (!reader.IsDBNull(1))
                    {
                        methodNode.Text = reader.GetString(1);
                    }
                    methodsNode.Nodes.Add(methodNode);
                }
                eDocTypeNode.Nodes.Add(methodsNode);
            }
            reader.Close();
        }

        public IsbNode Load()
        {
            var methodsSupports = CheckTableExist("MBEDocTypeMethod");
            IsbNode listNode = null;
            if (this.CheckTableExist("MBEDocType") && CheckTableExist("MBEDocTypeRecv"))
            {
                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandText = "select TypeID, Kod, Name, Exprn, LastUpd, Comment from MBEDocType order by Name ASC";
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    listNode = new IsbNode();
                    listNode.Name = "Тип карточки электронного документа";

                    while (reader.Read())
                    {
                        var eDocNode = new IsbNode();
                        eDocNode.Type = IsbNodeType.EDocType;
                        //ИД 
                        eDocNode.Id = reader.GetInt32(0);
                        eDocNode.Code = reader.GetString(1).Trim();
                        //Имя 
                        eDocNode.Name = reader.GetString(2).Trim() + " (" + eDocNode.Code + ")";
                        //Текст событий
                        if (!reader.IsDBNull(3))
                        {
                            ReferenceEventsParser.ParseEvents(reader.GetString(3).Trim(), eDocNode);
                        }
                        //Дата последнего изменения
                        if (!reader.IsDBNull(4))
                        {
                            eDocNode.LastUpdate = reader.GetDateTime(4);
                        }
                        //Комментарий
                        if (!reader.IsDBNull(5))
                        {
                            eDocNode.Text = reader.GetString(5);
                        }

                        listNode.Nodes.Add(eDocNode);
                    }
                }
                reader.Close();
                foreach (var eDocNode in listNode.Nodes.Where(n => n.Id != 0))
                {
                    LoadRecvisites(eDocNode);
                    if (methodsSupports) LoadMethods(eDocNode);
                }
            }
            return listNode;
        }

    }
}
