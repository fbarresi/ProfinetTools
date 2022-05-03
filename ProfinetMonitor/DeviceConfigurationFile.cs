using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProfinetMonitor;
using ProfinetTools.Interfaces.Models;

namespace ProfinetMonitor
{
    public class DeviceConfigurationFile
    {
        private string OldFilecontent = "";

        private string CurrentFileContent()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Name;MAC;IP;NetMask;GateWay;Role;Type;NetworkAdapterName");
            foreach (var dev in Devices)
            {
                sb.Append(dev.Device.Name).Append(";");
                sb.Append(dev.Device.MAC).Append(";");
                sb.Append(dev.Device.IP).Append(";");
                sb.Append(dev.Device.SubnetMask).Append(";");
                sb.Append(dev.Device.Gateway).Append(";");
                sb.Append(dev.Device.Role).Append(";");
                sb.Append(dev.Device.Type).Append(";");
                sb.Append(dev.NetworkAdapterName);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public bool IsDirty
        {
            get
            {
                return OldFilecontent != CurrentFileContent();
            }
        }

        public List<DeviceConfigurationFileEntry> Devices { get; set; } = new List<DeviceConfigurationFileEntry>();

        public System.IO.FileInfo GetDefaultFileName()
        {
            string fName = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Devices.config.csv");
            return new System.IO.FileInfo(fName);
        }

        public void AddOrUpdateDevice(DeviceConfigurationFileEntry device)
        {
            DeviceConfigurationFileEntry fndDevice = null;
            foreach (var de in Devices)
            {
                if (de.Device.MAC == device.Device.MAC) fndDevice = de;
            }

            if (fndDevice != null) Devices.Remove(fndDevice);
            Devices.Add(device);
        }

        public DeviceConfigurationFile()
        {
            OldFilecontent = CurrentFileContent();
        }

        public void Save()
        {
            Save(GetDefaultFileName());
        }

        public void Load()
        {
            Load(GetDefaultFileName());
        }

        public void Save(System.IO.FileInfo fileName)
        {
            string FileContent = CurrentFileContent();
            System.IO.File.WriteAllText(fileName.FullName, FileContent);
            OldFilecontent = FileContent;
        }

        public void Load(System.IO.FileInfo fileName)
        {
            Devices.Clear();
            var FileContent = System.IO.File.ReadAllText(fileName.FullName);
            OldFilecontent = FileContent;
            var Lines = FileContent.Split(Environment.NewLine[0]);
            for (var i = 1; i<Lines.Length; i++) //start at second line, because first one i the header
            {
                var Values = Lines[i].Trim().Split(';');
                if (Values.Length < 8) continue;
                var Device = new Device();
                Device.Name = Values[0];
                Device.MAC = Values[1];
                Device.IP = Values[2];
                Device.SubnetMask = Values[3];
                Device.Gateway = Values[4];
                Device.Role = Values[5];
                Device.Type = Values[6];
                var nic = Values[7];

                var DevEntry = new DeviceConfigurationFileEntry();
                DevEntry.Device = Device;
                DevEntry.NetworkAdapterName = nic;
                Devices.Add(DevEntry);
            }
        }
    }
}
