using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProfinetTools.Interfaces.Extensions;
using ProfinetTools.Interfaces.Models;
using ProfinetTools.Logic.Protocols;
using ProfinetTools.Logic.Transport;
using SharpPcap;

namespace ProfinetTools.Logic.Services
{
	public class DeviceService
	{
		public async Task<List<Device>> GetDevices(ICaptureDevice adapter, TimeSpan timeout)
		{
			var disposables = new CompositeDisposable();
			var transport = new ProfinetEthernetTransport(adapter);
			transport.Open();
			transport.AddDisposableTo(disposables);

			var devices = new List<Device>();

			Observable.FromEventPattern<ProfinetEthernetTransport.OnDcpMessageHandler, ConnectionInfoEthernet, DcpMessageArgs>(h => transport.OnDcpMessage += h, h => transport.OnDcpMessage -= h)
				.Select(x => ConvertEventToDevice(x.Sender, x.EventArgs))
				.Do(device => devices.Add(device))
				.Subscribe()
				.AddDisposableTo(disposables)
				;

			transport.SendIdentifyBroadcast();

			await Task.Delay(timeout);

			disposables.Dispose();

			return devices;
		}

		private Device ConvertEventToDevice(ConnectionInfoEthernet sender, DcpMessageArgs args)
		{
			var device = new Device()
			{
				MAC = sender.Source.ToString(),
				Name = (string)args.Blocks[DCP.BlockOptions.DeviceProperties_NameOfStation],
				IP = ((DCP.IpInfo)args.Blocks[DCP.BlockOptions.IP_IPParameter]).Ip.ToString(),
				Type = (string)args.Blocks[DCP.BlockOptions.DeviceProperties_DeviceVendor],
				Role = ((DCP.DeviceRoleInfo)args.Blocks[DCP.BlockOptions.DeviceProperties_DeviceRole]).ToString()
			};
			return device;
		}
	}
}