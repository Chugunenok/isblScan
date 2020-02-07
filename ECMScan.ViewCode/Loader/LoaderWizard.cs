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
      SqlCommand command = new SqlCommand();
      command.Connection = Connection;
      command.CommandText = "select Analit, NameAn, Soder from MBAnalit where Vid = (select Vid from MBVidAn where Kod = 'WIZARD_GROUPS') order by NameAn asc";
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
      return listGroups;
    }

    public void Load(List<IsbNode> isblList)
    {
      System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
      IsbNode listNode = null;
      listNode = new IsbNode();
      listNode.Name = "Мастер действий";
      listNode.Text = null;

      var listGroups = LoadGroups(listNode);
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
    , HighLvl
from MBAnalit 
    join MBText on MBText.SrcRecID = MBAnalit.Analit and MBText.SrcObjID = 119
where Vid= (select Vid from MBVidAn where Kod = 'WIZARDS')
order by MBAnalit.NameAn asc";
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
                            MessageBox.Show("Ошибка парсинга схемы мастера " + wizardNode.Name + ": " + e.Message + ". " + e.StackTrace, "Ошибка разбора схемы мастера", buttons, icon);
                        }
          }
          if (!reader.IsDBNull(4))
          {
            wizardNode.LastUpdate = reader.GetDateTime(4);
          }
          var groupId = reader.GetInt32(5);
          listGroups.First(n => n.Id == groupId).Nodes.Add(wizardNode);
        }
      }
      reader.Close();
      isblList.Add(listNode);
    }
  }
}
