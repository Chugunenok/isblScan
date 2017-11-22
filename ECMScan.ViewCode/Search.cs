using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;

namespace ISBLScan.ViewCode
{
    public class SearchNode
    {
        /// <summary>
        /// Gets or sets Имя узла, отображаемое в дереве
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Элемент разработки
        /// </summary>
        public IsbNode IsbNode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SearchNode"/> is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets Признак того, что узел соотвествует поисковому запросу
        /// </summary>
        public bool IsMatch { get; set; }

        /// <summary>
        /// Gets or sets Дата последней модификации узла разработки
        /// </summary>
        public DateTime? LastUpdate { get; set; }

        /// <summary>
        /// Gets or sets Список подузлов
        /// </summary>
        public List<SearchNode> Nodes { get; set; } = new List<SearchNode>();

        public SearchNode(IsbNode isbNode)
        {
            Name = isbNode.Name;
            IsbNode = isbNode;
            Visible = isbNode.IsMatch || isbNode.IsContainsMatchedNode;
            IsMatch = isbNode.IsMatch;
            LastUpdate = isbNode.LastUpdate;
        }
    }

    public class Search : IDisposable
    {
        public IsbDev IsbDev { get; set; }
        public List<SearchNode> Nodes { get; set; } = new List<SearchNode>();
        public ICSharpCode.AvalonEdit.TextEditor SearchCriteriaTextEditor { get; set; }
        public ICSharpCode.AvalonEdit.TextEditor TextEditor { get; set; }
        private bool regExp { get; set; }
        private bool caseSensitive { get; set; }
        public MainForm.SearchControls SearchControls { get; set; }

        public bool RegExp
        {
            get { return regExp; }
            set
            {
                regExp = value;
                TextEditor.TextArea.TextView.Redraw();
            }
        }

        public bool CaseSensitive
        {
            get { return regExp; }
            set
            {
                caseSensitive = value;
                TextEditor.TextArea.TextView.Redraw();
            }
        }

        public bool FindAll { get; set; }

        public DateTime FilterStartDate { get; set; } = System.Windows.Forms.DateTimePicker.MinimumDateTime;
        public DateTime FilterEndDate { get; set; } = System.Windows.Forms.DateTimePicker.MaximumDateTime;
        public string FilterName { get; set; } = "";

        string[] _searchStrs = new string[] { };

        public Search(IsbDev sourceDev)
        {
            IsbDev = sourceDev;
            BuildSearchNodes(true);

            SearchCriteriaTextEditor = new ICSharpCode.AvalonEdit.TextEditor
            {
                Options =
                {
                    ShowEndOfLine = true,
                    ShowSpaces = true,
                    ShowTabs = true
                }
            };

            SearchCriteriaTextEditor.TextChanged += SearchCriteriaChanged;
            SearchCriteriaTextEditor.TextArea.KeyUp += new System.Windows.Input.KeyEventHandler(TextArea_KeyUp);
            SearchCriteriaTextEditor.TextArea.TextView.LineTransformers.Add(new HighlightIncorrectRegExp(this));
            SearchCriteriaTextEditor.FontFamily = new FontFamily("Courier New, Courier, monospace");
            SearchCriteriaTextEditor.FontSize = 13;
            SearchCriteriaTextEditor.FontStretch = FontStretch.FromOpenTypeStretch(5);

            TextEditor = new ICSharpCode.AvalonEdit.TextEditor
            {
                Options =
                {
                    ConvertTabsToSpaces = true,
                    ShowEndOfLine = true,
                    ShowSpaces = true,
                    ShowTabs = true,
                    AllowScrollBelowDocument = false,
                    EnableRectangularSelection = true,
                },
                ShowLineNumbers = true,
                Text = "",
                IsReadOnly = true
            };
            TextEditor.TextArea.TextView.LineTransformers.Add(new HighlightSearchedStrings(this));
            TextEditor.FontFamily = new FontFamily("Courier New, Courier, monospace");
            TextEditor.FontSize = 13;
            TextEditor.FontStretch = FontStretch.FromOpenTypeStretch(1);
            TextEditor.TextArea.KeyUp += new System.Windows.Input.KeyEventHandler(TextArea_KeyUp);

            var syntaxHighlightingFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ISBL.xshd");
            using (Stream s = File.OpenRead(syntaxHighlightingFilePath))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    TextEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        public void TextArea_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0)
            {
                SearchControls.Process();
            }
            if (e.Key == Key.T && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0)
            {
                new MainForm.SearchControls(SearchControls._form);
            }
            if (e.Key == Key.F2)
            {
                SearchControls.TreeSelectNextMatched(SearchControls.TreeViewResults.Nodes);
            }
            if (e.Key == Key.F3)
            {
                SearchControls.TextEditorShowNextMatchedString();
            }
            if (e.Key == Key.O && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0)
            {
                var selectedNode = SearchControls.TreeViewResults.SelectedNode;
                if (selectedNode != null)
                {
                    ((SearchNode)selectedNode.Tag).IsbNode.OpenInSbrte(IsbDev.ConnectionParams);
                }
            }
        }
        public void Process()
        {
            if (_searchStrs.Length > 0)
            {
                IsbDev.Search(_searchStrs, caseSensitive, regExp, FindAll);
                BuildSearchNodes();
            }
            else
            {
                BuildSearchNodes(true);
            }
            GC.Collect();
        }

        public class MatchedWordPosition
        {
            public int Start;
            public int Length;
        }

        public IEnumerable<MatchedWordPosition> GetMatchedWordPositions(string text, int searchStartPos = 0)
        {
            text = text.Substring(searchStartPos);
            if (!regExp)
            {
                StringComparison comparation = caseSensitive
                    ? StringComparison.CurrentCulture
                    : StringComparison.CurrentCultureIgnoreCase;

                int start;
                int index;
                foreach (var word in _searchStrs)
                {
                    start = 0;
                    while ((index = text.IndexOf(word, start, comparation)) >= 0)
                    {
                        yield return new MatchedWordPosition() { Length = word.Length, Start = searchStartPos + index };
                        start = index + 1; // search for next occurrence
                    }
                }
            }
            else
            {
                RegexOptions regExpOptions = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                for (int indexRegExpStrings = 0; indexRegExpStrings < _searchStrs.Length; indexRegExpStrings++)
                {
                    string hlStr = _searchStrs[indexRegExpStrings];
                    if (hlStr != "")
                    {
                        var regEx = new Regex(hlStr, regExpOptions);
                        var regExpFindResults = regEx.Matches(text);

                        foreach (Match match in regExpFindResults)
                        {
                            yield return new MatchedWordPosition() { Length = match.Length, Start = searchStartPos + match.Index};
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Подсветка строк поиска в текущем тексте разработки
        /// </summary>
        public class HighlightSearchedStrings : DocumentColorizingTransformer
        {
            private Search Search { get; set; }
            public HighlightSearchedStrings(Search search)
            {
                Search = search;
            }

            protected override void ColorizeLine(DocumentLine line)
            {
                if (Search._searchStrs.Length > 0)
                {
                    int lineStartOffset = line.Offset;
                    string text = CurrentContext.Document.GetText(line);
                    Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 156, 255, 156));

                    foreach (MatchedWordPosition pos in Search.GetMatchedWordPositions(text))
                    {
                        base.ChangeLinePart(
                            lineStartOffset + pos.Start, // startOffset
                            lineStartOffset + pos.Start + pos.Length, // endOffset
                            (VisualLineElement element) =>
                            {
                                element.BackgroundBrush = brush;
                            });
                    } 
                } 
            }
        }

        /// <summary>
        /// Подсветка некорректных регулярных выражений
        /// </summary>
        public class HighlightIncorrectRegExp : DocumentColorizingTransformer
        {
            private Search Search { get; set; }
            public HighlightIncorrectRegExp(Search search)
            {
                Search = search;
            }
            protected override void ColorizeLine(DocumentLine line)
            {
                int lineStartOffset = line.Offset;
                string text = CurrentContext.Document.GetText(line).Trim();
                Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));

                if (text != "")
                {
                    if (Search.regExp)
                    {
                        RegexOptions regExpOptions = Search.caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                    
                        try
                        {
                            new Regex(text, regExpOptions);
                        }
                        catch (ArgumentException)
                        {
                            base.ChangeLinePart(
                                lineStartOffset, // startOffset
                                lineStartOffset + line.Length, // endOffset
                                (VisualLineElement element) =>
                                {
                                    element.BackgroundBrush = brush;
                                });
                        } 
                    }
                }
            }
        }

        public void FillTreeView(TreeView treeViewResults)
        {
            FilterNodesName();
            FilterNodesDate();
            treeViewResults.Nodes.Clear();
            treeViewResults.BeginUpdate();
            CopySearchNodesToTreeNodes(Nodes, treeViewResults.Nodes);
            treeViewResults.EndUpdate();
        }

        void FilterNodesName()
        {
            foreach (SearchNode node in Nodes)
            {
                FilterNodeName(node);
            }
        }
        void FilterNodesDate()
        {
            foreach (SearchNode node in Nodes)
            {
                FilterNodeDate(node);
            }
        }

        void SetVisible(SearchNode node)
        {
            node.Visible = true;
            foreach (SearchNode childNode in node.Nodes)
            {
                SetVisible(childNode);
            }
        }

        bool FilterNodeName(SearchNode node)
        {
            if (node == null)
                return false;

            bool isFound = false;
            //Пропуск пустых поисковых строк
            if (string.IsNullOrEmpty(FilterName))
            {
                SetVisible(node);
                isFound = true;
            }
            else
            {
                if (node.Name.ToUpper().Contains(FilterName.ToUpper()))
                {
                    SetVisible(node);
                    isFound = true;
                }
                else
                {
                    foreach (SearchNode subNode in node.Nodes)
                    {
                        if (FilterNodeName(subNode))
                        {
                            isFound = true;
                        }
                    }
                }  
                node.Visible = isFound;
            }

            return isFound;
        }

        bool FilterNodeDate(SearchNode node)
        {
            if (node == null)
                return false;

            bool isFound = false;

            if (!(FilterEndDate == System.Windows.Forms.DateTimePicker.MaximumDateTime 
                && FilterStartDate == System.Windows.Forms.DateTimePicker.MinDateTime)
                && node.Visible)
            {
                if (node.LastUpdate.HasValue)
                {
                    isFound = node.LastUpdate >= FilterStartDate && node.LastUpdate <= FilterEndDate;
                }
                else
                {
                    foreach (SearchNode subNode in node.Nodes)
                    {
                        if (FilterNodeDate(subNode))
                        {
                            isFound = true;
                        }
                    }
                }
                node.Visible = node.Visible & isFound;
            }
            return isFound;
        }

        void CopySearchNodesToTreeNodes(List<SearchNode> searchNodes, TreeNodeCollection treeNodes)
        {
            foreach (SearchNode node in searchNodes)
            {
                if (node.Visible)
                {
                    TreeNode treeNode = treeNodes.Add(node.Name);
                    treeNode.Tag = node;

                    if (node.IsMatch)
                    {
                        treeNode.ForeColor = System.Drawing.Color.Black;
                    }
                    else
                    {
                        treeNode.ForeColor = System.Drawing.Color.Gray;
                    }

                    CopySearchNodesToTreeNodes(node.Nodes, treeNode.Nodes);
                }
            }
        }

        /// <summary>
        /// Возврат всех слов из текста реактора поискового запроса, 
        /// разделителями слов в тексте выступают пробельные символы.
        /// Выделение слов, удовлетворяющих криетериям поиска.
        /// </summary>
        /// <returns></returns>
        void SearchCriteriaChanged(object sender, EventArgs e)
        {
            if (regExp)
            {
                _searchStrs = CheckRegExpFormat();
            }
            else
            {
                var searchLineStrs = SearchCriteriaTextEditor.Text.Split(
                new string[] { "\n" }
                , StringSplitOptions.RemoveEmptyEntries
                );
                char[] delimeters = { ' ', '\t', '\n', '\r' };
                var searchWords = new List<string>();
                foreach (string searchLine in searchLineStrs)
                {
                    foreach (string searchWord in searchLine.Split(delimeters, StringSplitOptions.RemoveEmptyEntries))
                    {
                        searchWords.Add(searchWord.Trim());
                    }
                }
                _searchStrs = searchWords.ToArray();
            }

            TextEditor.TextArea.TextView.Redraw();
        }


        /// <summary>
        /// Проверка списка регулярных выражений на корректность и возврат списка строк корректных регулярных выражений.
        /// </summary>
        /// <returns></returns>
        string[] CheckRegExpFormat()
        {
            List<string> regExpResultList = new List<string>();
            bool errorExist = false;
            for (int indexRegExpCandidate = 1; indexRegExpCandidate <= SearchCriteriaTextEditor.Document.LineCount; indexRegExpCandidate++)
            {
                var line = SearchCriteriaTextEditor.Document.GetLineByNumber(indexRegExpCandidate);
                ISegment segment = new TextSegment { StartOffset = line.Offset, EndOffset = line.EndOffset };
                var regExpCandidateString = SearchCriteriaTextEditor.Document.GetText(segment);
                try
                {
                    if (!string.IsNullOrEmpty(regExpCandidateString.Trim()))
                    {
                        new Regex(regExpCandidateString, RegexOptions.Compiled);
                        regExpResultList.Add(regExpCandidateString);
                    }
                }
                catch (ArgumentException)
                {
                    errorExist = true;
                }
            }
            if (errorExist)
            {
                SearchCriteriaTextEditor.TextArea.TextView.Redraw();
            }
            return regExpResultList.ToArray();
        }

            

        void BuildSearchNodes(bool copyAll = false)
        {
            Nodes.Clear();
            CopyMatchedIsblNodes(Nodes, IsbDev.Nodes, copyAll);
        }

        void CopyMatchedIsblNodes(List<SearchNode> targetNodes, List<IsbNode> sourceNodes, bool copyAll)
        {
            foreach (var isblNode in sourceNodes)
            {
                if (isblNode.IsMatch || isblNode.IsContainsMatchedNode || copyAll)
                {
                    var searchNode = new SearchNode(isblNode);
                    if (copyAll) searchNode.Visible = true;
                    if (isblNode.Nodes != null)
                    {
                        searchNode.Nodes = new List<SearchNode>();
                        CopyMatchedIsblNodes(searchNode.Nodes, isblNode.Nodes, copyAll);
                    }
                    targetNodes.Add(searchNode);
                }
            }
        }

        public void Dispose()
        {
            Nodes = null;
            IsbDev = null;
            _searchStrs = null;
            TextEditor = null;
            SearchCriteriaTextEditor = null;
        }
    }
}
