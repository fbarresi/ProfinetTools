using System;
using System.Linq;

namespace ProfinetTools.Logic.Protocols
{
	public class Ethernet
	{
		public enum Type : ushort
		{
			None = 0,
			Loop = 96,
			Echo = 512,
			IpV4 = 2048,
			Arp = 2054,
			WakeOnLan = 2114,
			ReverseArp = 32821,
			AppleTalk = 32923,
			AppleTalkArp = 33011,
			VLanTaggedFrame = 33024,
			NovellInternetworkPacketExchange = 33079,
			Novell = 33080,
			IpV6 = 34525,
			MacControl = 34824,
			CobraNet = 34841,
			MultiprotocolLabelSwitchingUnicast = 34887,
			MultiprotocolLabelSwitchingMulticast = 34888,
			PointToPointProtocolOverEthernetDiscoveryStage = 34915,
			PointToPointProtocolOverEthernetSessionStage = 34916,
			ExtensibleAuthenticationProtocolOverLan = 34958,
			HyperScsi = 34970,
			AtaOverEthernet = 34978,
			EtherCatProtocol = 34980,
			ProviderBridging = 34984,
			AvbTransportProtocol = 34997,
			LLDP = 35020,
			SerialRealTimeCommunicationSystemIii = 35021,
			CircuitEmulationServicesOverEthernet = 35032,
			HomePlug = 35041,
			MacSecurity = 35045,
			PrecisionTimeProtocol = 35063,
			ConnectivityFaultManagementOrOperationsAdministrationManagement = 35074,
			FibreChannelOverEthernet = 35078,
			FibreChannelOverEthernetInitializationProtocol = 35092,
			QInQ = 37120,
			VeritasLowLatencyTransport = 51966,
		}

		public static int Encode(System.IO.Stream buffer, System.Net.NetworkInformation.PhysicalAddress destination, System.Net.NetworkInformation.PhysicalAddress source, Type type)
		{
			//destination
			DCP.EncodeOctets(buffer, destination.GetAddressBytes());

			//source
			DCP.EncodeOctets(buffer, source.GetAddressBytes());

			//type
			DCP.EncodeU16(buffer, (ushort)type);

			return 14;
		}
	}
}