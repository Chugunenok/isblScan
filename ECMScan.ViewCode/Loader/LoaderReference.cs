/*
 * Date: 10.08.2012
 * Time: 21:23
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

namespace ISBLScan.ViewCode
{
  /// <summary>
  /// Справочник (аналитика).
  /// </summary>
  public class Reference : LoaderCommon
  {
    public Reference(SqlConnection sqlConnect) : base(sqlConnect)
    {
    }

    void LoadRecvisites(List<IsbNode> refNodes)
    {
      SqlCommand command = new SqlCommand();
      command.Connection = this.Connection;
      command.CommandText = "SELECT r.Name, r.Kod, r.Exprn, r.InpExprn, r.Razd, r.Vid FROM MBVidAnRecv r join MBVidAn s on r.Vid = s.Vid and s.Sost = 'A'  WHERE (r.Exprn IS NOT NULL OR r.InpExprn IS NOT NULL) ORDER BY r.NumRecv";
      command.Prepare();
      SqlDataReader reader = command.ExecuteReader();
      if (reader.HasRows)
      {
        while (reader.Read())
        {
          var vid = reader.GetInt32(5);
          var refNode = refNodes.First(n => n.Id == vid);
          var section = reader.GetString(4)[0];
          IsbNode recvGroupNode = null;
          recvGroupNode = refNode.Nodes.FirstOrDefault(n => n.Code == section.ToString());
          if (recvGroupNode == null) { 
            var sectionName = ReferenceEventsParser.SectionCodeToName.ContainsKey(section) ? ReferenceEventsParser.SectionCodeToName[section] : "Неизвестно [" + section + "]";
            recvGroupNode = new IsbNode(sectionName);
            recvGroupNode.Code = section.ToString();
            refNode.Nodes.Add(recvGroupNode);
          }

          var recvNode = new IsbNode();
          if (!reader.IsDBNull(0))
          {
            recvNode.Name = reader.GetString(0).Trim();
          }
          if (!reader.IsDBNull(1))
          {
            recvNode.Code = reader.GetString(1).Trim();
            recvNode.Name += " (" + recvNode.Code.Trim() + ")";
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
      command.CommandText = "SELECT [Name], [Exprn] FROM [MBVidAnMethod] WHERE [Vid]=@Vid AND [Exprn] IS NOT NULL ORDER BY [Name]";
      SqlParameter paramVid = new SqlParameter("@Vid", SqlDbType.Int);
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

    public void Load(List<IsbNode> isblList)
    {
      var rootRefNode = new IsbNode("Тип справочника");
      SqlCommand command = new SqlCommand();
      command.Connection = Connection;
      command.CommandText = "select Vid, Name, Kod, Exprn, LastUpd, Comment from MBVidAn where Sost = 'A' order by Name ASC";
      SqlDataReader reader = command.ExecuteReader();
      if (reader.HasRows)
      {
        while (reader.Read())
        {
          var refNode = new IsbNode();
          refNode.Type = IsbNodeType.ReferenceType;
          //ИД 
          refNode.Id = reader.GetInt32(0);
          //Имя (Код)
          if ((!reader.IsDBNull(1)) && (!reader.IsDBNull(2)))
          {
            refNode.Name = reader.GetString(1).Trim() + " (" + reader.GetString(2).Trim() + ")";
          }
          if (!reader.IsDBNull(2))
          {
            refNode.Code = reader.GetString(2).Trim();
          }
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

      LoadRecvisites(rootRefNode.Nodes);
      //foreach (var node in rootRefNode.Nodes)
      //{
      //  if (CheckTableExist("MBVidAnMethod")) LoadMethods(node);
      //}
      isblList.Add(rootRefNode);
    }
  }
}
