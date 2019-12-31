using System;
using System.Linq;
using System.Threading.Tasks;
using ProfinetTools.Interfaces.Models;
using SharpPcap;

namespace ProfinetTools.Interfaces.Services
{
	public interface ISettingsService
	{
		Task<SaveResult> FactoryReset(ICaptureDevice adapter, string deviceName);
		Task<SaveResult> SendSettings(ICaptureDevice adapter, string macAddress, Device newSettings, bool permanent);

        Task<SaveResult> SendSignalRequest(ICaptureDevice adapter, string macAddress);
        bool TryParseNetworkConfiguration(Device device);
	}
}