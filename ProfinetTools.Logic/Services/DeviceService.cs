using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
			var device = new ProfinetEthernetTransport(adapter);
			device.Open();

			Observable.FromEventPattern<ProfinetEthernetTransport.OnDcpMessageHandler, ConnectionInfoEthernet, DcpMessageArgs>(h => device.OnDcpMessage += h, h => device.OnDcpMessage -= h)
				//.Select(x=> x.)
				;

			device.SendIdentifyBroadcast();

			await Task.Delay(timeout);

			device.Dispose();

			return null;
		}

		private void m_device_OnDcpMessage(ConnectionInfoEthernet sender, DCP.ServiceIds service_id, uint xid, ushort response_delay_factor, Dictionary<DCP.BlockOptions, object> blocks)
		{
			if (service_id == DCP.ServiceIds.Identify_Response)
			{
				string mac = sender.Source.ToString();
				string name = (string) blocks[DCP.BlockOptions.DeviceProperties_NameOfStation];
				string ip = ((DCP.IpInfo) blocks[DCP.BlockOptions.IP_IPParameter]).Ip.ToString();
				string type = (string) blocks[DCP.BlockOptions.DeviceProperties_DeviceVendor];
				string role = ((DCP.DeviceRoleInfo) blocks[DCP.BlockOptions.DeviceProperties_DeviceRole]).ToString();
				//if (m_device_infos.ContainsKey(mac)) m_device_infos.Remove(mac);
				//m_device_infos.Add(mac, blocks);
				//this.Invoke((MethodInvoker)delegate
				//{
				//	if (DeviceList.Items.ContainsKey(mac)) DeviceList.Items.RemoveByKey(mac);
				//	ListViewItem itm = DeviceList.Items.Add(mac, (string)blocks[DCP.BlockOptions.DeviceProperties_NameOfStation], 0);
				//	itm.SubItems.Add(mac);
				//	itm.SubItems.Add(((DCP.IpInfo)blocks[DCP.BlockOptions.IP_IPParameter]).Ip.ToString());
				//	itm.SubItems.Add((string)blocks[DCP.BlockOptions.DeviceProperties_DeviceVendor]);
				//	itm.SubItems.Add(((DCP.DeviceRoleInfo)blocks[DCP.BlockOptions.DeviceProperties_DeviceRole]).ToString());
				//});
			}
		}
	}
}