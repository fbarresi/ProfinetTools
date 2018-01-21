using System;
using System.Linq;

namespace ProfinetTools.Logic.Protocols
{
	public class VLAN
	{
		public enum Priorities
		{
			/// <summary>
			/// DCP, IP
			/// </summary>
			Priority0 = 0,
			/// <summary>
			/// Low prior RTA_CLASS_1 or RTA_CLASS_UDP
			/// </summary>
			Priority5 = 5,
			/// <summary>
			/// RT_CLASS_UDP, RT_CLASS_1, RT_CLASS_2, RT_CLASS_3, high prior RTA_CLASS_1 or RTA_CLASS_UDP
			/// </summary>
			Priority6 = 6,
			/// <summary>
			/// PTCP-AnnouncePDU
			/// </summary>
			Priority7 = 7,
		}

		public enum Type : ushort
		{
			/// <summary>
			/// UDP, RPC, SNMP, ICMP
			/// </summary>
			IP = 0x0800,
			ARP = 0x0806,
			TagControlInformation = 0x8100,
			/// <summary>
			/// RTC, RTA, DCP, PTCP, FRAG
			/// </summary>
			PN = 0x8892,
			IEEE_802_1AS = 0x88F7,
			LLDP = 0x88CC,
			MRP = 0x88E3,
		}

		public static int Encode(System.IO.Stream buffer, Priorities priority, Type type)
		{
			UInt16 tmp = 0;

			//Priority
			tmp |= (UInt16)((((UInt16)priority) & 0x7) << 13);

			//CanonicalFormatIdentificator
			tmp |= 0 << 12;

			//VLAN_Id
			tmp |= 0;

			DCP.EncodeU16(buffer, tmp);
			DCP.EncodeU16(buffer, (UInt16)type);

			return 4;
		}
	}
}