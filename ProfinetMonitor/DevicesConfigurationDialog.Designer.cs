
namespace ProfinetMonitor
{
    partial class DevicesConfigurationDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DevicesConfigurationDialog));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listViewDevices = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.comboBoxNICs = new System.Windows.Forms.ComboBox();
            this.toolStripOnline = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAdd = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAddAll = new System.Windows.Forms.ToolStripButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.toolStripFile = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLoad = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRemove = new System.Windows.Forms.ToolStripButton();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonMonitorMode = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonLogFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonInfo = new System.Windows.Forms.ToolStripButton();
            this.listViewFile = new ProfinetMonitor.EditableListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.toolStripOnline.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.toolStripFile.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 76);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Size = new System.Drawing.Size(942, 395);
            this.splitContainer1.SplitterDistance = 388;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listViewDevices);
            this.groupBox1.Controls.Add(this.comboBoxNICs);
            this.groupBox1.Controls.Add(this.toolStripOnline);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(388, 395);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Online";
            // 
            // listViewDevices
            // 
            this.listViewDevices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listViewDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewDevices.HideSelection = false;
            this.listViewDevices.Location = new System.Drawing.Point(3, 62);
            this.listViewDevices.Name = "listViewDevices";
            this.listViewDevices.Size = new System.Drawing.Size(382, 330);
            this.listViewDevices.TabIndex = 2;
            this.listViewDevices.UseCompatibleStateImageBehavior = false;
            this.listViewDevices.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "MAC";
            this.columnHeader1.Width = 118;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 68;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "IP";
            this.columnHeader3.Width = 116;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Role";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Type";
            this.columnHeader5.Width = 78;
            // 
            // comboBoxNICs
            // 
            this.comboBoxNICs.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboBoxNICs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNICs.FormattingEnabled = true;
            this.comboBoxNICs.Location = new System.Drawing.Point(3, 41);
            this.comboBoxNICs.Name = "comboBoxNICs";
            this.comboBoxNICs.Size = new System.Drawing.Size(382, 21);
            this.comboBoxNICs.TabIndex = 1;
            this.comboBoxNICs.SelectedIndexChanged += new System.EventHandler(this.comboBoxNICs_SelectedIndexChanged);
            // 
            // toolStripOnline
            // 
            this.toolStripOnline.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripOnline.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonRefresh,
            this.toolStripButtonAdd,
            this.toolStripButtonAddAll});
            this.toolStripOnline.Location = new System.Drawing.Point(3, 16);
            this.toolStripOnline.Name = "toolStripOnline";
            this.toolStripOnline.Size = new System.Drawing.Size(382, 25);
            this.toolStripOnline.TabIndex = 1;
            this.toolStripOnline.Text = "toolStrip2";
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRefresh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRefresh.Image")));
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRefresh.Text = "Refresh Available Device on selected Network Interface";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // toolStripButtonAdd
            // 
            this.toolStripButtonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAdd.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAdd.Image")));
            this.toolStripButtonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAdd.Name = "toolStripButtonAdd";
            this.toolStripButtonAdd.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAdd.Text = "Add Selected Device to the Configuration File";
            this.toolStripButtonAdd.Click += new System.EventHandler(this.toolStripButtonAdd_Click);
            // 
            // toolStripButtonAddAll
            // 
            this.toolStripButtonAddAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddAll.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAddAll.Image")));
            this.toolStripButtonAddAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddAll.Name = "toolStripButtonAddAll";
            this.toolStripButtonAddAll.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddAll.Text = "Add all available devices to the Configuration File";
            this.toolStripButtonAddAll.Click += new System.EventHandler(this.toolStripButtonAddAll_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listViewFile);
            this.groupBox2.Controls.Add(this.toolStripFile);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(550, 395);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Configuration File";
            // 
            // toolStripFile
            // 
            this.toolStripFile.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSave,
            this.toolStripButtonLoad,
            this.toolStripSeparator1,
            this.toolStripButtonRemove});
            this.toolStripFile.Location = new System.Drawing.Point(3, 16);
            this.toolStripFile.Name = "toolStripFile";
            this.toolStripFile.Size = new System.Drawing.Size(544, 25);
            this.toolStripFile.TabIndex = 0;
            this.toolStripFile.Text = "Save current Configuration";
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSave.Image")));
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSave.Text = "toolStripButton1";
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // toolStripButtonLoad
            // 
            this.toolStripButtonLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonLoad.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonLoad.Image")));
            this.toolStripButtonLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLoad.Name = "toolStripButtonLoad";
            this.toolStripButtonLoad.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonLoad.Text = "Load from File";
            this.toolStripButtonLoad.ToolTipText = "Load Confguration from File";
            this.toolStripButtonLoad.Click += new System.EventHandler(this.toolStripButtonLoad_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonRemove
            // 
            this.toolStripButtonRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRemove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRemove.Image")));
            this.toolStripButtonRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemove.Name = "toolStripButtonRemove";
            this.toolStripButtonRemove.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRemove.Text = "toolStripButton3";
            this.toolStripButtonRemove.ToolTipText = "Remove selected device from configuration";
            this.toolStripButtonRemove.Click += new System.EventHandler(this.toolStripButtonRemove_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 25);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 10);
            this.label1.Size = new System.Drawing.Size(807, 51);
            this.label1.TabIndex = 1;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // toolStripMain
            // 
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonMonitorMode,
            this.toolStripSeparator2,
            this.toolStripButtonLogFile,
            this.toolStripButtonInfo});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Size = new System.Drawing.Size(942, 25);
            this.toolStripMain.TabIndex = 2;
            this.toolStripMain.Text = "toolStrip3";
            // 
            // toolStripButtonMonitorMode
            // 
            this.toolStripButtonMonitorMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMonitorMode.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMonitorMode.Image")));
            this.toolStripButtonMonitorMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMonitorMode.Name = "toolStripButtonMonitorMode";
            this.toolStripButtonMonitorMode.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonMonitorMode.Text = "Start Monitoring of configured Devices";
            this.toolStripButtonMonitorMode.Click += new System.EventHandler(this.toolStripButtonMonitorMode_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonLogFile
            // 
            this.toolStripButtonLogFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonLogFile.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonLogFile.Image")));
            this.toolStripButtonLogFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLogFile.Name = "toolStripButtonLogFile";
            this.toolStripButtonLogFile.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonLogFile.Text = "View Log File";
            this.toolStripButtonLogFile.Click += new System.EventHandler(this.toolStripButtonLogFile_Click);
            // 
            // toolStripButtonInfo
            // 
            this.toolStripButtonInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonInfo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonInfo.Image")));
            this.toolStripButtonInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonInfo.Name = "toolStripButtonInfo";
            this.toolStripButtonInfo.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonInfo.Text = "View Application Information";
            this.toolStripButtonInfo.Click += new System.EventHandler(this.toolStripButtonInfo_Click);
            // 
            // listViewFile
            // 
            this.listViewFile.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11});
            this.listViewFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewFile.HideSelection = false;
            this.listViewFile.LabelEdit = true;
            this.listViewFile.Location = new System.Drawing.Point(3, 41);
            this.listViewFile.Name = "listViewFile";
            this.listViewFile.Size = new System.Drawing.Size(544, 351);
            this.listViewFile.TabIndex = 3;
            this.listViewFile.UseCompatibleStateImageBehavior = false;
            this.listViewFile.View = System.Windows.Forms.View.Details;
            this.listViewFile.AfterSubItemLabelEdit += new System.EventHandler<ProfinetMonitor.SubItemLabelEditEventArgs>(this.listViewFile_AfterSubItemLabelEdit);
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "MAC";
            this.columnHeader6.Width = 118;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Name";
            this.columnHeader7.Width = 68;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "IP";
            this.columnHeader8.Width = 116;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Role";
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Type";
            this.columnHeader10.Width = 78;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "NIC";
            // 
            // DevicesConfigurationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(942, 471);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.toolStripMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DevicesConfigurationDialog";
            this.Text = "Profinet Monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DevicesConfigurationDialog_FormClosing);
            this.Load += new System.EventHandler(this.DevicesConfigurationDialog_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStripOnline.ResumeLayout(false);
            this.toolStripOnline.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.toolStripFile.ResumeLayout(false);
            this.toolStripFile.PerformLayout();
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStrip toolStripOnline;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdd;
        private System.Windows.Forms.ToolStrip toolStripFile;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddAll;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemove;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.ToolStripButton toolStripButtonLoad;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ComboBox comboBoxNICs;
        private System.Windows.Forms.ListView listViewDevices;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private ProfinetMonitor.EditableListView listViewFile;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton toolStripButtonMonitorMode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonLogFile;
        private System.Windows.Forms.ToolStripButton toolStripButtonInfo;
    }
}

