using System;
using System.Net.NetworkInformation;

namespace ProfinetTools.Logic.Transport
{
	public class ConnectionInfoEthernet
	{
		public ProfinetEthernetTransport Adapter;
		public PhysicalAddress Destination;
		public PhysicalAddress Source;
		public ConnectionInfoEthernet(ProfinetEthernetTransport adapter, PhysicalAddress destination, PhysicalAddress source)
		{
			Adapter = adapter;
			Destination = destination;
			Source = source;
		}
	}
}