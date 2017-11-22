using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CheckBox = System.Windows.Forms.CheckBox;
using FontStyle = System.Drawing.FontStyle;
using MessageBox = System.Windows.Forms.MessageBox;
using Point = System.Drawing.Point;
using TextBox = System.Windows.Forms.TextBox;
using TreeView = System.Windows.Forms.TreeView;

namespace ISBLScan.ViewCode
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        Loader _isblLoader = new Loader();
        public IsbDev SourceDev = new IsbDev();
        public class SearchControls : IDisposable
        {
            public System.Windows.Forms.CheckBox CheckBoxFindRegExp { get; set; }
            public System.Windows.Forms.CheckBox CheckBoxFindAll { get; set; }
            public System.Windows.Forms.CheckBox CheckBoxFindCaseSensitive { get; set; }
            public System.Windows.Forms.Integration.ElementHost TextEditorHost { get; set; }
            public System.Windows.Forms.Integration.ElementHost SearchCriteriaTextEditorHost { get; set; }
            public System.Windows.Forms.TextBox TextBoxFilterName { get; set; }
            public System.Windows.Forms.DateTimePicker TextBoxFilterStartDate { get; set; }
            public System.Windows.Forms.DateTimePicker TextBoxFilterEndDate { get; set; }
            public TreeView TreeViewResults = new TreeView();
            public Search Search { get; set; }
            public MainForm _form { get; set; }
            private TabPage _tab { get; set; }
            private List<TreeNode> AlreadyAutoSelectedTreeNodes { get; set; } = new List<TreeNode>();

            public SearchControls(MainForm form)
            {
                _form = form;
                _tab = new TabPage(string.Format("Search {0}", _form.tabControlSearchText.TabCount + 1));
                _form.tabControlSearchText.Controls.Add(_tab);
                _form.tabControlSearchText.SelectedTab = _tab;
                Search = new Search(form.SourceDev);
                Search.SearchControls = this;

                SearchCriteriaTextEditorHost = new System.Windows.Forms.Integration.ElementHost();
                SearchCriteriaTextEditorHost.Dock = DockStyle.Fill;
                SearchCriteriaTextEditorHost.Location = new Point(0, 0);
                SearchCriteriaTextEditorHost.TabIndex = 1;
                SearchCriteriaTextEditorHost.Child = Search.SearchCriteriaTextEditor;

                _tab.Tag = this;
                _tab.Controls.Add(SearchCriteriaTextEditorHost);
                _tab.UseVisualStyleBackColor = true;
                Search.SearchCriteriaTextEditor.Focus();

                Search.FillTreeView(TreeViewResults);
                TreeViewResults.CheckBoxes = false;
                TreeViewResults.Dock = DockStyle.Fill;
                TreeViewResults.HideSelection = false;
                TreeViewResults.Location = new Point(0, 0);
                TreeViewResults.Margin = new Padding(4);
                TreeViewResults.Name = "treeViewResults";
                TreeViewResults.TabIndex = 2121;
                TreeViewResults.AfterSelect += new TreeViewEventHandler(form.TreeViewResultsAfterSelect);
                _form.panelTree.Controls.Add(TreeViewResults);

                TextEditorHost = new System.Windows.Forms.Integration.ElementHost();
                TextEditorHost.Dock = DockStyle.Fill;
                TextEditorHost.Location = new Point(0, 0);
                TextEditorHost.Margin = new System.Windows.Forms.Padding(4);
                TextEditorHost.TabIndex = 150;
                TextEditorHost.Child = Search.TextEditor;
                _form.panelISBLResult.Controls.Add(TextEditorHost);

                TextBoxFilterName = new System.Windows.Forms.TextBox();
                TextBoxFilterName.Dock = System.Windows.Forms.DockStyle.Fill;
                TextBoxFilterName.Location = new System.Drawing.Point(0, 0);
                TextBoxFilterName.Margin = new System.Windows.Forms.Padding(4);
                TextBoxFilterName.Name = "textBoxFilter";
                TextBoxFilterName.Size = new System.Drawing.Size(280, 22);
                TextBoxFilterName.TabIndex = 2111;
                TextBoxFilterName.TextChanged += new System.EventHandler(form.TextBoxFilter_TextChanged);
                _form.panelFilterTreeName.Controls.Add(TextBoxFilterName);

                TextBoxFilterStartDate = new System.Windows.Forms.DateTimePicker();
                TextBoxFilterStartDate.Format = DateTimePickerFormat.Custom;
                TextBoxFilterStartDate.CustomFormat = "yyyy-MM-dd HH:mm";
                TextBoxFilterStartDate.Location = new System.Drawing.Point(0, 0);
                TextBoxFilterStartDate.Margin = new System.Windows.Forms.Padding(4);
                TextBoxFilterStartDate.Name = "textBoxFilterStartDate";
                TextBoxFilterStartDate.Size = new System.Drawing.Size(130, 22);
                TextBoxFilterStartDate.TabIndex = 2112;
                TextBoxFilterStartDate.TextChanged += new System.EventHandler(form.TextBoxFilterStartDate_TextChanged);
                TextBoxFilterStartDate.Value = DateTimePicker.MinimumDateTime.Date;
                _form.panelFilterTreeDate.Controls.Add(TextBoxFilterStartDate);

                TextBoxFilterEndDate = new System.Windows.Forms.DateTimePicker();
                TextBoxFilterEndDate.Format = DateTimePickerFormat.Custom;
                TextBoxFilterEndDate.CustomFormat = "yyyy-MM-dd HH:mm";
                TextBoxFilterEndDate.Location = new System.Drawing.Point(150, 0);
                TextBoxFilterEndDate.Margin = new System.Windows.Forms.Padding(4);
                TextBoxFilterEndDate.Name = "textBoxFilterEndDate";
                TextBoxFilterEndDate.Size = new System.Drawing.Size(130, 22);
                TextBoxFilterEndDate.TabIndex = 2113;
                TextBoxFilterEndDate.TextChanged += new System.EventHandler(form.TextBoxFilterEndDate_TextChanged);
                TextBoxFilterEndDate.Value = DateTimePicker.MaximumDateTime.Date.AddDays(-1).AddHours(23).AddMinutes(59);
                _form.panelFilterTreeDate.Controls.Add(TextBoxFilterEndDate);

                CheckBoxFindAll = new System.Windows.Forms.CheckBox();
                CheckBoxFindAll.AutoSize = true;
                CheckBoxFindAll.Dock = System.Windows.Forms.DockStyle.None;
                CheckBoxFindAll.Location = new System.Drawing.Point(320, 4);
                CheckBoxFindAll.Margin = new System.Windows.Forms.Padding(4);
                CheckBoxFindAll.Name = "checkBoxFindAll";
                CheckBoxFindAll.Size = new System.Drawing.Size(146, 28);
                CheckBoxFindAll.TabIndex = 1226;
                CheckBoxFindAll.Text = "satisfies all criteria";
                CheckBoxFindAll.UseVisualStyleBackColor = true;
                CheckBoxFindAll.CheckedChanged += new System.EventHandler(form.checkBoxFindAll_CheckedChanged);
                _form.panelSearchButtons.Controls.Add(CheckBoxFindAll);

                CheckBoxFindRegExp = new System.Windows.Forms.CheckBox();
                CheckBoxFindRegExp.AutoSize = true;
                CheckBoxFindRegExp.Dock = System.Windows.Forms.DockStyle.None;
                CheckBoxFindRegExp.Location = new System.Drawing.Point(150, 4);
                CheckBoxFindRegExp.Margin = new System.Windows.Forms.Padding(4);
                CheckBoxFindRegExp.Name = "CheckBoxFindRegExp";
                CheckBoxFindRegExp.Size = new System.Drawing.Size(170, 28);
                CheckBoxFindRegExp.TabIndex = 1225;
                CheckBoxFindRegExp.Text = ".* (regular expression)";
                CheckBoxFindRegExp.UseVisualStyleBackColor = true;
                CheckBoxFindRegExp.CheckedChanged += new System.EventHandler(form.checkBoxFindRegExp_CheckedChanged);
                _form.panelSearchButtons.Controls.Add(CheckBoxFindRegExp);

                CheckBoxFindCaseSensitive = new System.Windows.Forms.CheckBox();
                CheckBoxFindCaseSensitive.AutoSize = true;
                CheckBoxFindCaseSensitive.Dock = System.Windows.Forms.DockStyle.None;
                CheckBoxFindCaseSensitive.Location = new System.Drawing.Point(2, 4);
                CheckBoxFindCaseSensitive.Margin = new System.Windows.Forms.Padding(4);
                CheckBoxFindCaseSensitive.Name = "checkBoxFindCaseSensitive";
                CheckBoxFindCaseSensitive.Size = new System.Drawing.Size(150, 28);
                CheckBoxFindCaseSensitive.TabIndex = 1224;
                CheckBoxFindCaseSensitive.Text = "Aa (case sensitive)";
                CheckBoxFindCaseSensitive.UseVisualStyleBackColor = true;
                CheckBoxFindCaseSensitive.CheckedChanged += new System.EventHandler(form.checkBoxFindCaseSensitive_CheckedChanged);
                _form.panelSearchButtons.Controls.Add(CheckBoxFindCaseSensitive);

                this.Activate();
            }

            public void Activate()
            {
                TreeViewResults.BringToFront();
                TextEditorHost.BringToFront();
                TextBoxFilterName.BringToFront();
                TextBoxFilterStartDate.BringToFront();
                TextBoxFilterEndDate.BringToFront();
                CheckBoxFindRegExp.BringToFront();
                CheckBoxFindAll.BringToFront();
                CheckBoxFindCaseSensitive.BringToFront();
            }

            public void Dispose()
            {
                TextEditorHost.Child = null;
                SearchCriteriaTextEditorHost.Child = null;
                _tab.Controls.Remove(SearchCriteriaTextEditorHost);
                _form.panelTree.Controls.Remove(TreeViewResults);
                _form.panelISBLResult.Controls.Remove(TextEditorHost);
                _form.panelFilterTreeName.Controls.Remove(TextBoxFilterName);
                _form.panelFilterTreeDate.Controls.Remove(TextBoxFilterStartDate);
                _form.panelFilterTreeDate.Controls.Remove(TextBoxFilterEndDate);
                _form.panelSearchButtons.Controls.Remove(CheckBoxFindCaseSensitive);
                _form.panelSearchButtons.Controls.Remove(CheckBoxFindAll);
                _form.panelSearchButtons.Controls.Remove(CheckBoxFindRegExp);
                Search.Nodes.Clear();
                Search.Dispose();
                TreeViewResults.Nodes.Clear();
                TreeViewResults.Dispose();
                TextEditorHost.Dispose();
                SearchCriteriaTextEditorHost.Dispose();
                TextBoxFilterName.Dispose();
                CheckBoxFindCaseSensitive.Dispose();
                CheckBoxFindAll.Dispose();
                CheckBoxFindRegExp.Dispose();
            }

            public void Process()
            {
                _form.buttonSearch.Enabled = false;
                Search.Process();
                Search.FillTreeView(TreeViewResults);
                AlreadyAutoSelectedTreeNodes.Clear();
                TreeSelectNextMatched(TreeViewResults.Nodes);
                _form.buttonSearch.Enabled = true;
            }

            public bool TreeSelectNextMatched(TreeNodeCollection treeNodes)
            {
                bool isFound = false;
                foreach (TreeNode node in treeNodes)
                {
                    var searchNode = (SearchNode)node.Tag;
                    if (searchNode.IsMatch && !AlreadyAutoSelectedTreeNodes.Contains(node))
                    {
                        TreeViewResults.SelectedNode = node;
                        AlreadyAutoSelectedTreeNodes.Add(node);
                        isFound = true;
                        break;
                    }
                    else
                    {
                        isFound = TreeSelectNextMatched(node.Nodes);
                        if (isFound)
                        {
                            break;
                        }
                    }
                }
                return isFound;
            }

            public void TextEditorShowNextMatchedString(bool gotoNextTreeNodeIfNoMore = true)
            {
                var matchedPos = Search.GetMatchedWordPositions(Search.TextEditor.Text, Search.TextEditor.TextArea.Caret.Offset).FirstOrDefault();
                if (matchedPos != null)
                {
                    Search.TextEditor.TextArea.Caret.Offset = matchedPos.Start + matchedPos.Length;
                    Search.TextEditor.TextArea.Caret.BringCaretToView();
                    Search.TextEditor.SelectionStart = matchedPos.Start;
                    Search.TextEditor.SelectionLength = matchedPos.Length;
                }
                else
                {
                    if (gotoNextTreeNodeIfNoMore) TreeSelectNextMatched(TreeViewResults.Nodes);
                }
            }
        }

        public MainForm()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            this.Load += Window_Loaded;
            this.KeyDown += MainForm_KeyDown;
            groupBoxSearch_Resize(null, null);
            new SearchControls(this);

            string sqlServer;
            string dataBase;
            string login;
            string password;
            bool isWinAuth = false;


            var namedArguments = new Dictionary<string, string>();
            String[] arguments = Environment.GetCommandLineArgs();

            foreach (string argument in arguments)
            {
                string[] splitted = argument.Split('=');

                if (splitted.Length == 2)
                {
                    namedArguments[splitted[0]] = splitted[1];
                }
            }
            if (namedArguments.Count > 0)
            {
                namedArguments.TryGetValue("-S", out sqlServer);
                namedArguments.TryGetValue("-D", out dataBase);
                namedArguments.TryGetValue("-N", out login);
                namedArguments.TryGetValue("-W", out password);

                textBoxSQLServer.Text = sqlServer;
                textBoxDB.Text = dataBase;
                textBoxLogin.Text = login;
                checkBoxWinAuth.Checked = String.IsNullOrWhiteSpace(password);
                textBoxPassword.Text = password;

                ConnectAndGetIsbl();
            }
            else
            {
                if (Configuration.Load(out sqlServer, out dataBase, out login, out isWinAuth))
                {
                    textBoxSQLServer.Text = sqlServer;
                    textBoxDB.Text = dataBase;
                    textBoxLogin.Text = login;
                    if (isWinAuth)
                    {
                        textBoxPassword.Text = "";
                        checkBoxWinAuth.Checked = isWinAuth;
                    }
                    else
                    {
                        textBoxPassword.Text = "";
                    }
                }
            }

        }

        /// <summary>
        /// Установка соединения с базой данных SQL Server
        /// </summary>
        /// <returns></returns>
		bool Connect()
        {
            bool connect;
            if (checkBoxWinAuth.Checked)
            {
                connect = _isblLoader.Connect(textBoxSQLServer.Text,
                                   textBoxDB.Text,
                                   "",
                                   "",
                                   true);
            }
            else
            {
                connect = _isblLoader.Connect(textBoxSQLServer.Text,
                                   textBoxDB.Text,
                                   textBoxLogin.Text,
                                   textBoxPassword.Text,
                                   false
                                  );
            }
            if (connect)
            {
                Configuration.Save(textBoxSQLServer.Text, textBoxDB.Text, textBoxLogin.Text, checkBoxWinAuth.Checked);
                SourceDev.ConnectionParams.Server = textBoxSQLServer.Text;
                SourceDev.ConnectionParams.Database = textBoxDB.Text;
                SourceDev.ConnectionParams.Login = textBoxLogin.Text;
                SourceDev.ConnectionParams.Password = checkBoxWinAuth.Checked ? null : textBoxPassword.Text;
            }
            return connect;
        }

        /// <summary>
        /// Загрузка элементов разработки из базы данных
        /// </summary>
        bool GetIsbl()
        {
            SourceDev.Nodes.Clear();
            _isblLoader.Load(SourceDev.Nodes);
            SourceDev.FillParent();

            bool isblNodesIsEmpty = true;
            foreach (var node in SourceDev.Nodes)
            {
                if (node != null)
                {
                    isblNodesIsEmpty = false;
                    break;
                }
            }
            if (isblNodesIsEmpty)
            {
                string loginSqlUser = this.textBoxLogin.Text;
                if (this.checkBoxWinAuth.Checked)
                {
                    loginSqlUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                }
                else
                {
                    loginSqlUser = this.textBoxLogin.Text;
                }
                string text = string.Format(
@"В выбранной базе данных ""{0}"" сервера ""{1}"" отсутствуют таблицы разработки.
Или у пользователя с логином ""{2}"" нет прав на просмотр содержимого таблиц разработки в указанной базе данных."
                , this.textBoxDB.Text
                , this.textBoxSQLServer.Text
                , loginSqlUser
);
                string caption = "Разработка не загружена";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBoxIcon icon = MessageBoxIcon.Information;
                //MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1;
                MessageBox.Show(text, caption, buttons, icon);
                return false;
            }
            else return true;
        }

        /// <summary>
        /// Соединение с SQL Server и загрузка элементов разработки из базы данных
        /// </summary>
        void ConnectAndGetIsbl()
        {
            if ((textBoxSQLServer.Text.Trim() != "") && (textBoxDB.Text.Trim() != "") &&
                   (checkBoxWinAuth.Checked || (!checkBoxWinAuth.Checked && textBoxLogin.Text.Trim() != ""))
              )
            {
                if (Connect())
                {
                    if (GetIsbl())
                    {
                        var searchControls = (SearchControls)tabControlSearchText.SelectedTab.Tag;
                        searchControls.Search.Process();
                        searchControls.Search.FillTreeView(searchControls.TreeViewResults);
                        searchControls.Search.SearchCriteriaTextEditor.Focus();
                        buttonSearch.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show(_isblLoader.ErrorText, "Ошибка открытия базы данных");
                }
            }
            else
            {
                if (textBoxLogin.Text.Trim() == "")
                {
                    textBoxLogin.Text = "Login";
                    textBoxLogin.Font = new Font(textBoxLogin.Font, FontStyle.Italic);
                    textBoxLogin.BackColor = Color.LightCoral;
                    textBoxLogin.SelectAll();
                    textBoxLogin.Focus();
                }
                if (textBoxDB.Text.Trim() == "")
                {
                    textBoxDB.Text = "Data Base";
                    textBoxDB.Font = new Font(textBoxDB.Font, FontStyle.Italic);
                    textBoxDB.BackColor = Color.LightCoral;
                    textBoxDB.SelectAll();
                    textBoxDB.Focus();
                }
                if (textBoxSQLServer.Text.Trim() == "")
                {
                    textBoxSQLServer.Text = "Sql Server";
                    textBoxSQLServer.Font = new Font(textBoxSQLServer.Font, FontStyle.Italic);
                    textBoxSQLServer.BackColor = Color.LightCoral;
                    textBoxSQLServer.SelectAll();
                    textBoxSQLServer.Focus();
                }
            }
        }

        private void Window_Loaded(object sender, EventArgs eventArgs)
        {
            var searchControls = (SearchControls)tabControlSearchText.SelectedTab.Tag;
            searchControls.Search.SearchCriteriaTextEditor.Focus();
        }

        /// <summary>
        /// Нажатие кнопки "Conact and Load ISBL"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void ButtonConnectClick(object sender, EventArgs e)
        {
            ConnectAndGetIsbl();
        }

        /// <summary>
        /// Нажатие кнопки "Find"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void ButtonFilterClick(object sender, EventArgs e)
        {
            var searchControls = (SearchControls)tabControlSearchText.SelectedTab.Tag;
            searchControls.Process();
        }

        /// <summary>
        /// Обработка нажатия клавиши в форме аутентификации на SQL Server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>		
        void TextBoxLoginFormKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ConnectAndGetIsbl();
            }
        }

        void TextBoxLoginFormTextChanged(object sender, EventArgs e)
        {
            (sender as TextBox).Font = new Font((sender as TextBox).Font, FontStyle.Regular);
            (sender as TextBox).BackColor = this.textBoxPassword.BackColor;
            (sender as TextBox).ForeColor = this.textBoxPassword.ForeColor;
        }

        /// <summary>
        /// Изменение текста в поле фильтрации дерева проекта
        /// </summary>
        public void TextBoxFilter_TextChanged(object sender, System.EventArgs e)
        {
            var searchControls = (SearchControls)tabControlSearchText.SelectedTab.Tag;
            searchControls.Search.FilterName = ((TextBox)sender).Text;
            searchControls.Search.FillTreeView(searchControls.TreeViewResults);
        }
        /// <summary>
        /// Изменение начальной даты в поле фильтрации дерева проекта
        /// </summary>
        public void TextBoxFilterStartDate_TextChanged(object sender, System.EventArgs e)
        {
            var searchControls = (SearchControls)tabControlSearchText.SelectedTab.Tag;
            searchControls.Search.FilterStartDate = ((DateTimePicker)sender).Value;
            searchControls.Search.FillTreeView(searchControls.TreeViewResults);
        }
        /// <summary>
        /// Изменение конечной даты в поле фильтрации дерева проекта
        /// </summary>
        public void TextBoxFilterEndDate_TextChanged(object sender, System.EventArgs e)
        {
            var searchControls = (SearchControls)tabControlSearchText.SelectedTab.Tag;
            searchControls.Search.FilterEndDate = ((DateTimePicker)sender).Value;
            searchControls.Search.FillTreeView(searchControls.TreeViewResults);
        }

        void CheckBoxWinAuthCheckedChanged(object sender, EventArgs e)
        {
            textBoxLogin.Enabled = !checkBoxWinAuth.Checked;
            textBoxPassword.Enabled = textBoxLogin.Enabled;
        }

        private void buttonCloseCurrentTab_Click(object sender, EventArgs e)
        {
            if (tabControlSearchText.TabCount > 1)
            {
                var tabPageForClose = tabControlSearchText.SelectedTab;
                ((SearchControls)tabPageForClose.Tag).Dispose();
                tabPageForClose.Tag = null;

                tabControlSearchText.SelectedIndex = tabControlSearchText.SelectedIndex > 0 ?
                    tabControlSearchText.SelectedIndex - 1 : 0;
                tabControlSearchText.Controls.Remove(tabPageForClose);
                tabPageForClose.Dispose();
                GC.Collect();
            }
        }

        private void buttonAddNewTab_Click(object sender, EventArgs e)
        {
            new SearchControls(this);
        }

        public void checkBoxFindRegExp_CheckedChanged(object sender, EventArgs e)
        {
            ((SearchControls)tabControlSearchText.SelectedTab.Tag).Search.RegExp = ((CheckBox)sender).Checked;
        }

        public void checkBoxFindAll_CheckedChanged(object sender, EventArgs e)
        {
            ((SearchControls)tabControlSearchText.SelectedTab.Tag).Search.FindAll = ((CheckBox)sender).Checked;
        }

        public void tabControlSearchText_SelectedIndexChanged(object sender, EventArgs e)
        {
            ((SearchControls)tabControlSearchText.SelectedTab.Tag)?.Activate();
        }

        public void checkBoxFindCaseSensitive_CheckedChanged(object sender, EventArgs e)
        {
            ((SearchControls)tabControlSearchText.SelectedTab.Tag).Search.CaseSensitive = ((CheckBox)sender).Checked;
        }

        void groupBoxSearch_Resize(object sender, System.EventArgs e)
        {
            this.buttonAddNewTab.Left = groupBoxSearch.Width - 36;
            this.buttonCloseCurrentTab.Left = groupBoxSearch.Width - 19;
        }

        private void buttonExpand_Click(object sender, EventArgs e)
        {
            var treeViewResults = ((SearchControls)tabControlSearchText.SelectedTab.Tag).TreeViewResults;
            treeViewResults.BeginUpdate();
            foreach (TreeNode node in treeViewResults.Nodes)
            {
                node.ExpandAll();
            }
            treeViewResults.EndUpdate();
        }
        /// <summary>
        /// Выбор узла в дереве разработки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewResultsAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                var node = (SearchNode)e.Node.Tag;
                var searchControls = (SearchControls)tabControlSearchText.SelectedTab.Tag;
                searchControls.Search.TextEditor.Text = node.IsbNode.Text;
                searchControls.Search.TextEditor.TextArea.Caret.Offset = 0;
                searchControls.TextEditorShowNextMatchedString(false);
                toolStripStatusLabelSelectedElement.Text = node.Name;
                toolStripStatusLabelLastUpd.Text = node.IsbNode.LastUpdate?.ToString("yyyy-MM-dd HH:mm");
            }
            else
            {
                ((SearchControls)tabControlSearchText.SelectedTab.Tag).Search.TextEditor.Text = "";
                toolStripStatusLabelSelectedElement.Text = "";
                toolStripStatusLabelLastUpd.Text = "";
            }
        }

        public void MainForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                var searchControls = (SearchControls)tabControlSearchText.SelectedTab.Tag;
                searchControls.TreeSelectNextMatched(searchControls.TreeViewResults.Nodes);
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.F3)
            {
                var searchControls = (SearchControls)tabControlSearchText.SelectedTab.Tag;
                searchControls.TextEditorShowNextMatchedString();
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.T && e.Control)
            {
                new SearchControls(this);
            }
            if (e.KeyCode == Keys.O && e.Control)
            {
                var selectedNode = ((SearchControls)tabControlSearchText.SelectedTab.Tag).TreeViewResults.SelectedNode;
                if (selectedNode != null)
                {
                    ((SearchNode)selectedNode.Tag).IsbNode.OpenInSbrte(SourceDev.ConnectionParams);
                }

            }
        }
    }
}
