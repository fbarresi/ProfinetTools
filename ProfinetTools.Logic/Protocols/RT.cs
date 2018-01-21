using System;
using System.Linq;

namespace ProfinetTools.Logic.Protocols
{
	public class RT
	{
		public enum FrameIds : ushort
		{
			PTCP_RTSyncPDU_With_Follow_Up = 0x20,
			PTCP_RTSyncPDU = 0x80,
			Alarm_High = 0xFC01,
			Alarm_Low = 0xFE01,
			DCP_Hello_ReqPDU = 0xFEFC,
			DCP_Get_Set_PDU = 0xFEFD,
			DCP_Identify_ReqPDU = 0xFEFE,
			DCP_Identify_ResPDU = 0xFEFF,
			PTCP_AnnouncePDU = 0xFF00,
			PTCP_FollowUpPDU = 0xFF20,
			PTCP_DelayReqPDU = 0xFF40,
			PTCP_DelayResPDU_With_Follow_Up = 0xFF41,
			PTCP_DelayFuResPDU_With_Follow_Up = 0xFF42,
			PTCP_DelayResPDU = 0xFF43,
			RTC_Start = 0xC000,
			RTC_End = 0xF7FF,
		}

		public const string MulticastMACAdd_Identify_Address = "01-0E-CF-00-00-00";
		public const string MulticastMACAdd_Hello_Address = "01-0E-CF-00-00-01";
		public const string MulticastMACAdd_Range1_Destination_Address = "01-0E-CF-00-01-01";
		public const string MulticastMACAdd_Range1_Invalid_Address = "01-0E-CF-00-01-02";
		public const string PTCP_MulticastMACAdd_Range2_Clock_Synchronization_Address = "01-0E-CF-00-04-00";
		public const string PTCP_MulticastMACAdd_Range3_Clock_Synchronization_Address = "01-0E-CF-00-04-20";
		public const string PTCP_MulticastMACAdd_Range4_Clock_Synchronization_Address = "01-0E-CF-00-04-40";
		public const string PTCP_MulticastMACAdd_Range6_Clock_Synchronization_Address = "01-0E-CF-00-04-80";
		public const string PTCP_MulticastMACAdd_Range8_Address = "01-80-C2-00-00-0E";
		public const string RTC_PDU_RT_CLASS_3_Destination_Address = "01-0E-CF-00-01-01";
		public const string RTC_PDU_RT_CLASS_3_Invalid_Address = "01-0E-CF-00-01-02";

		public static int EncodeFrameId(System.IO.Stream buffer, FrameIds value)
		{
			return DCP.EncodeU16(buffer, (ushort)value);
		}

		public static int DecodeFrameId(System.IO.Stream buffer, out FrameIds value)
		{
			ushort val;
			DCP.DecodeU16(buffer, out val);
			value = (FrameIds)val;
			return 2;
		}

		[Flags]
		public enum DataStatus : byte
		{
			State_Primary = 1,  /* 0 is Backup */
			Redundancy_Backup = 2,  /* 0 is Primary */
			DataItemValid = 4,  /* 0 is invalid */
			ProviderState_Run = 1 << 4, /* 0 is stop */
			StationProblemIndicator_Normal = 1 << 5,    /* 0 is Detected */
			Ignore = 1 << 7,    /* 0 is Evaluate */
		}

		[Flags]
		public enum TransferStatus : byte
		{
			OK = 0,
			AlignmentOrFrameChecksumError = 1,
			WrongLengthError = 2,
			MACReceiveBufferOverflow = 4,
			RT_CLASS_3_Error = 8,
		}

		[Flags]
		public enum IOxS : byte
		{
			Extension_MoreIOxSOctetFollows = 1,
			Instance_DetectedBySubslot = 0 << 5,
			Instance_DetectedBySlot = 1 << 5,
			Instance_DetectedByIODevice = 2 << 5,
			Instance_DetectedByIOController = 3 << 5,
			DataState_Good = 1 << 7,
		}

		[Flags]
		public enum PDUTypes : byte
		{
			//0x00 Reserved —
			Data = 1, //Shall only be used to encode the DATA-RTA-PDU
			Nack = 2, //Shall only be used to encode the NACK-RTA-PDU
			Ack = 3, //Shall only be used to encode the ACK-RTA-PDU
			Err = 4, //Shall only be used to encode the ERR-RTA-PDU
			//0x05 – 0x0F Reserved —
			Version1 = 1 << 4,
		}

		[Flags]
		public enum AddFlags : byte
		{
			WindowSizeOne = 1,
			TACK_ImmediateAcknowledge = 1 << 4,
		}

		public static int DecodeRTCStatus(System.IO.Stream buffer, out UInt16 CycleCounter, out DataStatus DataStatus, out TransferStatus TransferStatus)
		{
			int ret = 0;
			byte tmp;

			ret += DCP.DecodeU16(buffer, out CycleCounter);
			ret += DCP.DecodeU8(buffer, out tmp);
			DataStatus = (DataStatus)tmp;
			ret += DCP.DecodeU8(buffer, out tmp);
			TransferStatus = (TransferStatus)tmp;

			return ret;
		}

		public static int EncodeRTCStatus(System.IO.Stream buffer, UInt16 CycleCounter, DataStatus DataStatus, TransferStatus TransferStatus)
		{
			int ret = 0;

			ret += DCP.EncodeU16(buffer, CycleCounter);
			ret += DCP.EncodeU8(buffer, (byte)DataStatus);
			ret += DCP.EncodeU8(buffer, (byte)TransferStatus);

			return ret;
		}

		public static int DecodeRTAHeader(System.IO.Stream buffer, out UInt16 AlarmDestinationEndpoint, out UInt16 AlarmSourceEndpoint, out PDUTypes PDUType, out AddFlags AddFlags, out UInt16 SendSeqNum, out UInt16 AckSeqNum, out UInt16 VarPartLen)
		{
			int ret = 0;
			byte tmp;

			ret += DCP.DecodeU16(buffer, out AlarmDestinationEndpoint);
			ret += DCP.DecodeU16(buffer, out AlarmSourceEndpoint);
			ret += DCP.DecodeU8(buffer, out tmp);
			PDUType = (PDUTypes)tmp;
			ret += DCP.DecodeU8(buffer, out tmp);
			AddFlags = (AddFlags)tmp;
			ret += DCP.DecodeU16(buffer, out SendSeqNum);
			ret += DCP.DecodeU16(buffer, out AckSeqNum);
			ret += DCP.DecodeU16(buffer, out VarPartLen);

			return ret;
		}
	}
}