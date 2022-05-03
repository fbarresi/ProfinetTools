using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProfinetMonitor
{
    public partial class MonitorModeDialog : Form
    {
        private int MonitoringInterval;

        public MonitorModeDialog(): this(0)
        {
        }

        public MonitorModeDialog( int monitoringInterval)
        {
            InitializeComponent();
            this.MonitoringInterval = monitoringInterval;
        }

        private void MonitorModeDialog_Load(object sender, EventArgs e)
        {

            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ProfinetDeviceMonitor.RunMonitoringMode(MonitoringInterval);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }

        private void MonitorModeDialog_Shown(object sender, EventArgs e)
        {
            this.Hide();
            this.Visible = false;
        }

        private void MonitorModeDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon1.Visible = false;
        }

        private void stopMonitoringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProfinetDeviceMonitor.StopMonitoringMode();
        }

        private void showLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Logging.LogManager.LogFileName);
            }
            catch { }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(MousePosition);
        }
    }
}
