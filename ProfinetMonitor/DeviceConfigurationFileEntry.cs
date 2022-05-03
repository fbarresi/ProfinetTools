using ProfinetTools.Interfaces.Models;
using System;

namespace ProfinetMonitor
{
    public class DeviceConfigurationFileEntry 
    {
        public Device Device { get; set; }
        public string NetworkAdapterName { get; set; }
    }
}