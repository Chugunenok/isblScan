using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace ISBLScan.ViewCode
{
    /// <summary>
    /// Прикладные диалоги.
    /// </summary>
    public class Dialog : LoaderCommon
    {
        public Dialog(SqlConnection sqlConnect) : base(sqlConnect)
        {
        }

        /// <summary>
        /// Загрузка реквизитов диалога
        /// </summary>
        /// <param name="recvGroupNode">
        /// Ссылка на узел диалога
        /// A <see cref="IsbNode"/>
        /// </param>
        void LoadRecvisites(IsbNode refNode)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = this.Connection;
            command.CommandText = "SELECT [Name], [Code], [ChangeEventHandlerText], [LookupEventHandlerText], [Section] FROM [SBDialogRequisiteLink] WHERE [DialogID]=@DialogID AND ([ChangeEventHandlerText] IS NOT NULL OR [LookupEventHandlerText] IS NOT NULL) ORDER BY [RequisiteNumber]";
            SqlParameter paramVid = new SqlParameter("@DialogID", SqlDbType.Int);
            paramVid.Value = refNode.Id;
            command.Parameters.Add(paramVid);
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
                        refNode.Nodes.Add(recvGroupNode);
                        sectionCodeToSectionNode.Add(section, recvGroupNode);
                    }

                    var recvNode = new IsbNode();
                    if (!reader.IsDBNull(0))
                    {
                        recvNode.Name = reader.GetString(0);
                    }
                    if (!reader.IsDBNull(1))
                    {
                        recvNode.Code = reader.GetString(1);
                        recvNode.Name += " (" + recvNode.Code + ")";
                    }
                    if (!reader.IsDBNull(2))
                    {
                        var exprnRefRecvNode = new IsbNode();
                        exprnRefRecvNode.Name = "-=[ Вычисление ]=-";
                        exprnRefRecvNode.Text = reader.GetString(2);
                        recvNode.Nodes.Add(exprnRefRecvNode);
                    }
                    if (!reader.IsDBNull(3))
                    {
                        var inpExprnRefRecvNode = new IsbNode();
                        inpExprnRefRecvNode.Name = "-=[ Выбор из справочника ]=-";
                        inpExprnRefRecvNode.Text = reader.GetString(3);
                        recvNode.Nodes.Add(inpExprnRefRecvNode);
                    }
                    recvGroupNode.Nodes.Add(recvNode);
                }
            }
            reader.Close();
        }
        void LoadMethods(IsbNode refNode)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = this.Connection;
            command.CommandText = "SELECT [Name], [Exprn] FROM [SBDialogMethod] WHERE [DialogID]=@DialogID AND [Exprn] IS NOT NULL ORDER BY [Name]";
            SqlParameter paramVid = new SqlParameter("@DialogID", SqlDbType.Int);
            paramVid.Value = refNode.Id;
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
                refNode.Nodes.Add(methodsNode);
            }
            reader.Close();
        }

        public IsbNode Load()
        {
            var methodsSupports = CheckTableExist("SBDialogMethod");
            var rootRefNode = new IsbNode("Диалог");
            if (this.CheckTableExist("SBDialog") && this.CheckTableExist("SBDialogRequisiteLink"))
            {
                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandText = "select XRecID, Name, Code, EventHandlersText, LastUpd, Comment from SBDialog order by Name ASC";
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var refNode = new IsbNode();
                        refNode.Type = IsbNodeType.Dialog;
                        refNode.Id = reader.GetInt32(0);
                        refNode.Code = reader.GetString(2).Trim();
                        refNode.Name = reader.GetString(1).Trim() + " (" + refNode.Code + ")";
                        if (!reader.IsDBNull(3))
                        {
                            ReferenceEventsParser.ParseEvents(reader.GetString(3).Trim(), refNode);
                        }
                        if (!reader.IsDBNull(4))
                        {
                            refNode.LastUpdate = reader.GetDateTime(4);
                        }
                        if (!reader.IsDBNull(5))
                        {
                            refNode.Text = reader.GetString(5).Trim();
                        }


                        rootRefNode.Nodes.Add(refNode);
                    }
                }
                reader.Close();
                foreach (var node in rootRefNode.Nodes)
                {
                    LoadRecvisites(node);
                    if (methodsSupports) LoadMethods(node);
                }
            }
            return rootRefNode;
        }
    }
}
