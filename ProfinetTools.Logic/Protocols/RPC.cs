using System;
using System.Linq;

namespace ProfinetTools.Logic.Protocols
{
	public class RPC
	{
		public static Guid UUID_IO_ObjectInstance_XYZ = Guid.Parse("DEA00000-6C97-11D1-8271-000000000000");
		public static Guid UUID_IO_DeviceInterface = Guid.Parse("DEA00001-6C97-11D1-8271-00A02442DF7D");
		public static Guid UUID_IO_ControllerInterface = Guid.Parse("DEA00002-6C97-11D1-8271-00A02442DF7D");
		public static Guid UUID_IO_SupervisorInterface = Guid.Parse("DEA00003-6C97-11D1-8271-00A02442DF7D");
		public static Guid UUID_IO_ParameterServerInterface = Guid.Parse("DEA00004-6C97-11D1-8271-00A02442DF7D");
		public static Guid UUID_EPMap_Interface = Guid.Parse("E1AF8308-5D1F-11C9-91A4-08002B14A0FA");
		public static Guid UUID_EPMap_Object = Guid.Parse("00000000-0000-0000-0000-000000000000");

		public enum PacketTypes : byte
		{
			Request = 0,
			Ping = 1,
			Response = 2,
			Fault = 3,
			Working = 4,
			NoCall = 5,
			Reject = 6,
			Acknowledge = 7,
			ConnectionlessCancel = 8,
			FragmentAcknowledge = 9,
			CancelAcknowledge = 10,
		}

		[Flags]
		public enum Flags1 : byte
		{
			LastFragment = 2,
			Fragment = 4,
			NoFragmentAckRequested = 8,
			Maybe = 16,
			Idempotent = 32,
			Broadcast = 64,
		}

		[Flags]
		public enum Flags2 : byte
		{
			CancelPendingAtCallEnd = 2,
		}

		[Flags]
		public enum Encodings : ushort
		{
			ASCII = 0x000,
			EBCDIC = 0x100,
			BigEndian = 0x000,
			LittleEndian = 0x1000,
			IEEE_float = 0,
			VAX_float = 1,
			CRAY_float = 2,
			IBM_float = 3,
		}

		public enum Operations : ushort
		{
			//IO device
			Connect = 0,
			Release = 1,
			Read = 2,
			Write = 3,
			Control = 4,
			ReadImplicit = 5,

			//Endpoint mapper
			Insert = 0,
			Delete = 1,
			Lookup = 2,
			Map = 3,
			LookupHandleFree = 4,
			InqObject = 5,
			MgmtDelete = 6,
		}

		public static Guid GenerateObjectInstanceUUID(UInt16 InstanceNo, byte InterfaceNo, UInt16 DeviceId, UInt16 VendorId)
		{
			byte[] bytes = UUID_IO_ObjectInstance_XYZ.ToByteArray();
			UInt32 Data1 = BitConverter.ToUInt32(bytes, 0);
			UInt16 Data2 = BitConverter.ToUInt16(bytes, 4);
			UInt16 Data3 = BitConverter.ToUInt16(bytes, 6);
			byte Data4 = bytes[8];
			byte Data5 = bytes[9];
			InstanceNo &= 0xFFF;
			InstanceNo |= (UInt16)(InterfaceNo << 12);
			Guid ret = new Guid(Data1, Data2, Data3, Data4, Data5, (byte)(InstanceNo >> 8), (byte)(InstanceNo & 0xFF), (byte)(DeviceId >> 8), (byte)(DeviceId & 0xFF), (byte)(VendorId >> 8), (byte)(VendorId & 0xFF));
			return ret;
		}

		public static int EncodeU32(System.IO.Stream buffer, Encodings encoding, UInt32 value)
		{
			if ((encoding & Encodings.LittleEndian) == Encodings.BigEndian)
			{
				buffer.WriteByte((byte)((value & 0xFF000000) >> 24));
				buffer.WriteByte((byte)((value & 0x00FF0000) >> 16));
				buffer.WriteByte((byte)((value & 0x0000FF00) >> 08));
				buffer.WriteByte((byte)((value & 0x000000FF) >> 00));
			}
			else
			{
				buffer.WriteByte((byte)((value & 0x000000FF) >> 00));
				buffer.WriteByte((byte)((value & 0x0000FF00) >> 08));
				buffer.WriteByte((byte)((value & 0x00FF0000) >> 16));
				buffer.WriteByte((byte)((value & 0xFF000000) >> 24));
			}
			return 4;
		}

		public static int DecodeU32(System.IO.Stream buffer, Encodings encoding, out UInt32 value)
		{
			if ((encoding & Encodings.LittleEndian) == Encodings.BigEndian)
			{
				value = (UInt32)((buffer.ReadByte() << 24) | (buffer.ReadByte() << 16) | (buffer.ReadByte() << 8) | (buffer.ReadByte() << 0));
			}
			else
			{
				value = (UInt32)((buffer.ReadByte() << 0) | (buffer.ReadByte() << 8) | (buffer.ReadByte() << 16) | (buffer.ReadByte() << 24));
			}
			return 4;
		}

		public static int EncodeU16(System.IO.Stream buffer, Encodings encoding, UInt16 value)
		{
			if ((encoding & Encodings.LittleEndian) == Encodings.BigEndian)
			{
				buffer.WriteByte((byte)((value & 0x0000FF00) >> 08));
				buffer.WriteByte((byte)((value & 0x000000FF) >> 00));
			}
			else
			{
				buffer.WriteByte((byte)((value & 0x000000FF) >> 00));
				buffer.WriteByte((byte)((value & 0x0000FF00) >> 08));
			}
			return 2;
		}

		public static int DecodeU16(System.IO.Stream buffer, Encodings encoding, out UInt16 value)
		{
			if ((encoding & Encodings.LittleEndian) == Encodings.BigEndian)
			{
				value = (UInt16)((buffer.ReadByte() << 8) | (buffer.ReadByte() << 0));
			}
			else
			{
				value = (UInt16)((buffer.ReadByte() << 0) | (buffer.ReadByte() << 8));
			}
			return 2;
		}

		public static int EncodeGuid(System.IO.Stream buffer, Encodings encoding, Guid value)
		{
			int ret = 0;
			byte[] bytes = value.ToByteArray();
			UInt32 Data1 = BitConverter.ToUInt32(bytes, 0);
			UInt16 Data2 = BitConverter.ToUInt16(bytes, 4);
			UInt16 Data3 = BitConverter.ToUInt16(bytes, 6);
			byte[] Data4 = new byte[8];
			Array.Copy(bytes, 8, Data4, 0, 8);
			ret += EncodeU32(buffer, encoding, Data1);
			ret += EncodeU16(buffer, encoding, Data2);
			ret += EncodeU16(buffer, encoding, Data3);
			ret += DCP.EncodeOctets(buffer, Data4);
			return ret;
		}

		public static int DecodeGuid(System.IO.Stream buffer, Encodings encoding, out Guid value)
		{
			int ret = 0;
			UInt32 Data1;
			UInt16 Data2;
			UInt16 Data3;
			byte[] Data4 = new byte[8];

			ret += DecodeU32(buffer, encoding, out Data1);
			ret += DecodeU16(buffer, encoding, out Data2);
			ret += DecodeU16(buffer, encoding, out Data3);
			buffer.Read(Data4, 0, Data4.Length);
			ret += Data4.Length;

			value = new Guid((int)Data1, (short)Data2, (short)Data3, Data4);

			return ret;
		}

		public static int EncodeHeader(System.IO.Stream buffer, PacketTypes type, Flags1 flags1, Flags2 flags2, Encodings encoding, UInt16 serial_high_low, Guid object_id, Guid interface_id, Guid activity_id, UInt32 server_boot_time, UInt32 sequence_no, Operations op, UInt16 body_length, UInt16 fragment_no, out long body_length_position)
		{
			int ret = 0;

			ret += DCP.EncodeU8(buffer, 4); //RPCVersion
			ret += DCP.EncodeU8(buffer, (byte)type);
			ret += DCP.EncodeU8(buffer, (byte)flags1);
			ret += DCP.EncodeU8(buffer, (byte)flags2);
			ret += DCP.EncodeU16(buffer, (ushort)encoding);
			ret += DCP.EncodeU8(buffer, 0); //pad
			ret += DCP.EncodeU8(buffer, (byte)(serial_high_low >> 8));
			ret += EncodeGuid(buffer, encoding, object_id);
			ret += EncodeGuid(buffer, encoding, interface_id);
			ret += EncodeGuid(buffer, encoding, activity_id);
			ret += EncodeU32(buffer, encoding, server_boot_time);
			ret += EncodeU32(buffer, encoding, 1);   //interface version
			ret += EncodeU32(buffer, encoding, sequence_no);
			ret += EncodeU16(buffer, encoding, (ushort)op);
			ret += EncodeU16(buffer, encoding, 0xFFFF);     //interface hint
			ret += EncodeU16(buffer, encoding, 0xFFFF);     //activity hint
			body_length_position = buffer.Position;
			ret += EncodeU16(buffer, encoding, body_length);
			ret += EncodeU16(buffer, encoding, fragment_no);
			ret += DCP.EncodeU8(buffer, 0); //authentication protocol
			ret += DCP.EncodeU8(buffer, (byte)(serial_high_low & 0xFF));

			return ret;
		}

		public static int DecodeHeader(System.IO.Stream buffer, out PacketTypes type, out Flags1 flags1, out Flags2 flags2, out Encodings encoding, out UInt16 serial_high_low, out Guid object_id, out Guid interface_id, out Guid activity_id, out UInt32 server_boot_time, out UInt32 sequence_no, out Operations op, out UInt16 body_length, out UInt16 fragment_no)
		{
			int ret = 0;
			byte tmp1;
			UInt16 tmp2;
			UInt32 tmp3;

			serial_high_low = 0;

			ret += DCP.DecodeU8(buffer, out tmp1); //RPCVersion
			if (tmp1 != 4) throw new Exception("Wrong protocol");
			ret += DCP.DecodeU8(buffer, out tmp1);
			type = (PacketTypes)tmp1;
			ret += DCP.DecodeU8(buffer, out tmp1);
			flags1 = (Flags1)tmp1;
			ret += DCP.DecodeU8(buffer, out tmp1);
			flags2 = (Flags2)tmp1;
			ret += DCP.DecodeU16(buffer, out tmp2);
			encoding = (Encodings)tmp2;
			ret += DCP.DecodeU8(buffer, out tmp1); //pad
			ret += DCP.DecodeU8(buffer, out tmp1);
			serial_high_low |= (UInt16)(tmp1 << 8);
			ret += DecodeGuid(buffer, encoding, out object_id);
			ret += DecodeGuid(buffer, encoding, out interface_id);
			ret += DecodeGuid(buffer, encoding, out activity_id);
			ret += DecodeU32(buffer, encoding, out server_boot_time);
			ret += DecodeU32(buffer, encoding, out tmp3);   //interface version
			if ((tmp3 & 0xFFFF) != 1) throw new Exception("Wrong protocol version");
			ret += DecodeU32(buffer, encoding, out sequence_no);
			ret += DecodeU16(buffer, encoding, out tmp2);
			op = (Operations)tmp2;
			ret += DecodeU16(buffer, encoding, out tmp2);     //interface hint
			ret += DecodeU16(buffer, encoding, out tmp2);     //activity hint
			ret += DecodeU16(buffer, encoding, out body_length);
			ret += DecodeU16(buffer, encoding, out fragment_no);
			ret += DCP.DecodeU8(buffer, out tmp1); //authentication protocol
			ret += DCP.DecodeU8(buffer, out tmp1);
			serial_high_low |= tmp1;

			return ret;
		}

		public static void ReEncodeHeaderLength(System.IO.Stream buffer, Encodings encoding, long body_length_position)
		{
			long current_pos = buffer.Position;
			UInt16 actual_length = (UInt16)(buffer.Position - body_length_position - 6);

			buffer.Position = body_length_position;
			EncodeU16(buffer, encoding, actual_length);

			buffer.Position = current_pos;
		}

		public static int EncodeNDRDataHeader(System.IO.Stream buffer, Encodings encoding, UInt32 ArgsMaximum_or_PNIOStatus, UInt32 ArgsLength, UInt32 MaximumCount, UInt32 Offset, UInt32 ActualCount, out long NDRDataHeader_position)
		{
			int ret = 0;

			NDRDataHeader_position = buffer.Position;
			ret += EncodeU32(buffer, encoding, ArgsMaximum_or_PNIOStatus);
			ret += EncodeU32(buffer, encoding, ArgsLength);
			ret += EncodeU32(buffer, encoding, MaximumCount);
			ret += EncodeU32(buffer, encoding, Offset);
			ret += EncodeU32(buffer, encoding, ActualCount);

			return ret;
		}

		public static int DecodeNDRDataHeader(System.IO.Stream buffer, Encodings encoding, out UInt32 ArgsMaximum_or_PNIOStatus, out UInt32 ArgsLength, out UInt32 MaximumCount, out UInt32 Offset, out UInt32 ActualCount)
		{
			int ret = 0;

			ret += DecodeU32(buffer, encoding, out ArgsMaximum_or_PNIOStatus);
			ret += DecodeU32(buffer, encoding, out ArgsLength);
			ret += DecodeU32(buffer, encoding, out MaximumCount);
			ret += DecodeU32(buffer, encoding, out Offset);
			ret += DecodeU32(buffer, encoding, out ActualCount);

			return ret;
		}

		public static void ReEncodeNDRDataHeaderLength(System.IO.Stream buffer, Encodings encoding, long NDRDataHeader_position, bool re_encode_pniostatus)
		{
			long current_pos = buffer.Position;
			UInt32 actual_length = (UInt32)(buffer.Position - NDRDataHeader_position - 20);

			buffer.Position = NDRDataHeader_position;
			if (re_encode_pniostatus) EncodeU32(buffer, encoding, actual_length);     //ArgsMaximum/PNIOStatus
			else buffer.Position += 4;
			EncodeU32(buffer, encoding, actual_length);     //ArgsLength
			EncodeU32(buffer, encoding, actual_length);     //MaximumCount
			buffer.Position += 4;
			EncodeU32(buffer, encoding, actual_length);     //ActualCount

			buffer.Position = current_pos;
		}
	}
}