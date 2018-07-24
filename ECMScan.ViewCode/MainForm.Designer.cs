using ISBLScan.ViewCode;
namespace ISBLScan.ViewCode
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

	
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelLastUpd = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelDash = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelSelectedElement = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelTest = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelResults = new System.Windows.Forms.Panel();
            this.splitContainerResults = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.groupBoxSearch = new System.Windows.Forms.GroupBox();
            this.buttonAddNewTab = new System.Windows.Forms.Button();
            this.buttonCloseCurrentTab = new System.Windows.Forms.Button();
            this.panelSearchText = new System.Windows.Forms.Panel();
            this.tabControlSearchText = new System.Windows.Forms.TabControl();
            this.panelSearchButtons = new System.Windows.Forms.Panel();
            this.buttonExpand = new System.Windows.Forms.Button();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.panelFilterTree = new System.Windows.Forms.Panel();
            this.panelFilterTreeDate = new System.Windows.Forms.Panel();
            this.panelFilterTreeName = new System.Windows.Forms.Panel();
            this.panelTree = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_save = new System.Windows.Forms.Button();
            this.panelISBLResult = new System.Windows.Forms.Panel();
            this.tabPageSimpleSearchText = new System.Windows.Forms.TabPage();
            this.statusStrip1.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerResults)).BeginInit();
            this.splitContainerResults.Panel1.SuspendLayout();
            this.splitContainerResults.Panel2.SuspendLayout();
            this.splitContainerResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.groupBoxSearch.SuspendLayout();
            this.panelSearchText.SuspendLayout();
            this.panelSearchButtons.SuspendLayout();
            this.panelFilterTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.White;
            this.imageList1.Images.SetKeyName(0, "close8.png");
            this.imageList1.Images.SetKeyName(1, "add8.png");
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelLastUpd,
            this.toolStripStatusLabelDash,
            this.toolStripStatusLabelSelectedElement,
            this.toolStripStatusLabelTest});
            this.statusStrip1.Location = new System.Drawing.Point(0, 617);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(792, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelLastUpd
            // 
            this.toolStripStatusLabelLastUpd.Name = "toolStripStatusLabelLastUpd";
            this.toolStripStatusLabelLastUpd.Size = new System.Drawing.Size(106, 17);
            this.toolStripStatusLabelLastUpd.Text = "00.00.0000 00:00:00";
            // 
            // toolStripStatusLabelDash
            // 
            this.toolStripStatusLabelDash.Name = "toolStripStatusLabelDash";
            this.toolStripStatusLabelDash.Size = new System.Drawing.Size(12, 17);
            this.toolStripStatusLabelDash.Text = "-";
            // 
            // toolStripStatusLabelSelectedElement
            // 
            this.toolStripStatusLabelSelectedElement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabelSelectedElement.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.toolStripStatusLabelSelectedElement.Name = "toolStripStatusLabelSelectedElement";
            this.toolStripStatusLabelSelectedElement.Size = new System.Drawing.Size(36, 17);
            this.toolStripStatusLabelSelectedElement.Text = "None";
            // 
            // toolStripStatusLabelTest
            // 
            this.toolStripStatusLabelTest.Name = "toolStripStatusLabelTest";
            this.toolStripStatusLabelTest.Size = new System.Drawing.Size(0, 17);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelResults);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(792, 617);
            this.panelMain.TabIndex = 7;
            // 
            // panelResults
            // 
            this.panelResults.Controls.Add(this.splitContainerResults);
            this.panelResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelResults.Location = new System.Drawing.Point(0, 0);
            this.panelResults.Name = "panelResults";
            this.panelResults.Size = new System.Drawing.Size(792, 617);
            this.panelResults.TabIndex = 2;
            // 
            // splitContainerResults
            // 
            this.splitContainerResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerResults.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerResults.Location = new System.Drawing.Point(0, 0);
            this.splitContainerResults.Name = "splitContainerResults";
            // 
            // splitContainerResults.Panel1
            // 
            this.splitContainerResults.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainerResults.Panel1MinSize = 280;
            // 
            // splitContainerResults.Panel2
            // 
            this.splitContainerResults.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainerResults.Size = new System.Drawing.Size(792, 617);
            this.splitContainerResults.SplitterDistance = 312;
            this.splitContainerResults.SplitterWidth = 7;
            this.splitContainerResults.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panelTree);
            this.splitContainer2.Size = new System.Drawing.Size(312, 617);
            this.splitContainer2.SplitterDistance = 242;
            this.splitContainer2.TabIndex = 213;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.groupBoxSearch);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.panelFilterTree);
            this.splitContainer3.Size = new System.Drawing.Size(312, 242);
            this.splitContainer3.SplitterDistance = 192;
            this.splitContainer3.TabIndex = 0;
            // 
            // groupBoxSearch
            // 
            this.groupBoxSearch.Controls.Add(this.buttonAddNewTab);
            this.groupBoxSearch.Controls.Add(this.buttonCloseCurrentTab);
            this.groupBoxSearch.Controls.Add(this.panelSearchText);
            this.groupBoxSearch.Controls.Add(this.panelSearchButtons);
            this.groupBoxSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxSearch.Location = new System.Drawing.Point(0, 0);
            this.groupBoxSearch.Name = "groupBoxSearch";
            this.groupBoxSearch.Size = new System.Drawing.Size(312, 192);
            this.groupBoxSearch.TabIndex = 140;
            this.groupBoxSearch.TabStop = false;
            this.groupBoxSearch.Text = "Search Text";
            this.groupBoxSearch.DoubleClick += new System.EventHandler(this.buttonAddNewTab_Click);
            this.groupBoxSearch.Resize += new System.EventHandler(this.groupBoxSearch_Resize);
            // 
            // buttonAddNewTab
            // 
            this.buttonAddNewTab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAddNewTab.ImageIndex = 1;
            this.buttonAddNewTab.ImageList = this.imageList1;
            this.buttonAddNewTab.Location = new System.Drawing.Point(277, 3);
            this.buttonAddNewTab.Margin = new System.Windows.Forms.Padding(0);
            this.buttonAddNewTab.Name = "buttonAddNewTab";
            this.buttonAddNewTab.Size = new System.Drawing.Size(14, 14);
            this.buttonAddNewTab.TabIndex = 3000;
            this.buttonAddNewTab.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonAddNewTab.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonAddNewTab.UseVisualStyleBackColor = true;
            this.buttonAddNewTab.Click += new System.EventHandler(this.buttonAddNewTab_Click);
            // 
            // buttonCloseCurrentTab
            // 
            this.buttonCloseCurrentTab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCloseCurrentTab.ImageIndex = 0;
            this.buttonCloseCurrentTab.ImageList = this.imageList1;
            this.buttonCloseCurrentTab.Location = new System.Drawing.Point(291, 3);
            this.buttonCloseCurrentTab.Margin = new System.Windows.Forms.Padding(0);
            this.buttonCloseCurrentTab.Name = "buttonCloseCurrentTab";
            this.buttonCloseCurrentTab.Size = new System.Drawing.Size(14, 14);
            this.buttonCloseCurrentTab.TabIndex = 2000;
            this.buttonCloseCurrentTab.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonCloseCurrentTab.UseVisualStyleBackColor = true;
            this.buttonCloseCurrentTab.Click += new System.EventHandler(this.buttonCloseCurrentTab_Click);
            // 
            // panelSearchText
            // 
            this.panelSearchText.Controls.Add(this.tabControlSearchText);
            this.panelSearchText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSearchText.Location = new System.Drawing.Point(3, 16);
            this.panelSearchText.Name = "panelSearchText";
            this.panelSearchText.Size = new System.Drawing.Size(306, 114);
            this.panelSearchText.TabIndex = 124;
            // 
            // tabControlSearchText
            // 
            this.tabControlSearchText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlSearchText.Location = new System.Drawing.Point(0, 0);
            this.tabControlSearchText.Margin = new System.Windows.Forms.Padding(2);
            this.tabControlSearchText.Name = "tabControlSearchText";
            this.tabControlSearchText.SelectedIndex = 0;
            this.tabControlSearchText.Size = new System.Drawing.Size(306, 114);
            this.tabControlSearchText.TabIndex = 0;
            this.tabControlSearchText.SelectedIndexChanged += new System.EventHandler(this.tabControlSearchText_SelectedIndexChanged);
            // 
            // panelSearchButtons
            // 
            this.panelSearchButtons.Controls.Add(this.buttonExpand);
            this.panelSearchButtons.Controls.Add(this.buttonSearch);
            this.panelSearchButtons.Controls.Add(this.buttonConnect);
            this.panelSearchButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelSearchButtons.Location = new System.Drawing.Point(3, 130);
            this.panelSearchButtons.Margin = new System.Windows.Forms.Padding(0);
            this.panelSearchButtons.Name = "panelSearchButtons";
            this.panelSearchButtons.Size = new System.Drawing.Size(306, 59);
            this.panelSearchButtons.TabIndex = 300;
            // 
            // buttonExpand
            // 
            this.buttonExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonExpand.Location = new System.Drawing.Point(92, 31);
            this.buttonExpand.Name = "buttonExpand";
            this.buttonExpand.Size = new System.Drawing.Size(81, 25);
            this.buttonExpand.TabIndex = 1224;
            this.buttonExpand.Text = "Expand Tree";
            this.buttonExpand.UseVisualStyleBackColor = true;
            this.buttonExpand.Click += new System.EventHandler(this.buttonExpand_Click);
            // 
            // buttonSearch
            // 
            this.buttonSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearch.Enabled = false;
            this.buttonSearch.Location = new System.Drawing.Point(200, 3);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(106, 25);
            this.buttonSearch.TabIndex = 200;
            this.buttonSearch.Text = "Find";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.ButtonFilterClick);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonConnect.Location = new System.Drawing.Point(0, 31);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(83, 25);
            this.buttonConnect.TabIndex = 116;
            this.buttonConnect.Text = "Reload";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.ButtonConnectClick);
            // 
            // panelFilterTree
            // 
            this.panelFilterTree.Controls.Add(this.panelFilterTreeDate);
            this.panelFilterTree.Controls.Add(this.panelFilterTreeName);
            this.panelFilterTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFilterTree.Location = new System.Drawing.Point(0, 0);
            this.panelFilterTree.Name = "panelFilterTree";
            this.panelFilterTree.Size = new System.Drawing.Size(312, 46);
            this.panelFilterTree.TabIndex = 211;
            // 
            // panelFilterTreeDate
            // 
            this.panelFilterTreeDate.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFilterTreeDate.Location = new System.Drawing.Point(0, 26);
            this.panelFilterTreeDate.Margin = new System.Windows.Forms.Padding(2);
            this.panelFilterTreeDate.Name = "panelFilterTreeDate";
            this.panelFilterTreeDate.Size = new System.Drawing.Size(312, 20);
            this.panelFilterTreeDate.TabIndex = 1;
            // 
            // panelFilterTreeName
            // 
            this.panelFilterTreeName.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilterTreeName.Location = new System.Drawing.Point(0, 0);
            this.panelFilterTreeName.Margin = new System.Windows.Forms.Padding(2);
            this.panelFilterTreeName.Name = "panelFilterTreeName";
            this.panelFilterTreeName.Size = new System.Drawing.Size(312, 20);
            this.panelFilterTreeName.TabIndex = 0;
            // 
            // panelTree
            // 
            this.panelTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTree.Location = new System.Drawing.Point(0, 0);
            this.panelTree.Name = "panelTree";
            this.panelTree.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.panelTree.Size = new System.Drawing.Size(312, 371);
            this.panelTree.TabIndex = 212;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.button_cancel);
            this.splitContainer1.Panel1.Controls.Add(this.button_save);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelISBLResult);
            this.splitContainer1.Size = new System.Drawing.Size(473, 617);
            this.splitContainer1.SplitterDistance = 29;
            this.splitContainer1.TabIndex = 221;
            // 
            // button_cancel
            // 
            this.button_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_cancel.Location = new System.Drawing.Point(317, 3);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 1;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // button_save
            // 
            this.button_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_save.Location = new System.Drawing.Point(395, 3);
            this.button_save.Name = "button_save";
            this.button_save.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 0;
            this.button_save.Text = "Save";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // panelISBLResult
            // 
            this.panelISBLResult.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelISBLResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelISBLResult.Location = new System.Drawing.Point(0, 0);
            this.panelISBLResult.Name = "panelISBLResult";
            this.panelISBLResult.Size = new System.Drawing.Size(473, 584);
            this.panelISBLResult.TabIndex = 220;
            // 
            // tabPageSimpleSearchText
            // 
            this.tabPageSimpleSearchText.Location = new System.Drawing.Point(0, 0);
            this.tabPageSimpleSearchText.Name = "tabPageSimpleSearchText";
            this.tabPageSimpleSearchText.Size = new System.Drawing.Size(200, 100);
            this.tabPageSimpleSearchText.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 639);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.statusStrip1);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "ISBLScan ViewCode";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelResults.ResumeLayout(false);
            this.splitContainerResults.Panel1.ResumeLayout(false);
            this.splitContainerResults.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerResults)).EndInit();
            this.splitContainerResults.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.groupBoxSearch.ResumeLayout(false);
            this.panelSearchText.ResumeLayout(false);
            this.panelSearchButtons.ResumeLayout(false);
            this.panelFilterTree.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
        public System.Windows.Forms.ImageList imageListIcons;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelLastUpd;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDash;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSelectedElement;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelTest;
        private System.Windows.Forms.TabPage tabPageSimpleSearchText;
        private System.Windows.Forms.Panel panelResults;
        private System.Windows.Forms.SplitContainer splitContainerResults;
        private System.Windows.Forms.Panel panelTree;
        private System.Windows.Forms.GroupBox groupBoxSearch;
        private System.Windows.Forms.Button buttonAddNewTab;
        private System.Windows.Forms.Button buttonCloseCurrentTab;
        private System.Windows.Forms.Panel panelSearchText;
        private System.Windows.Forms.TabControl tabControlSearchText;
        private System.Windows.Forms.Panel panelSearchButtons;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Panel panelFilterTree;
        private System.Windows.Forms.Panel panelFilterTreeDate;
        private System.Windows.Forms.Panel panelFilterTreeName;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonExpand;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Panel panelISBLResult;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        //Main menu


    }
}
