using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using ProfinetTools.Logic.Protocols;
using SharpPcap;

namespace ProfinetTools.Logic.Transport
{
	public class ProfinetEthernetTransport : IDisposable
	{
		private ICaptureDevice adapter;
		private UInt16 lastXid = 0;
		private bool isOpen = false;

		public delegate void OnDcpMessageHandler(ConnectionInfoEthernet sender, DCP.ServiceIds serviceID, uint xid, ushort responseDelayFactor, Dictionary<DCP.BlockOptions, object> blocks);
		public event OnDcpMessageHandler OnDcpMessage;
		public delegate void OnAcyclicMessageHandler(ConnectionInfoEthernet sender, UInt16 alarmDestinationEndpoint, UInt16 alarmSourceEndpoint, RT.PDUTypes pduType, RT.AddFlags addFlags, UInt16 sendSeqNum, UInt16 ackSeqNum, UInt16 varPartLen, Stream data);
		public event OnAcyclicMessageHandler OnAcyclicMessage;
		public delegate void OnCyclicMessageHandler(ConnectionInfoEthernet sender, UInt16 cycleCounter, RT.DataStatus dataStatus, RT.TransferStatus transferStatus, Stream data, int dataLength);
		public event OnCyclicMessageHandler OnCyclicMessage;

		public bool IsOpen => isOpen;
		public ICaptureDevice Adapter => adapter;

		public ProfinetEthernetTransport(ICaptureDevice adapter)
		{
			this.adapter = adapter;
			this.adapter.OnPacketArrival += new PacketArrivalEventHandler(m_adapter_OnPacketArrival);
		}

		/// <summary>
		/// Will return pcap version. Use this to validate installed pcap library
		/// </summary>
		public static string PcapVersion
		{
			get
			{
				try
				{
					return SharpPcap.Pcap.Version;
				}
				catch { }
				return "";
			}
		}

		public void Open()
		{
			if (isOpen) return;
			if (adapter is SharpPcap.WinPcap.WinPcapDevice)
				((SharpPcap.WinPcap.WinPcapDevice)adapter).Open(SharpPcap.WinPcap.OpenFlags.MaxResponsiveness | SharpPcap.WinPcap.OpenFlags.NoCaptureLocal, -1);
			else
				adapter.Open(DeviceMode.Normal);
			adapter.Filter = "ether proto 0x8892 or vlan 0";
			adapter.StartCapture();
			isOpen = true;
			System.Threading.Thread.Sleep(50);  //let the pcap start up
		}

		public void Close()
		{
			if (!isOpen) return;
			try
			{
				adapter.StopCapture();
			}
			catch
			{
			}
			isOpen = false;
		}

		public void Dispose()
		{
			if (adapter != null)
			{
				Close();
				adapter.Close();
				adapter = null;
			}
		}

		public static DCP.IpInfo GetPcapIp(SharpPcap.ICaptureDevice pcapDevice)
		{
			foreach (System.Net.NetworkInformation.NetworkInterface nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
			{
				PhysicalAddress mac = null;
				try
				{
					mac = nic.GetPhysicalAddress();
				}
				catch (Exception)
				{
					//interface have no mac address
				}

				if (mac != null && mac.Equals(pcapDevice.MacAddress))
				{
					IPInterfaceProperties ipp = nic.GetIPProperties();
					foreach (var entry in ipp.UnicastAddresses)
					{
						if (entry.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
						{
							byte[] gw = new byte[] { 0, 0, 0, 0 };
							if (ipp.GatewayAddresses.Count > 0) gw = ipp.GatewayAddresses[0].Address.GetAddressBytes();
							return new DCP.IpInfo(DCP.BlockInfo.IpSet, entry.Address.GetAddressBytes(), entry.IPv4Mask.GetAddressBytes(), gw);
						}
					}
				}
			}
			return null;
		}

		public static PhysicalAddress GetDeviceMac(string interfaceIp)
		{
			IPAddress searchIp = IPAddress.Parse(interfaceIp);
			foreach (System.Net.NetworkInformation.NetworkInterface nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
			{
				PhysicalAddress mac = null;
				try
				{
					mac = nic.GetPhysicalAddress();
				}
				catch (Exception)
				{
					//interface have no mac address
					continue;
				}
				foreach (var entry in nic.GetIPProperties().UnicastAddresses)
				{
					if (searchIp.Equals(entry.Address))
					{
						return mac;
					}
				}
			}
			return null;
		}

		public static SharpPcap.ICaptureDevice GetPcapDevice(string localIp)
		{
			IPAddress searchIp = IPAddress.Parse(localIp);
			PhysicalAddress searchMac = null;
			Dictionary<PhysicalAddress, SharpPcap.ICaptureDevice> networks = new Dictionary<PhysicalAddress, SharpPcap.ICaptureDevice>();

			//search all networks
			foreach (System.Net.NetworkInformation.NetworkInterface nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
			{
				PhysicalAddress mac = null;
				try
				{
					mac = nic.GetPhysicalAddress();
				}
				catch (Exception)
				{
					//interface have no mac address
					continue;
				}
				foreach (var entry in nic.GetIPProperties().UnicastAddresses)
				{
					if (searchIp.Equals(entry.Address))
					{
						searchMac = mac;
						break;
					}
				}
				if (searchMac != null) break;
			}

			//validate
			if (searchMac == null) return null;

			//search all pcap networks
			foreach (SharpPcap.ICaptureDevice dev in SharpPcap.CaptureDeviceList.Instance)
			{
				try
				{
					dev.Open();
					networks.Add(dev.MacAddress, dev);
					dev.Close();
				}
				catch { }
			}

			//find link
			if (networks.ContainsKey(searchMac)) return networks[searchMac];
			else return null;
		}

		public static System.Net.IPAddress GetNetworkAddress(System.Net.IPAddress address, System.Net.IPAddress subnetMask)
		{
			byte[] ipAdressBytes = address.GetAddressBytes();
			byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

			if (ipAdressBytes.Length != subnetMaskBytes.Length)
				throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

			byte[] broadcastAddress = new byte[ipAdressBytes.Length];
			for (int i = 0; i < broadcastAddress.Length; i++)
				broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
			return new System.Net.IPAddress(broadcastAddress);
		}

		public static string GetLocalIpAddress(string ipMatch)
		{
			System.Net.IPAddress target = System.Net.IPAddress.Parse(ipMatch);
			foreach (System.Net.NetworkInformation.NetworkInterface nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
			{
				foreach (var entry in nic.GetIPProperties().UnicastAddresses)
				{
					if (entry.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
					{
						if (GetNetworkAddress(entry.Address, entry.IPv4Mask).Equals(GetNetworkAddress(target, entry.IPv4Mask)))
							return entry.Address.ToString();
					}
				}
			}
			return "";
		}

		private void m_adapter_OnProfinetArrival(ConnectionInfoEthernet sender, Stream stream)
		{
			RT.FrameIds frameID;

			//Real Time
			RT.DecodeFrameId(stream, out frameID);
			if (frameID == RT.FrameIds.DCP_Identify_ResPDU || frameID == RT.FrameIds.DCP_Identify_ReqPDU || frameID == RT.FrameIds.DCP_Get_Set_PDU || frameID == RT.FrameIds.DCP_Hello_ReqPDU)
			{
				DCP.ServiceIds serviceID;
				uint xid;
				ushort responseDelayFactor;
				ushort dcpDataLength;
				DCP.DecodeHeader(stream, out serviceID, out xid, out responseDelayFactor, out dcpDataLength);
				Dictionary<DCP.BlockOptions, object> blocks;
				DCP.DecodeAllBlocks(stream, dcpDataLength, out blocks);
				if (OnDcpMessage != null) OnDcpMessage(sender, serviceID, xid, responseDelayFactor, blocks);
			}
			else if (frameID == RT.FrameIds.PTCP_DelayReqPDU)
			{
				//ignore this for now
			}
			else if (frameID >= RT.FrameIds.RTC_Start && frameID <= RT.FrameIds.RTC_End)
			{
				long dataPos = stream.Position;
				stream.Position = stream.Length - 4;
				UInt16 cycleCounter;
				RT.DataStatus dataStatus;
				RT.TransferStatus transferStatus;
				RT.DecodeRTCStatus(stream, out cycleCounter, out dataStatus, out transferStatus);
				stream.Position = dataPos;
				if (OnCyclicMessage != null) OnCyclicMessage(sender, cycleCounter, dataStatus, transferStatus, stream, (int)(stream.Length - dataPos - 4));
			}
			else if (frameID == RT.FrameIds.Alarm_Low || frameID == RT.FrameIds.Alarm_High)
			{
				UInt16 alarmDestinationEndpoint;
				UInt16 alarmSourceEndpoint;
				RT.PDUTypes pduType;
				RT.AddFlags addFlags;
				UInt16 sendSeqNum;
				UInt16 ackSeqNum;
				UInt16 varPartLen;
				RT.DecodeRTAHeader(stream, out alarmDestinationEndpoint, out alarmSourceEndpoint, out pduType, out addFlags, out sendSeqNum, out ackSeqNum, out varPartLen);
				if (OnAcyclicMessage != null) OnAcyclicMessage(sender, alarmDestinationEndpoint, alarmSourceEndpoint, pduType, addFlags, sendSeqNum, ackSeqNum, varPartLen, stream);
			}
			else
			{
				Trace.TraceWarning("Unclassified RT message");
			}
		}

		private void m_adapter_OnPacketArrival(object sender, CaptureEventArgs e)
		{
			if (e.Packet.LinkLayerType != PacketDotNet.LinkLayers.Ethernet) return;
			PacketDotNet.Utils.ByteArraySegment bas = new PacketDotNet.Utils.ByteArraySegment(e.Packet.Data);
			PacketDotNet.EthernetPacket ethP = new PacketDotNet.EthernetPacket(bas);
			if (ethP.Type != (PacketDotNet.EthernetPacketType)0x8892 && ethP.Type != PacketDotNet.EthernetPacketType.VLanTaggedFrame) return;
			if (ethP.PayloadPacket != null && ethP.PayloadPacket is PacketDotNet.Ieee8021QPacket)
			{
				if (((PacketDotNet.Ieee8021QPacket)ethP.PayloadPacket).Type != (PacketDotNet.EthernetPacketType)0x8892) return;
				if (((PacketDotNet.Ieee8021QPacket)ethP.PayloadPacket).PayloadData == null)
				{
					Trace.TraceWarning("Empty vlan package");
					return;
				}
				m_adapter_OnProfinetArrival(new ConnectionInfoEthernet(this, ethP.DestinationHwAddress, ethP.SourceHwAddress), new MemoryStream(((PacketDotNet.Ieee8021QPacket)ethP.PayloadPacket).PayloadData, false));
			}
			else
			{
				if (ethP.PayloadData == null)
				{
					Trace.TraceWarning("Empty ethernet package");
					return;
				}
				m_adapter_OnProfinetArrival(new ConnectionInfoEthernet(this, ethP.DestinationHwAddress, ethP.SourceHwAddress), new MemoryStream(ethP.PayloadData, false));
			}
		}

		private void Send(MemoryStream stream)
		{
			byte[] buffer = stream.GetBuffer();
			adapter.SendPacket(buffer, (int)stream.Position);
		}

		public void SendIdentifyBroadcast()
		{
			Trace.WriteLine("Sending identify broadcast", null);

			MemoryStream mem = new MemoryStream();

			//ethernet
			PhysicalAddress ethernetDestinationHwAddress = PhysicalAddress.Parse(RT.MulticastMACAdd_Identify_Address);
			Ethernet.Encode(mem, ethernetDestinationHwAddress, adapter.MacAddress, Ethernet.Type.VLanTaggedFrame);

			//VLAN
			VLAN.Encode(mem, VLAN.Priorities.Priority0, VLAN.Type.PN);

			//Profinet Real Time
			RT.EncodeFrameId(mem, RT.FrameIds.DCP_Identify_ReqPDU);

			//Profinet DCP
			DCP.EncodeIdentifyRequest(mem, ++lastXid);

			//Send
			Send(mem);
		}

		public void SendIdentifyResponse(PhysicalAddress destination, uint xid, Dictionary<DCP.BlockOptions, object> blocks)
		{
			Trace.WriteLine("Sending identify response", null);

			MemoryStream mem = new MemoryStream();

			//ethernet
			Ethernet.Encode(mem, destination, adapter.MacAddress, Ethernet.Type.VLanTaggedFrame);

			//VLAN
			VLAN.Encode(mem, VLAN.Priorities.Priority0, VLAN.Type.PN);

			//Profinet Real Time
			RT.EncodeFrameId(mem, RT.FrameIds.DCP_Identify_ResPDU);

			//Profinet DCP
			DCP.EncodeIdentifyResponse(mem, xid, blocks);

			//Send
			Send(mem);
		}

		public void SendCyclicData(PhysicalAddress destination, UInt16 frameID, UInt16 cycleCounter, byte[] userData)
		{
			Trace.WriteLine("Sending cyclic data", null);

			MemoryStream mem = new MemoryStream();

			//ethernet
			Ethernet.Encode(mem, destination, adapter.MacAddress, (Ethernet.Type)0x8892);

			//Profinet Real Time
			RT.EncodeFrameId(mem, (RT.FrameIds)frameID);

			//user data
			if (userData == null) userData = new byte[40];
			if (userData.Length < 40) Array.Resize<byte>(ref userData, 40);
			mem.Write(userData, 0, userData.Length);

			//RT footer
			RT.EncodeRTCStatus(mem, cycleCounter, RT.DataStatus.DataItemValid |
			                   RT.DataStatus.State_Primary |
			                   RT.DataStatus.ProviderState_Run |
			                   RT.DataStatus.StationProblemIndicator_Normal,
			                   RT.TransferStatus.OK);

			//Send
			Send(mem);
		}

		public class ProfinetAsyncDcpResult : IAsyncResult, IDisposable
		{
			private System.Threading.ManualResetEvent mWait = new System.Threading.ManualResetEvent(false);
			private ushort mXid;
			private ProfinetEthernetTransport mConn = null;

			public object AsyncState { get; set; }
			public System.Threading.WaitHandle AsyncWaitHandle { get { return mWait; } }
			public bool CompletedSynchronously { get; private set; }    //always false
			public bool IsCompleted { get; private set; }

			public Dictionary<DCP.BlockOptions, object> Result { get; private set; }

			public ProfinetAsyncDcpResult(ProfinetEthernetTransport conn, MemoryStream message, UInt16 xid)
			{
				mConn = conn;
				conn.OnDcpMessage += new OnDcpMessageHandler(conn_OnDcpMessage);
				mXid = xid;
				conn.Send(message);
			}

			private void conn_OnDcpMessage(ConnectionInfoEthernet sender, DCP.ServiceIds serviceID, uint xid, ushort responseDelayFactor, Dictionary<DCP.BlockOptions, object> blocks)
			{
				if (xid == mXid)
				{
					Result = blocks;
					IsCompleted = true;
					mWait.Set();
				}
			}

			public void Dispose()
			{
				if (mConn != null)
				{
					mConn.OnDcpMessage -= conn_OnDcpMessage;
					mConn = null;
				}
			}
		}

		public IAsyncResult BeginGetRequest(PhysicalAddress destination, DCP.BlockOptions option)
		{
			Trace.WriteLine("Sending Get " + option.ToString() + " request", null);

			MemoryStream mem = new MemoryStream();

			//ethernet
			Ethernet.Encode(mem, destination, adapter.MacAddress, Ethernet.Type.VLanTaggedFrame);

			//VLAN
			VLAN.Encode(mem, VLAN.Priorities.Priority0, VLAN.Type.PN);

			//Profinet Real Time
			RT.EncodeFrameId(mem, RT.FrameIds.DCP_Get_Set_PDU);

			//Profinet DCP
			UInt16 xid = ++lastXid;
			DCP.EncodeGetRequest(mem, xid, option);
			//start Async
			return new ProfinetAsyncDcpResult(this, mem, xid);
		}

		public void SendGetResponse(PhysicalAddress destination, uint xid, DCP.BlockOptions option, object data)
		{
			Trace.WriteLine("Sending Get " + option.ToString() + " response", null);

			MemoryStream mem = new MemoryStream();

			//ethernet
			Ethernet.Encode(mem, destination, adapter.MacAddress, Ethernet.Type.VLanTaggedFrame);

			//VLAN
			VLAN.Encode(mem, VLAN.Priorities.Priority0, VLAN.Type.PN);

			//Profinet Real Time
			RT.EncodeFrameId(mem, RT.FrameIds.DCP_Get_Set_PDU);

			//Profinet DCP
			DCP.EncodeGetResponse(mem, xid, option, data);

			//send
			Send(mem);
		}

		public IAsyncResult BeginSetRequest(PhysicalAddress destination, DCP.BlockOptions option, DCP.BlockQualifiers qualifiers, byte[] data)
		{
			Trace.WriteLine("Sending Set " + option.ToString() + " request", null);

			MemoryStream mem = new MemoryStream();

			//ethernet
			Ethernet.Encode(mem, destination, adapter.MacAddress, Ethernet.Type.VLanTaggedFrame);

			//VLAN
			VLAN.Encode(mem, VLAN.Priorities.Priority0, VLAN.Type.PN);

			//Profinet Real Time
			RT.EncodeFrameId(mem, RT.FrameIds.DCP_Get_Set_PDU);

			//Profinet DCP
			UInt16 xid = ++lastXid;
			DCP.EncodeSetRequest(mem, xid, option, qualifiers, data);

			//start Async
			return new ProfinetAsyncDcpResult(this, mem, xid);
		}

		public void SendSetResponse(PhysicalAddress destination, uint xid, DCP.BlockOptions option, DCP.BlockErrors status)
		{
			Trace.WriteLine("Sending Set " + option.ToString() + " response", null);

			MemoryStream mem = new MemoryStream();

			//ethernet
			Ethernet.Encode(mem, destination, adapter.MacAddress, Ethernet.Type.VLanTaggedFrame);

			//VLAN
			VLAN.Encode(mem, VLAN.Priorities.Priority0, VLAN.Type.PN);

			//Profinet Real Time
			RT.EncodeFrameId(mem, RT.FrameIds.DCP_Get_Set_PDU);

			//Profinet DCP
			DCP.EncodeSetResponse(mem, xid, option, status);

			//Send
			Send(mem);
		}

		public DCP.BlockErrors EndSetRequest(IAsyncResult result, int timeoutMs)
		{
			ProfinetAsyncDcpResult r = (ProfinetAsyncDcpResult)result;

			if (result.AsyncWaitHandle.WaitOne(timeoutMs))
			{
				DCP.BlockErrors ret = ((DCP.ResponseStatus)r.Result[DCP.BlockOptions.Control_Response]).Error;
				r.Dispose();
				return ret;
			}
			else
			{
				r.Dispose();
				throw new TimeoutException("No response received");
			}
		}

		public Dictionary<DCP.BlockOptions, object> EndGetRequest(IAsyncResult result, int timeoutMs)
		{
			ProfinetAsyncDcpResult r = (ProfinetAsyncDcpResult)result;

			if (result.AsyncWaitHandle.WaitOne(timeoutMs))
			{
				Dictionary<DCP.BlockOptions, object> ret = r.Result;
				r.Dispose();
				return ret;
			}
			else
			{
				r.Dispose();
				throw new TimeoutException("No response received");
			}
		}

		public IAsyncResult BeginSetSignalRequest(PhysicalAddress destination)
		{
			return BeginSetRequest(destination, DCP.BlockOptions.Control_Signal, DCP.BlockQualifiers.Temporary, BitConverter.GetBytes((ushort)0x100));      //SignalValue - Flash once
		}

		public IAsyncResult BeginSetResetRequest(PhysicalAddress destination)
		{
			return BeginSetRequest(destination, DCP.BlockOptions.Control_FactoryReset, DCP.BlockQualifiers.Permanent, null);
		}

		public IAsyncResult BeginSetNameRequest(PhysicalAddress destination, string name)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(name);
			return BeginSetRequest(destination, DCP.BlockOptions.DeviceProperties_NameOfStation, DCP.BlockQualifiers.Permanent, bytes);
		}

		public IAsyncResult BeginSetIpRequest(PhysicalAddress destination, IPAddress ip, IPAddress subnetMask, IPAddress gateway)
		{
			byte[] bytes = new byte[12];
			Array.Copy(ip.GetAddressBytes(), 0, bytes, 0, 4);
			Array.Copy(subnetMask.GetAddressBytes(), 0, bytes, 4, 4);
			Array.Copy(gateway.GetAddressBytes(), 0, bytes, 8, 4);
			return BeginSetRequest(destination, DCP.BlockOptions.IP_IPParameter, DCP.BlockQualifiers.Permanent, bytes);
		}

		public IAsyncResult BeginSetIpFullRequest(PhysicalAddress destination, IPAddress ip, IPAddress subnetMask, IPAddress gateway, IPAddress[] dns)
		{
			byte[] bytes = new byte[28];
			Array.Copy(ip.GetAddressBytes(), 0, bytes, 0, 4);
			Array.Copy(subnetMask.GetAddressBytes(), 0, bytes, 4, 4);
			Array.Copy(gateway.GetAddressBytes(), 0, bytes, 8, 4);
			if (dns == null || dns.Length != 4) throw new ArgumentException("dns array length must be 4");
			for (int i = 0; i < 4; i++)
				Array.Copy(dns[i].GetAddressBytes(), 0, bytes, 12 + i * 4, 4);
			return BeginSetRequest(destination, DCP.BlockOptions.IP_FullIPSuite, DCP.BlockQualifiers.Permanent, bytes);
		}

		public DCP.BlockErrors SendSetRequest(PhysicalAddress destination, int timeoutMs, int retries, DCP.BlockOptions option, DCP.BlockQualifiers qualifiers, byte[] data)
		{
			for (int r = 0; r < retries; r++)
			{
				IAsyncResult asyncResult = BeginSetRequest(destination, option, qualifiers, data);
				try
				{
					return EndSetRequest(asyncResult, timeoutMs);
				}
				catch (TimeoutException)
				{
					//continue
				}
			}
			throw new TimeoutException("No response received");
		}

		public DCP.BlockErrors SendSetRequest(PhysicalAddress destination, int timeoutMs, int retries, DCP.BlockOptions option, byte[] data)
		{
			return SendSetRequest(destination, timeoutMs, retries, option, DCP.BlockQualifiers.Permanent, data);
		}

		public Dictionary<DCP.BlockOptions, object> SendGetRequest(PhysicalAddress destination, int timeoutMs, int retries, DCP.BlockOptions option)
		{
			for (int r = 0; r < retries; r++)
			{
				IAsyncResult asyncResult = BeginGetRequest(destination, option);
				try
				{
					return EndGetRequest(asyncResult, timeoutMs);
				}
				catch (TimeoutException)
				{
					//continue
				}
			}
			throw new TimeoutException("No response received");
		}

		public DCP.BlockErrors SendSetSignalRequest(PhysicalAddress destination, int timeoutMs, int retries)
		{
			for (int r = 0; r < retries; r++)
			{
				IAsyncResult asyncResult = BeginSetSignalRequest(destination);
				try
				{
					return EndSetRequest(asyncResult, timeoutMs);
				}
				catch (TimeoutException)
				{
					//continue
				}
			}
			throw new TimeoutException("No response received");
		}

		public DCP.BlockErrors SendSetResetRequest(PhysicalAddress destination, int timeoutMs, int retries)
		{
			for (int r = 0; r < retries; r++)
			{
				IAsyncResult asyncResult = BeginSetResetRequest(destination);
				try
				{
					return EndSetRequest(asyncResult, timeoutMs);
				}
				catch (TimeoutException)
				{
					//continue
				}
			}
			throw new TimeoutException("No response received");
		}

		public DCP.BlockErrors SendSetNameRequest(PhysicalAddress destination, int timeoutMs, int retries, string name)
		{
			for (int r = 0; r < retries; r++)
			{
				IAsyncResult asyncResult = BeginSetNameRequest(destination, name);
				try
				{
					return EndSetRequest(asyncResult, timeoutMs);
				}
				catch (TimeoutException)
				{
					//continue
				}
			}
			throw new TimeoutException("No response received");
		}

		public DCP.BlockErrors SendSetIpFullRequest(PhysicalAddress destination, int timeoutMs, int retries, IPAddress ip, IPAddress subnetMask, IPAddress gateway, IPAddress[] dns)
		{
			for (int r = 0; r < retries; r++)
			{
				IAsyncResult asyncResult = BeginSetIpFullRequest(destination, ip, subnetMask, gateway, dns);
				try
				{
					return EndSetRequest(asyncResult, timeoutMs);
				}
				catch (TimeoutException)
				{
					//continue
				}
			}
			throw new TimeoutException("No response received");
		}

		public DCP.BlockErrors SendSetIpRequest(PhysicalAddress destination, int timeoutMs, int retries, IPAddress ip, IPAddress subnetMask, IPAddress gateway)
		{
			for (int r = 0; r < retries; r++)
			{
				IAsyncResult asyncResult = BeginSetIpRequest(destination, ip, subnetMask, gateway);
				try
				{
					return EndSetRequest(asyncResult, timeoutMs);
				}
				catch (TimeoutException)
				{
					//continue
				}
			}
			throw new TimeoutException("No response received");
		}
	}
}