using ProfinetTools.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfinetMonitor
{
    class ProfinetDeviceMonitor
    {

        /// <summary>
        /// the currently opened Configuration File
        /// </summary>
        private static DeviceConfigurationFile ConfigFile;

        /// <summary>
        /// The Devices from the Configuration File, but Aggregated by their NIC
        /// </summary>
        private static Dictionary<string, List<Device>> NicDevices;
        private static Logging.Logger Log = new Logging.Logger("ProfinetDeviceMonitor");
        private static bool CancelationPending = false;
        private static bool RestartPending = false;
        private static System.Threading.ManualResetEvent WaitEvent = new System.Threading.ManualResetEvent(false);

        public static bool AddNewFoundDevicesAutomatically = false;

        public static void RunMonitoringMode( int MonitoringCycle )
        {
            try
            {
            Restart:
                CancelationPending = false;
                RestartPending = false;
                WaitEvent.Reset();
                missingDevices.Clear();
                LoadConfigurationFile();

                //Depending on the Cycle time, do it once or stay running
                if (MonitoringCycle == 0)
                {
                    Log.Info("Running Monitoring only once");
                    ScanAndMonitorDevices();
                }
                else
                {
                    Log.Info("Entering constant monitoring mode every {0} milliseconds", MonitoringCycle);
                    while (true)
                    {
                        ScanAndMonitorDevices();
                        WaitEvent.WaitOne(MonitoringCycle);
                        if (CancelationPending) break;
                    }
                    Log.Info("Monitoring Mode Stopped");

                    if (RestartPending)
                    {
                        Log.Info("Restarting Monitor Mode");
                        goto Restart;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public static void StopMonitoringMode()
        {
            Log.Info("Stopping Monitoring Mode");
            CancelationPending = true;
            WaitEvent.Set();
        }

        public static void RestartMonitoringMode()
        {
            CancelationPending = true;
            RestartPending = true;
            WaitEvent.Set();
        }

        /// <summary>
        /// Load Config file and Separate devices into Network interface Cards
        /// </summary>
        private static void LoadConfigurationFile()
        {
            Log.Info("Loading configuration file");

            ConfigFile = new DeviceConfigurationFile();
            ConfigFile.Load();
            NicDevices = new Dictionary<string, List<Device>>();

            foreach (var entry in ConfigFile.Devices)
            {
                if (!NicDevices.ContainsKey(entry.NetworkAdapterName)) NicDevices.Add(entry.NetworkAdapterName, new List<Device>());
                NicDevices[entry.NetworkAdapterName].Add(entry.Device);
            }
        }

        /// <summary>
        /// Do one single scan over the configured devices 
        /// </summary>
        private static void ScanAndMonitorDevices()
        {
            var Adapterservice = new ProfinetTools.Logic.Services.AdaptersService();
            var Nics = Adapterservice.GetAdapters();
            var Deviceservice = new ProfinetTools.Logic.Services.DeviceService();

            foreach (var nic in NicDevices.Keys)
            {
                try
                {
                    var ConfiguredDevices = NicDevices[nic];

                    //Find corresponding ICaptureDevice
                    var CaptureDevice = FindCaptureDeviceByDescription(nic, Nics);
                    if (CaptureDevice == null)
                    {
                        Log.Error("the Network interface '{0}' was not found", nic);
                        return;
                    }

                    //Now Scan the NIC for available devices
                    var Task = Deviceservice.GetDevices(CaptureDevice, TimeSpan.FromSeconds(10));
                    Task.Wait();
                    var OnlineDevices = Task.Result;

                    //There is a problem, that sometimes, but only sometimes, we get an "False Positive". 
                    //To mitigate that, we check the network "Two times". 
                    //System.Threading.Thread.Sleep(1000); //wait one second
                    //var Task2 = Deviceservice.GetDevices(CaptureDevice, TimeSpan.FromSeconds(10));
                    //Task2.Wait();
                    //OnlineDevices.AddRange(Task2.Result); //this creates duplicates, but that does not really matter

                    //Now check if we have an Device missing
                    //go through all configure devices.....
                    foreach (var ConfigDevice in ConfiguredDevices)
                    {
                        try
                        {
                            //...Check if they are currently available...
                            var OnlineDevice = FindDeviceByMAC(ConfigDevice.MAC, OnlineDevices);
                            if (OnlineDevice == null) //... and if not, then we have an error
                            {
                                if (missingDevices.Contains(ConfigDevice.Name)) continue; //Already Logged
                                missingDevices.Add(ConfigDevice.Name);
                                Log.Error("Device '{0}' is not reachable via its configured NIC", ConfigDevice.Name);
                                continue;
                            }
                            else //... if the Device was available, but it was missing before, then it is reachable again
                            {
                                if (missingDevices.Contains(ConfigDevice.Name))
                                {
                                    missingDevices.Remove(ConfigDevice.Name);
                                    Log.Info("Device '{0}' is reachable again via its configured NIC", ConfigDevice.Name);
                                }
                            }

                            //Check if it has an Profinet name
                            if (string.IsNullOrWhiteSpace(OnlineDevice.Name))
                            {
                                Log.Error("Device '{0}' has lost its online Profinet name", ConfigDevice.Name);
                                AssignConfiguredSettingsToDevice(ConfigDevice, CaptureDevice);
                            }
                            else if (string.IsNullOrWhiteSpace(OnlineDevice.IP))
                            {
                                Log.Error("Device '{0}' has lost its IP-Address", ConfigDevice.Name);
                                AssignConfiguredSettingsToDevice(ConfigDevice, CaptureDevice);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Device '{0}' throw an exception: {1}", ConfigDevice.Name, ex);
                        }
                    }

                    //Add devices that have been found on the bus
                    if (AddNewFoundDevicesAutomatically)
                    {
                        foreach (var OnlineDevice in ConfiguredDevices)
                        {
                            try
                            {
                                if (OnlineDevice.MAC == null) continue;

                                var ConfigDevice = FindDeviceByMAC(OnlineDevice.MAC, ConfiguredDevices);
                                if (ConfigDevice == null)
                                {
                                    Log.Info("Device '{0}' is an new Device found on the Network, which is not yet configured", OnlineDevice.MAC);

                                    //Check if it has an IP address, sub-net-mask and name
                                    if (OnlineDevice.IP == null)
                                    {
                                        Log.Debug("the device {0} did not have an IP address yet. Ignoring", OnlineDevice.MAC);
                                        continue;
                                    }
                                    //Check if it has an IP address, sub-net-mask and name
                                    if (OnlineDevice.SubnetMask == null)
                                    {
                                        Log.Debug("the device {0} did not have an SubnetMask yet. Ignoring", OnlineDevice.MAC);
                                        continue;
                                    }
                                    //Check if it has an IP address, sub-net-mask and name
                                    if (OnlineDevice.Name == null)
                                    {
                                        Log.Debug("the device {0} did not have an Name yet. Ignoring", OnlineDevice.MAC);
                                        continue;
                                    }

                                    Log.Info("Adding device {0};{1};{2}", OnlineDevice.MAC, OnlineDevice.Name, OnlineDevice.IP);
                                    var dev = new DeviceConfigurationFileEntry();
                                    dev.Device = OnlineDevice;
                                    dev.NetworkAdapterName = nic;
                                    ConfigFile.AddOrUpdateDevice(dev);

                                    Log.Info("Saving config file");
                                    ConfigFile.Save();
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Device '{0}' throw an exception: {1}", OnlineDevice.MAC, ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("NIC '{0}' throw an exception: {1}", nic, ex);
                }
            }
        }

        private static List<string> missingDevices = new List<string>();

        private static SharpPcap.ICaptureDevice FindCaptureDeviceByDescription(string Description, List<SharpPcap.ICaptureDevice> Adapters)
        {
            foreach (var adapter in Adapters)
            {
                if (adapter.Description == Description) return adapter;
            }
            return null;
        }

        private static Device FindDeviceByMAC(string MAC, List<Device> devices)
        {
            if (devices == null) return null;
            if (MAC == null) return null;

            foreach (var dev in devices)
            {
                if (dev.MAC == MAC) return dev;
            }
            return null;
        }

        private static void AssignConfiguredSettingsToDevice(Device device, SharpPcap.ICaptureDevice CaptureDevice)
        {
            Log.Info("Assigning device name and IP-address to device {0}", device.Name);
            var Settingsservice = new ProfinetTools.Logic.Services.SettingsService();
            Settingsservice.SendSettings(CaptureDevice, device.MAC, device);

        }
    }
}
