using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using ProfinetTools.Interfaces.Extensions;
using ProfinetTools.Interfaces.Models;
using ProfinetTools.Interfaces.Services;
using ProfinetTools.Logic.Protocols;
using ProfinetTools.Logic.Transport;
using SharpPcap;

namespace ProfinetTools.Logic.Services
{
	public class SettingsService : ISettingsService
	{
		private readonly int timeoutInMilliseconds = 3000;
		private readonly int retries = 2;

		public bool TryParseNetworkConfiguration(Device device)
		{
			try
			{
				System.Net.IPAddress ip = System.Net.IPAddress.Parse(device.IP);
				System.Net.IPAddress subnet = System.Net.IPAddress.Parse(device.SubnetMask);
				System.Net.IPAddress gateway = System.Net.IPAddress.Parse(device.Gateway);

				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		public Task<SaveResult> SendSettings(ICaptureDevice adapter, string deviceName, Device newSettings)
		{
			var disposables = new CompositeDisposable();
			var transport = new ProfinetEthernetTransport(adapter);
			transport.Open();
			transport.AddDisposableTo(disposables);

			System.Net.NetworkInformation.PhysicalAddress deviceAddress = System.Net.NetworkInformation.PhysicalAddress.Parse(deviceName);

			try
			{
				DCP.BlockErrors err = transport.SendSetNameRequest(deviceAddress, timeoutInMilliseconds, retries, newSettings.Name);
				if (err != DCP.BlockErrors.NoError) return Task.FromResult(new SaveResult(false, err.ToString()));

				System.Net.IPAddress ip = System.Net.IPAddress.Parse(newSettings.IP);
				System.Net.IPAddress subnet = System.Net.IPAddress.Parse(newSettings.SubnetMask);
				System.Net.IPAddress gateway = System.Net.IPAddress.Parse(newSettings.Gateway);
				err = transport.SendSetIpRequest(deviceAddress, timeoutInMilliseconds, retries, ip, subnet, gateway);
				if (err != DCP.BlockErrors.NoError) return Task.FromResult(new SaveResult(false, err.ToString()));

				return Task.FromResult(new SaveResult(true, err.ToString()));
			}
			catch (Exception e)
			{
				return Task.FromResult(new SaveResult(false, e.Message));
			}
			finally
			{
				disposables.Dispose();
			}
		}

		public Task<SaveResult> FactoryReset(ICaptureDevice adapter, string deviceName)
		{
			var disposables = new CompositeDisposable();
			var transport = new ProfinetEthernetTransport(adapter);
			transport.Open();
			transport.AddDisposableTo(disposables);

			System.Net.NetworkInformation.PhysicalAddress deviceAddress = System.Net.NetworkInformation.PhysicalAddress.Parse(deviceName);

			try
			{
				DCP.BlockErrors err = transport.SendSetResetRequest(deviceAddress, timeoutInMilliseconds, retries);
				if (err != DCP.BlockErrors.NoError) return Task.FromResult(new SaveResult(false, err.ToString()));

				return Task.FromResult(new SaveResult(false, err.ToString()));
			}
			catch (Exception e)
			{
				return Task.FromResult(new SaveResult(false, e.Message));
			}
			finally
			{
				disposables.Dispose(); 
			}
		}
	}
}