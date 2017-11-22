using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace ISBLScan.ViewCode
{
    /// <summary>
    /// Прикладная разработка
    /// </summary>
    public class IsbDev
    {
        /// <summary>
        /// Дерево элементов прикладной разработки
        /// </summary>
        public List<IsbNode> Nodes = new List<IsbNode>();

        public ConnectionParams ConnectionParams { get; set; } = new ConnectionParams();

        public void FillParent()
        {
            foreach(var node in this.Nodes)
            {
                node.FillParent();
            }
        }

        public void Search(string[] searchStrArray, bool caseSensitive, bool regExp, bool findAll)
        {
            foreach (var node in Nodes)
            {
                FilterNode(node, searchStrArray, caseSensitive, regExp, findAll);
            }
        }

        /// <summary>
        /// Рекурсивный поиск по дереву разработки
        /// </summary>
        /// <param name="node"></param>
        /// <param name="searchStrArray"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="regExp"></param>
        /// <param name="findAll"></param>
        /// <returns></returns>
        bool FilterNode(IsbNode node, string[] searchStrArray, bool caseSensitive, bool regExp, bool findAll)
        {
            bool isFound = false;
            //Сначала выделим текущий элемент так, как будто в нём ничего не найдено
            node.IsMatch = false;
            node.IsContainsMatchedNode = false;
            if (node.Nodes != null)
            {
                foreach (var subNode in node.Nodes)
                {
                    if (FilterNode(subNode, searchStrArray, caseSensitive, regExp, findAll))
                    {
                        node.IsContainsMatchedNode = true;
                        isFound = true;
                    }
                }
            }
            string isblText = node.Text;
            string nodeName = node.Name;
            bool searchSatisfied = false;
            if (regExp)
            {
                RegexOptions regExpOptions = caseSensitive ? RegexOptions.Compiled : RegexOptions.Compiled | RegexOptions.IgnoreCase;
                foreach (string searchPhrase in searchStrArray)
                {
                    Regex regEx = new Regex(searchPhrase, regExpOptions);
                    if ((!string.IsNullOrEmpty(isblText) && regEx.IsMatch(isblText)) || regEx.IsMatch(nodeName))
                    {
                        searchSatisfied = true;
                        if (!findAll) break;
                    }
                    else
                    {
                        if (findAll)
                        {
                            searchSatisfied = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                var comparation = caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

                foreach (string searchPhrase in searchStrArray)
                {
                    if (
                        (!string.IsNullOrEmpty(isblText) && isblText.IndexOf(searchPhrase, 0, comparation) >= 0) ||
                        (!string.IsNullOrEmpty(nodeName) && nodeName.IndexOf(searchPhrase, 0, comparation) >= 0)
                    )
                    {
                        searchSatisfied = true;
                        if (!findAll) break;
                    }
                    else
                    {
                        if (findAll)
                        {
                            searchSatisfied = false;
                            break;
                        }
                    }
                }
            }
            if (searchSatisfied)
            {
                node.IsMatch = true;
                isFound = true;
            }
            return isFound;
        }
    }

    /// <summary>
    /// Узел дерева элементов разработки
    /// </summary>
    public class IsbNode
    {
        /// <summary>
        /// Gets or sets ИД записи
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Код записи, группы, вида, ...
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets Имя узла, отображаемое в дереве
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Текст отображаемый при выборе узла
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets Список подузлов
        /// </summary>
        public List<IsbNode> Nodes { get; set; } = new List<IsbNode>();

        /// <summary>
        /// Gets or sets Признак того, что узел соотвествует поисковому запросу
        /// </summary>
        public bool IsMatch { get; set; }

        /// <summary>
        /// Gets or sets Признак того, что узел содержит подузлы, которые соответствуют поисковому запросу
        /// </summary>
        public bool IsContainsMatchedNode { get; set; }

        /// <summary>
        /// Gets or sets Дата последней модификации узла разработки (ISBL)
        /// </summary>
        public DateTime? LastUpdate { get; set; }

        /// <summary>
        /// Gets or sets Родительский элемент
        /// </summary>
        public IsbNode Parent { get; set; }

        /// <summary>
        /// Gets or sets Тип элемента
        /// </summary>
        public IsbNodeType? Type { get; set; }

        public IsbNode(string name)
        {
            Name = name;
        }
        public IsbNode()
        {
        }

        public IsbNode GetMainNode()
        {
            if(Id != 0 && Type != null)
            {
                return this;
            }
            else
            {
                if(Parent != null)
                {
                    return Parent.GetMainNode();
                }
                else
                {
                    return null;
                }
            } 
        }

        public void OpenInSbrte(ConnectionParams connectionParams)
        {
            var typeToCMDParams = new Dictionary<IsbNodeType?, string>()
            {
                { IsbNodeType.ReferenceType, "-F=\"REFERENCE_TYPES\"" },
                { IsbNodeType.Function, "-F=\"FUNCTIONS\"" },
                { IsbNodeType.Report, "-F=\"REPORTS\" -V=\"Design\"" },
                { IsbNodeType.Script, "-F=\"SCRIPTS\"" },
                { IsbNodeType.StandardRoute, "-F=\"ТМТ\"" },
                { IsbNodeType.Wizard, "-F=\"WIZARDS\"" },
                { IsbNodeType.RouteBlock, "-F=\"ROUTE_BLOCK\"" },
                { IsbNodeType.EDocType, "-F=\"EDOCUMENT_TYPES\"" },
                { IsbNodeType.IntegratedReport, "-F=\"REFERENCE_TYPES\"" },
                { IsbNodeType.Dialog, "-F=\"DIALOGS\"" }
            };
            var mainNode = GetMainNode();
            if(mainNode != null)
            {
                var CMDParams = typeToCMDParams[mainNode.Type];
                var loader = new Loader();
                var version = loader.GetVersion(connectionParams);
                var config = LoadVersionToSbrtePathConfig();
                string FilePath = config.VersionToSbrtePath.Where(c => c.Version == version).FirstOrDefault()?.Path ?? "";
                if(String.IsNullOrWhiteSpace(FilePath))
                {
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBoxIcon icon = MessageBoxIcon.Information;
                    MessageBox.Show("Не удалось получить путь к Sbrte для версии " + version, "Ошибка открытия компоненты " + this.Name, buttons, icon);
                }
                else
                {
                    Process process = new Process();
                    process.StartInfo.FileName = FilePath;
                    var authParam = String.IsNullOrWhiteSpace(connectionParams.Password) ? "-IsOSAuth=true" : $"-W=\"{connectionParams.Password}\"";
                    process.StartInfo.Arguments = $"-S=\"{connectionParams.Server}\" -D=\"{connectionParams.Database}\" -N=\"{connectionParams.Login}\" {authParam} -CT=\"Reference\" {CMDParams} -RID={mainNode.Id}";
                    try
                    {
                        process.Start();
                    }
                    catch (Exception e)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        MessageBoxIcon icon = MessageBoxIcon.Information;
                        MessageBox.Show(e.Message, "Ошибка открытия компоненты " + this.Name, buttons, icon);
                    }
                }
            }
        }

        public void FillParent()
        {
            foreach(var node in this.Nodes)
            {
                node.Parent = this;
                node.FillParent();
            }
        }

        private class VersionToSbrtePath
        {
            public string Version;
            public string Path;
        }
        private class VersionToSbrtePathConfig
        {
            public List<VersionToSbrtePath> VersionToSbrtePath { get; set; }
        }

        private VersionToSbrtePathConfig LoadVersionToSbrtePathConfig()
        {
            var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VersionNumToSbrtePathConfig.json");
            var jsonSettings = File.ReadAllText(configFilePath);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<VersionToSbrtePathConfig>(jsonSettings);
        }
    }

    public enum IsbNodeType
    {
        ReferenceType = 0,
        Function = 1,
        EDocType = 2,
        Report = 3,
        IntegratedReport = 4,
        StandardRoute = 5,
        RouteBlock = 6,
        Script = 7,
        Wizard = 8,
        Dialog = 9
    }
    public class ConnectionParams
    {
        public string Server;
        public string Database;
        public string Login;
        public string Password;
    }
}
