/*
 * Date: 10.08.2012
 * Time: 21:22
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Функция.
	/// </summary>
	public class Function : LoaderCommon
	{
		public Function (SqlConnection sqlConnect) : base(sqlConnect)
		{
		}
		
		private List<IsbNode> LoadGroups (IsbNode rootNode, char charSysFunc)
		{
			var listGroups = new List<IsbNode> ();
			if (this.CheckTableExist ("MBRegUnit")) 
			{
				SqlCommand command = new SqlCommand ();
				command.Connection = this.Connection;
				//command.CommandText = "select t.id, t.name from (select MBGrFunc.NGroup [id], Max(MBGrFunc.GrName) [name] from MBGrFunc join MBFunc on (MBGrFunc.NGroup = MBFunc.NGroup) where MBFunc.SysFunc=@funcCategory and not(Txt is null) group by MBGrFunc.NGroup) t order by t.name";
				command.CommandText = @"
select t.id, t.name, t.Comment
from
(
    select MBGrFunc.NGroup [id], Max(MBGrFunc.GrName) [name], Max(MBGrFunc.LastUpd) [LastUpd], Max(CAST(MBGrFunc.Comment as NVARCHAR(MAX))) [Comment]
    from MBGrFunc 
    join MBFunc on (MBGrFunc.NGroup = MBFunc.NGroup) 
    where MBFunc.SysFunc=@funcCategory 
    group by MBGrFunc.NGroup
) t 
order by t.name";
				SqlParameter paramFuncCategory = new SqlParameter ("@funcCategory", SqlDbType.NChar, 1);
				paramFuncCategory.Value = charSysFunc;
				command.Parameters.Add (paramFuncCategory);
				command.Prepare ();
				SqlDataReader reader = command.ExecuteReader ();
				if (reader.HasRows) 
				{
					while (reader.Read()) 
					{
						var node = new IsbNode();
						//Номер группы функций
						node.Id = reader.GetInt32 (0);
						//Имя группы функций
						if (! reader.IsDBNull (1)) {
							node.Name = reader.GetString (1);
						}
						//Примечание
						if (!reader.IsDBNull (2)) {
							//node.Text = reader.GetString (2);
						}
						rootNode.Nodes.Add (node);
						listGroups.Add (node);
					}
				}
				reader.Close ();
			}
			return listGroups;
		}

		/// <summary>
		/// Загрузка параметров (реквизитов) функции
		/// </summary>
		/// <param name="rootNode">
		/// Узел дерева элементов, соотвествующий функции.
		/// A <see cref="Node"/>
		/// </param>
		private void LoadRecvisites (IsbNode rootNode)
		{
			if (this.CheckTableExist ("MBFuncRecv")) 
			{
				SqlCommand command = new SqlCommand ();
				command.Connection = this.Connection;
				command.CommandText = "SELECT [NumPar], [Ident], [Name], [Type], [ValueDef] FROM [MBFuncRecv] WHERE [FName] = @funcName ORDER BY [NumPar] ASC";
				SqlParameter paramFuncName = new SqlParameter ("@funcName", SqlDbType.NVarChar, 512);
				paramFuncName.Value = rootNode.Name;
				command.Parameters.Add (paramFuncName);
				command.Prepare ();
				SqlDataReader reader = command.ExecuteReader ();
				if (reader.HasRows) 
				{
					var funcRecvNode = new IsbNode();
					funcRecvNode.Name = "-=[ Параметры функции ]=-";
					rootNode.Nodes.Add (funcRecvNode);
					while (reader.Read ()) 
					{
						//Номер параметра функции
						funcRecvNode.Text += reader.GetInt32 (0).ToString () + ".\t";
						//Идентификатор параметра
						if (!reader.IsDBNull (1)) {
							funcRecvNode.Text += reader.GetString (1) + ".\t";
						}
						////Для внутреннего использования
						//if (!reader.IsDBNull (2)) {
						//	funcRecvNode.Text += reader.GetString (2) + ".\t";
						//}
						//Тип параметра
						if (!reader.IsDBNull (3)) {
							funcRecvNode.Text += CodeToNameConverter.FunctionParamTypeIDToName(reader.GetString (3)) + ".\t";
						}
						funcRecvNode.Text += "\r\n";
					}
					funcRecvNode.LastUpdate = rootNode.LastUpdate;
				}
				reader.Close ();
			}
		}

		public IsbNode Load ()
		{
		    IsbNode listNode = null;
			if (this.CheckTableExist ("MBFunc")) {
				listNode = new IsbNode();
				listNode.Name = "Функция";
				char[] charsSysUserFunc = {'P', 'S'};
				foreach (char charSysFunc in charsSysUserFunc) 
				{
					var systemFuncNode = new IsbNode();
					if (charSysFunc == 'P') {
						systemFuncNode.Name = "Пользовательская";
					} else {
						systemFuncNode.Name = "Системная";
					}
					listNode.Nodes.Add(systemFuncNode);
					
					var listGroups = LoadGroups(systemFuncNode, charSysFunc);
					foreach (var groupNode in listGroups) 
					{
						SqlCommand command = new SqlCommand ();
						command.Connection = Connection;
						//command.CommandText = "select XRecID, FName, Comment, Help, Txt from MBFunc where NGroup=@groupID and SysFunc=@sysFunc and not(Txt is null) order by FName";
						command.CommandText = "select XRecID, FName, Comment, Help, Txt, LastUpd from MBFunc where NGroup=@groupID and SysFunc=@sysFunc order by FName";
						SqlParameter paramGroupId = new SqlParameter ("@groupID", SqlDbType.Int);
						SqlParameter paramSysFunc = new SqlParameter ("@sysFunc", SqlDbType.NChar, 1);
						paramGroupId.Value = groupNode.Id;
						paramSysFunc.Value = charSysFunc;
						command.Parameters.Add (paramGroupId);
						command.Parameters.Add (paramSysFunc);
						command.Prepare ();
						SqlDataReader reader = command.ExecuteReader ();
						if (reader.HasRows) 
						{
							while (reader.Read()) 
							{
								var functionNode = new IsbNode();
                                functionNode.Type = IsbNodeType.Function;
                                //ИД
                                functionNode.Id = reader.GetInt32 (0);
								//Имя функции
								if (! reader.IsDBNull (1)) {
									functionNode.Name = reader.GetString (1);
								}
								//Комментарий к функции
								if (! reader.IsDBNull (2)) {
									var funcDescriptionNode = new IsbNode();
									funcDescriptionNode.Name = "-=[ Описание функции ]=-";
									funcDescriptionNode.Text = reader.GetString (2);
									functionNode.Nodes.Add(funcDescriptionNode);
								}
								//Справка по функции
								/*
								if(! reader.IsDBNull(3))
								{
									Node funcHelpNode = new Node();
									funcHelpNode.name = "Справка по функции";
									funcHelpNode.text = reader.GetString(3);;
									funcHelpNode.parent = functionNode;
									functionNode.nodes.Add(funcHelpNode);
								}
								*/
								//Текст функции
								if (! reader.IsDBNull (4)) {
									var funcTextNode = new IsbNode();
									funcTextNode.Name = "-=[ Текст функции ]=-";
									funcTextNode.Text = reader.GetString (4);
									functionNode.Nodes.Add(funcTextNode);
								}
								//Дата и время последнего изменения
								if (!reader.IsDBNull (5)) {
									functionNode.LastUpdate = reader.GetDateTime(5);
								}
								groupNode.Nodes.Add(functionNode);
							}
						}
						reader.Close ();
						//Загрузка параметров функций для функций текущей группы
						foreach (var funcNode in groupNode.Nodes) {
							this.LoadRecvisites (funcNode);
						}
					}				

				}
			}
			return listNode;
		}

	}
}