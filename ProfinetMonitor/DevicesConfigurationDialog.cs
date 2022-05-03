using SharpPcap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using ProfinetTools.Interfaces.Models;

namespace ProfinetMonitor
{
    public partial class DevicesConfigurationDialog : Form
    {
        private DeviceConfigurationFile CurrentFile;

        public DevicesConfigurationDialog()
        {
            InitializeComponent();
        }

        private void DevicesConfigurationDialog_Load(object sender, EventArgs e)
        {
            comboBoxNICs.Items.Clear();
            var AdapterServ = new ProfinetTools.Logic.Services.AdaptersService();
            foreach (var nic in AdapterServ.GetAdapters())
            {
                comboBoxNICs.Items.Add(new NIC(nic));
            }

            LoadFile();
        }

        private void DevicesConfigurationDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (CurrentFile.IsDirty)
                {
                    switch (MessageBox.Show(this, "File not saved, do you want to save now?", "Warning", MessageBoxButtons.YesNoCancel))
                    {
                        case DialogResult.Cancel:
                            e.Cancel = true;
                            break;
                        case DialogResult.Yes:
                            CurrentFile.Save();
                            break;
                        case DialogResult.No:
                            break;
                    }

                }
            }
        }

        private void LoadFile()
        {
            try
            {
                CurrentFile = new DeviceConfigurationFile();
                CurrentFile.Load();
            }
            catch { }
            finally
            {
                RefreshFileview();
            }
        }

        private void RefreshFileview()
        {
            this.listViewFile.Items.Clear();
            foreach (var device in CurrentFile.Devices)
            {
                var item = this.listViewFile.Items.Add(device.Device.MAC);
                item.Tag = device;
                item.SubItems.Add(device.Device.Name);
                item.SubItems.Add(device.Device.IP);
                item.SubItems.Add(device.Device.Role);
                item.SubItems.Add(device.Device.Type);
                item.SubItems.Add(device.NetworkAdapterName);
            }
        }

        #region OnlineView
        /// <summary>
        /// A simple wrapper for the Combobox, that overries "ToString"
        /// </summary>
        private class NIC
        {
            public ICaptureDevice Device;

            public NIC(ICaptureDevice Device)
            {
                this.Device = Device;
            }

            public override string ToString()
            {
                return Device.Description;
            }

        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            RefreshOnlineDevices();
        }

        private void RefreshOnlineDevices()
        {
            if (comboBoxNICs.SelectedItem == null) return;
            this.listViewDevices.Items.Clear();
            this.toolStripButtonRefresh.Enabled = false;
            NIC NIC = (NIC)comboBoxNICs.SelectedItem;

            var DeviceService = new ProfinetTools.Logic.Services.DeviceService();
            var task = DeviceService.GetDevices(NIC.Device, TimeSpan.FromSeconds(3));
            task.ContinueWith(new Action<Task<List<Device>>>(
                (taskResult) =>
                {
                    foreach (var device in taskResult.Result)
                    {
                        var item = this.listViewDevices.Items.Add(device.MAC);
                        item.Tag = device;
                        item.SubItems.Add(device.Name);
                        item.SubItems.Add(device.IP);
                        item.SubItems.Add(device.Role);
                        item.SubItems.Add(device.Type);
                    }
                    this.toolStripButtonRefresh.Enabled = true;
                }), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void comboBoxNICs_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.listViewDevices.Items.Clear();
        }

        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {

            if (this.listViewDevices.Items.Count == 0) return;
            var onlineItem = this.listViewDevices.Items[0];
            var fileEntry = new DeviceConfigurationFileEntry();
            fileEntry.Device = (Device)onlineItem.Tag;
            fileEntry.NetworkAdapterName = comboBoxNICs.SelectedItem.ToString();
            CurrentFile.AddOrUpdateDevice(fileEntry);
            RefreshFileview();
        }
        #endregion

        #region FileView
        private void toolStripButtonAddAll_Click(object sender, EventArgs e)
        {
            if (comboBoxNICs.SelectedItem == null) return;
            foreach (ListViewItem onlineItem in this.listViewDevices.Items)
            {
                var fileEntry = new DeviceConfigurationFileEntry();
                fileEntry.Device = (Device)onlineItem.Tag;
                fileEntry.NetworkAdapterName = comboBoxNICs.SelectedItem.ToString();
                CurrentFile.AddOrUpdateDevice(fileEntry);
            }
            RefreshFileview();
        }

        private void toolStripButtonLoad_Click(object sender, EventArgs e)
        {
            LoadFile();
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            CurrentFile.Save();
        }

        private void toolStripButtonRemove_Click(object sender, EventArgs e)
        {
            if (this.listViewFile.SelectedItems.Count == 0) return;
            foreach (ListViewItem item in this.listViewFile.SelectedItems)
            {
                CurrentFile.Devices.Remove((DeviceConfigurationFileEntry)item.Tag);
                item.Remove();
            }
        }

        private void listViewFile_AfterSubItemLabelEdit(object sender, SubItemLabelEditEventArgs e)
        {
            var LI = this.listViewFile.Items[e.Item];
            var LSI = LI.SubItems[e.SubItem];
            var device = (DeviceConfigurationFileEntry)LI.Tag;

            switch (e.SubItem)
            {
                case 0:
                    device.Device.MAC = e.Label;
                    break;
                case 1:
                    device.Device.Name = e.Label;
                    break;
                case 2:
                    device.Device.IP = e.Label;
                    break;
                case 3:
                    device.Device.Role = e.Label;
                    break;
                case 4:
                    device.Device.Type = e.Label;
                    break;
                case 5:
                    device.NetworkAdapterName = e.Label;
                    break;
            }

        }
        #endregion

        private void toolStripButtonMonitorMode_Click(object sender, EventArgs e)
        {
            var form = new MonitorModeDialog(10000);
            form.Show();
            form.FormClosed += (a, b) => { this.Close(); };
            this.Hide();
        }

        private void toolStripButtonLogFile_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Logging.LogManager.LogFileName);
            }
            catch { }
        }

        private void toolStripButtonInfo_Click(object sender, EventArgs e)
        {
            var form = new AboutDialog();
            form.ShowDialog();
        }
    }
}
