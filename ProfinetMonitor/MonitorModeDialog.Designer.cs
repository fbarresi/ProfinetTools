
namespace ProfinetMonitor
{
    partial class MonitorModeDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitorModeDialog));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.stopMonitoringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipText = "Monitoring Devices";
            this.notifyIcon1.BalloonTipTitle = "Monitoring Devices";
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Profinet Monitor";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stopMonitoringToolStripMenuItem,
            this.showLogFileToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(162, 48);
            // 
            // stopMonitoringToolStripMenuItem
            // 
            this.stopMonitoringToolStripMenuItem.Name = "stopMonitoringToolStripMenuItem";
            this.stopMonitoringToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.stopMonitoringToolStripMenuItem.Text = "Stop Monitoring";
            this.stopMonitoringToolStripMenuItem.Click += new System.EventHandler(this.stopMonitoringToolStripMenuItem_Click);
            // 
            // showLogFileToolStripMenuItem
            // 
            this.showLogFileToolStripMenuItem.Name = "showLogFileToolStripMenuItem";
            this.showLogFileToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.showLogFileToolStripMenuItem.Text = "Show Log File";
            this.showLogFileToolStripMenuItem.Click += new System.EventHandler(this.showLogFileToolStripMenuItem_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // MonitorModeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(241, 129);
            this.Name = "MonitorModeDialog";
            this.Text = "MonitorModeDialog";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MonitorModeDialog_FormClosed);
            this.Load += new System.EventHandler(this.MonitorModeDialog_Load);
            this.Shown += new System.EventHandler(this.MonitorModeDialog_Shown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem stopMonitoringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showLogFileToolStripMenuItem;
    }
}