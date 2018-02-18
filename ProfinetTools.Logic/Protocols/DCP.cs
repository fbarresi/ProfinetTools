using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ProfinetTools.Interfaces.Serialization;

namespace ProfinetTools.Logic.Protocols
{
	public class DCP
	{
		[Flags]
		public enum ServiceIds : ushort
		{
			Get_Request = 0x0300,
			Get_Response = 0x0301,
			Set_Request = 0x0400,
			Set_Response = 0x0401,
			Identify_Request = 0x0500,
			Identify_Response = 0x0501,
			Hello_Request = 0x0600,
			ServiceIDNotSupported = 0x0004,
		}

		public enum BlockOptions : ushort
		{
			//IP
			IP_MACAddress = 0x0101,
			IP_IPParameter = 0x0102,
			IP_FullIPSuite = 0x0103,

			//DeviceProperties
			DeviceProperties_DeviceVendor = 0x0201,
			DeviceProperties_NameOfStation = 0x0202,
			DeviceProperties_DeviceID = 0x0203,
			DeviceProperties_DeviceRole = 0x0204,
			DeviceProperties_DeviceOptions = 0x0205,
			DeviceProperties_AliasName = 0x0206,
			DeviceProperties_DeviceInstance = 0x0207,
			DeviceProperties_OEMDeviceID = 0x0208,

			//DHCP
			DHCP_HostName = 0x030C,
			DHCP_VendorSpecificInformation = 0x032B,
			DHCP_ServerIdentifier = 0x0336,
			DHCP_ParameterRequestList = 0x0337,
			DHCP_ClassIdentifier = 0x033C,
			DHCP_DHCPClientIdentifier = 0x033D,
			DHCP_FullyQualifiedDomainName = 0x0351,
			DHCP_UUIDClientIdentifier = 0x0361,
			DHCP_DHCP = 0x03FF,

			//Control
			Control_Start = 0x0501,
			Control_Stop = 0x0502,
			Control_Signal = 0x0503,
			Control_Response = 0x0504,
			Control_FactoryReset = 0x0505,
			Control_ResetToFactory = 0x0506,

			//DeviceInitiative
			DeviceInitiative_DeviceInitiative = 0x0601,

			//AllSelector
			AllSelector_AllSelector = 0xFFFF,
		}

		public enum BlockQualifiers : ushort
		{
			Temporary = 0,
			Permanent = 1,

			ResetApplicationData = 2,
			ResetCommunicationParameter = 4,
			ResetEngineeringParameter = 6,
			ResetAllStoredData = 8,
			ResetDevice = 16,
			ResetAndRestoreData = 18,
		}

		[Flags]
		public enum BlockInfo : ushort
		{
			IpSet = 1,
			IpSetViaDhcp = 2,
			IpConflict = 0x80,
		}

		[Flags]
		public enum DeviceRoles : byte
		{
			Device = 1,
			Controller = 2,
			Multidevice = 4,
			Supervisor = 8,
		}

		public enum BlockErrors : byte
		{
			NoError = 0,
			OptionNotSupported = 1,
			SuboptionNotSupported = 2,
			SuboptionNotSet = 3,
			ResourceError = 4,
			SetNotPossible = 5,
			Busy = 6,
		}

		public static int EncodeU32(System.IO.Stream buffer, UInt32 value)
		{
			buffer.WriteByte((byte)((value & 0xFF000000) >> 24));
			buffer.WriteByte((byte)((value & 0x00FF0000) >> 16));
			buffer.WriteByte((byte)((value & 0x0000FF00) >> 08));
			buffer.WriteByte((byte)((value & 0x000000FF) >> 00));
			return 4;
		}

		public static int EncodeU16(System.IO.Stream buffer, UInt16 value)
		{
			buffer.WriteByte((byte)((value & 0xFF00) >> 08));
			buffer.WriteByte((byte)((value & 0x00FF) >> 00));
			return 2;
		}

		public static int EncodeU8(System.IO.Stream buffer, byte value)
		{
			buffer.WriteByte((byte)value);
			return 1;
		}

		public static int DecodeU16(System.IO.Stream buffer, out UInt16 value)
		{
			if (buffer.Position >= buffer.Length)
			{
				value = 0;
				return 0;
			}
			value = (UInt16)((buffer.ReadByte() << 8) | buffer.ReadByte());
			return 2;
		}

		public static int DecodeU8(System.IO.Stream buffer, out byte value)
		{
			if (buffer.Position >= buffer.Length)
			{
				value = 0;
				return 0;
			}
			value = (byte)buffer.ReadByte();
			return 1;
		}

		public static int DecodeU32(System.IO.Stream buffer, out UInt32 value)
		{
			if (buffer.Position >= buffer.Length)
			{
				value = 0;
				return 0;
			}
			value = (UInt32)((buffer.ReadByte() << 24) | (buffer.ReadByte() << 16) | (buffer.ReadByte() << 8) | buffer.ReadByte());
			return 4;
		}

		public static int EncodeString(System.IO.Stream buffer, string value)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(value);
			buffer.Write(bytes, 0, bytes.Length);
			return bytes.Length;
		}

		public static int DecodeString(System.IO.Stream buffer, int length, out string value)
		{
			byte[] tmp = new byte[length];
			buffer.Read(tmp, 0, length);
			value = Encoding.ASCII.GetString(tmp);
			return tmp.Length;
		}

		public static int EncodeOctets(System.IO.Stream buffer, byte[] value)
		{
			if (value == null || value.Length == 0) return 0;
			buffer.Write(value, 0, value.Length);
			return value.Length;
		}

		public static int DecodeOctets(System.IO.Stream buffer, int length, out byte[] value)
		{
			if (length <= 0)
			{
				value = null;
				return 0;
			}
			value = new byte[length];
			buffer.Read(value, 0, length);
			return value.Length;
		}

		public static int EncodeHeader(System.IO.Stream buffer, ServiceIds ServiceID, UInt32 Xid, UInt16 ResponseDelayFactor, UInt16 DCPDataLength)
		{
			long dummy;
			return EncodeHeader(buffer, ServiceID, Xid, ResponseDelayFactor, DCPDataLength, out dummy);
		}

		public static int EncodeHeader(System.IO.Stream buffer, ServiceIds ServiceID, UInt32 Xid, UInt16 ResponseDelayFactor, UInt16 DCPDataLength, out long DCPDataLength_pos)
		{
			EncodeU16(buffer, (ushort)ServiceID);

			//big endian uint32
			EncodeU32(buffer, Xid);

			//ResponseDelayFactor, 1 = Allowed value without spread, 2 – 0x1900 = Allowed value with spread
			EncodeU16(buffer, ResponseDelayFactor);

			DCPDataLength_pos = buffer.Position;
			EncodeU16(buffer, DCPDataLength);

			return 10;
		}

		public static void ReEncodeDCPDataLength(System.IO.Stream buffer, long DCPDataLength_pos)
		{
			long current_pos = buffer.Position;
			buffer.Position = DCPDataLength_pos;
			EncodeU16(buffer, (ushort)(current_pos - buffer.Position - 2));
			buffer.Position = current_pos;
		}

		public static int DecodeHeader(System.IO.Stream buffer, out ServiceIds ServiceID, out UInt32 Xid, out UInt16 ResponseDelayFactor, out UInt16 DCPDataLength)
		{
			ushort val;
			DecodeU16(buffer, out val);
			ServiceID = (ServiceIds)val;

			//big endian uint32
			DecodeU32(buffer, out Xid);

			//ResponseDelayFactor, 1 = Allowed value without spread, 2 – 0x1900 = Allowed value with spread
			DecodeU16(buffer, out ResponseDelayFactor);

			DecodeU16(buffer, out DCPDataLength);

			return 10;
		}

		public static int EncodeBlock(System.IO.Stream buffer, BlockOptions options, UInt16 DCPBlockLength)
		{
			long dummy;
			return EncodeBlock(buffer, options, DCPBlockLength, out dummy);
		}

		public static int EncodeBlock(System.IO.Stream buffer, BlockOptions options, UInt16 DCPBlockLength, out long DCPBlockLength_pos)
		{
			EncodeU16(buffer, (ushort)options);
			DCPBlockLength_pos = buffer.Position;
			EncodeU16(buffer, DCPBlockLength);
			return 4;
		}

		public static int DecodeBlock(System.IO.Stream buffer, out BlockOptions options, out UInt16 DCPBlockLength)
		{
			ushort opt;
			DecodeU16(buffer, out opt);
			options = (BlockOptions)opt;
			DecodeU16(buffer, out DCPBlockLength);
			return 4;
		}

		public static int EncodeIdentifyResponse(System.IO.Stream buffer, UInt32 Xid, Dictionary<DCP.BlockOptions, object> blocks)
		{
			int ret = 0;
			long dcp_data_length_pos;

			//Header
			ret += EncodeHeader(buffer, ServiceIds.Identify_Response, Xid, 0, 0, out dcp_data_length_pos);

			//{ IdentifyResBlock, NameOfStationBlockRes,IPParameterBlockRes, DeviceIDBlockRes, DeviceVendorBlockRes,DeviceOptionsBlockRes, DeviceRoleBlockRes, [DeviceInitiativeBlockRes],[DeviceInstanceBlockRes], [OEMDeviceIDBlockRes] }

			foreach (KeyValuePair<DCP.BlockOptions, object> entry in blocks)
			{
				ret += EncodeNextBlock(buffer, entry);
			}

			//adjust dcp_length
			ReEncodeDCPDataLength(buffer, dcp_data_length_pos);

			return ret;
		}

		/// <summary>
		/// This is a helper class for the block options
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter)), Serializable]
		public class BlockOptionMeta
		{
			public string Name { get; set; }
			public bool IsReadable { get; set; }
			public bool IsWriteable { get; set; }

			[Browsable(false), System.Xml.Serialization.XmlIgnore]
			public BlockOptions BlockOption { get; set; }

			public byte Option
			{
				get
				{
					return (byte)((((UInt16)BlockOption) & 0xFF00) >> 8);
				}
				set
				{
					BlockOption = (BlockOptions)((((UInt16)BlockOption) & 0x00FF) | ((UInt16)value << 8));
				}
			}

			public byte SubOption
			{
				get
				{
					return (byte)((((UInt16)BlockOption) & 0x00FF) >> 0);
				}
				set
				{
					BlockOption = (BlockOptions)((((UInt16)BlockOption) & 0xFF00) | ((UInt16)value << 0));
				}
			}

			private BlockOptionMeta()   //For XmlSerializer
			{
			}

			public BlockOptionMeta(string name, byte option, byte sub_option, bool is_readable, bool is_writeable)
			{
				this.BlockOption = 0;
				this.Name = name;
				this.Option = option;
				this.SubOption = sub_option;
				this.IsReadable = is_readable;
				this.IsWriteable = is_writeable;
			}

			public BlockOptionMeta(BlockOptions opt)
			{
				BlockOption = opt;
				Name = opt.ToString();
				if (opt == BlockOptions.IP_MACAddress ||
					opt == BlockOptions.DeviceProperties_DeviceID ||
					opt == BlockOptions.DeviceProperties_DeviceVendor ||
					opt == BlockOptions.DeviceProperties_DeviceRole ||
					opt == BlockOptions.DeviceProperties_DeviceOptions ||
					opt == BlockOptions.DeviceProperties_DeviceInstance ||
					opt == BlockOptions.DeviceProperties_OEMDeviceID ||
					opt == BlockOptions.DeviceInitiative_DeviceInitiative)
				{
					IsReadable = true;
				}
				else if (opt == BlockOptions.DeviceProperties_AliasName ||
					opt == BlockOptions.Control_Response ||
					opt == BlockOptions.AllSelector_AllSelector)
				{
					//none
				}
				else if (opt == BlockOptions.Control_Start ||
					opt == BlockOptions.Control_Stop ||
					opt == BlockOptions.Control_Signal ||
					opt == BlockOptions.Control_FactoryReset ||
					opt == BlockOptions.Control_ResetToFactory)
				{
					IsWriteable = true;
				}
				else
				{
					//default
					IsReadable = true;
					IsWriteable = true;
				}
			}
		}

		public class IpAddressConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				if (sourceType == typeof(string)) return true;
				return base.CanConvertFrom(context, sourceType);
			}
			public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
			{
				if (value is string)
					return System.Net.IPAddress.Parse((string)value);
				return base.ConvertFrom(context, culture, value);
			}
		}

		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class IpInfo : IProfinetSerialize
		{
			public BlockInfo Info { get; set; }
			[TypeConverter(typeof(IpAddressConverter))]
			public System.Net.IPAddress Ip { get; set; }
			[TypeConverter(typeof(IpAddressConverter))]
			public System.Net.IPAddress SubnetMask { get; set; }
			[TypeConverter(typeof(IpAddressConverter))]
			public System.Net.IPAddress Gateway { get; set; }
			public IpInfo(BlockInfo info, byte[] ip, byte[] subnet, byte[] gateway)
			{
				Info = info;
				Ip = new System.Net.IPAddress(ip);
				SubnetMask = new System.Net.IPAddress(subnet);
				Gateway = new System.Net.IPAddress(gateway);
			}
			public override string ToString()
			{
				return "{" + Ip.ToString() + " - " + SubnetMask.ToString() + " - " + Gateway.ToString() + "}";
			}

			public int Serialize(System.IO.Stream buffer)
			{
				int ret = 0;
				byte[] tmp;
				tmp = Ip.GetAddressBytes();
				buffer.Write(tmp, 0, tmp.Length);
				ret += tmp.Length;
				tmp = SubnetMask.GetAddressBytes();
				buffer.Write(tmp, 0, tmp.Length);
				ret += tmp.Length;
				tmp = Gateway.GetAddressBytes();
				buffer.Write(tmp, 0, tmp.Length);
				ret += tmp.Length;
				return ret;
			}
		}

		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class DeviceIdInfo : IProfinetSerialize
		{
			public UInt16 VendorId { get; set; }
			public UInt16 DeviceId { get; set; }
			public DeviceIdInfo(UInt16 vendor_id, UInt16 device_id)
			{
				VendorId = vendor_id;
				DeviceId = device_id;
			}
			public override string ToString()
			{
				return "Vendor 0x" + VendorId.ToString("X") + " - Device 0x" + DeviceId.ToString("X");
			}

			public int Serialize(System.IO.Stream buffer)
			{
				int ret = 0;
				ret += EncodeU16(buffer, VendorId);
				ret += EncodeU16(buffer, DeviceId);
				return ret;
			}
			public override bool Equals(object obj)
			{
				if (!(obj is DeviceIdInfo)) return false;
				DeviceIdInfo o = (DeviceIdInfo)obj;
				return o.DeviceId == DeviceId && o.VendorId == VendorId;
			}
			public override int GetHashCode()
			{
				UInt32 tmp = (uint)(VendorId << 16);
				tmp |= DeviceId;
				return tmp.GetHashCode();
			}
		}

		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class DeviceRoleInfo : IProfinetSerialize
		{
			public DeviceRoles DeviceRole { get; set; }

			public DeviceRoleInfo(DeviceRoles device_role)
			{
				DeviceRole = device_role;
			}

			public int Serialize(System.IO.Stream buffer)
			{
				int ret = 0;
				ret += EncodeU8(buffer, (byte)DeviceRole);
				buffer.WriteByte(0);    //padding
				ret++;
				return ret;
			}

			public override string ToString()
			{
				return DeviceRole.ToString();
			}
		}

		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class ResponseStatus
		{
			public BlockOptions Option { get; set; }
			public BlockErrors Error { get; set; }
			public ResponseStatus()
			{
			}
			public ResponseStatus(BlockOptions Option, BlockErrors Error)
			{
				this.Option = Option;
				this.Error = Error;
			}
			public override string ToString()
			{
				return Error.ToString();
			}
		}

		private static int EncodeNextBlock(System.IO.Stream buffer, KeyValuePair<BlockOptions, object> block)
		{
			int ret = 0;
			int tmp = 0;
			long DCPBlockLength_pos;

			ret += EncodeBlock(buffer, block.Key, 0, out DCPBlockLength_pos);

			//block info
			if (block.Value != null) ret += EncodeU16(buffer, 0);
			else return ret;

			if (block.Value is ResponseStatus)
			{
				buffer.Position -= 2;
				EncodeU16(buffer, (ushort)((ResponseStatus)block.Value).Option);
				tmp += EncodeU8(buffer, (byte)((ResponseStatus)block.Value).Error);
			}
			else if (block.Value is IProfinetSerialize)
			{
				tmp += ((IProfinetSerialize)block.Value).Serialize(buffer);
			}
			else if (block.Value is string)
			{
				tmp += EncodeString(buffer, (string)block.Value);
			}
			else if (block.Value is byte[])
			{
				tmp += EncodeOctets(buffer, (byte[])block.Value);
			}
			else if (block.Value is BlockOptions[])
			{
				foreach (BlockOptions b in (BlockOptions[])block.Value)
				{
					tmp += EncodeU16(buffer, (ushort)b);
				}
			}
			else
			{
				throw new NotImplementedException();
			}

			//adjust length
			ReEncodeDCPDataLength(buffer, DCPBlockLength_pos);

			//padding
			ret += tmp;
			if ((tmp % 2) != 0)
			{
				buffer.WriteByte(0);
				ret++;
			}

			return ret;
		}

		private static int DecodeNextBlock(System.IO.Stream buffer, ushort dcp_length, out KeyValuePair<BlockOptions, object> block)
		{
			int ret = 0;
			BlockOptions options;
			UInt16 dcp_block_length;
			UInt16 block_info = 0;
			UInt16 tmp, tmp2;
			string str;
			object content;
			ResponseStatus set_response;

			if (buffer.Position >= buffer.Length || dcp_length <= 0)
			{
				block = new KeyValuePair<BlockOptions, object>(0, null);
				return ret;
			}

			ret += DecodeBlock(buffer, out options, out dcp_block_length);
			if (dcp_block_length >= 2) ret += DecodeU16(buffer, out block_info);
			dcp_block_length -= 2;

			switch (options)
			{
				case BlockOptions.DeviceProperties_NameOfStation:
					ret += DecodeString(buffer, dcp_block_length, out str);
					content = str;
					break;
				case BlockOptions.IP_IPParameter:
					byte[] ip, subnet, gateway;
					ret += DecodeOctets(buffer, 4, out ip);
					ret += DecodeOctets(buffer, 4, out subnet);
					ret += DecodeOctets(buffer, 4, out gateway);
					content = new IpInfo((BlockInfo)block_info, ip, subnet, gateway); ;
					break;
				case BlockOptions.DeviceProperties_DeviceID:
					ret += DecodeU16(buffer, out tmp);
					ret += DecodeU16(buffer, out tmp2);
					content = new DeviceIdInfo(tmp, tmp2);
					break;
				case BlockOptions.DeviceProperties_DeviceOptions:
					BlockOptions[] option_list = new BlockOptions[dcp_block_length / 2];
					for (int i = 0; i < option_list.Length; i++)
					{
						ret += DecodeU16(buffer, out tmp);
						option_list[i] = (BlockOptions)tmp;
					}
					content = option_list;
					break;
				case BlockOptions.DeviceProperties_DeviceRole:
					DeviceRoles roles = (DeviceRoles)buffer.ReadByte();
					buffer.ReadByte(); //padding
					ret += 2;
					content = new DeviceRoleInfo(roles);
					break;
				case BlockOptions.DeviceProperties_DeviceVendor:
					ret += DecodeString(buffer, dcp_block_length, out str);
					content = str;
					break;
				case BlockOptions.Control_Response:
					set_response = new ResponseStatus();
					set_response.Option = (BlockOptions)block_info;
					set_response.Error = (BlockErrors)buffer.ReadByte();
					ret++;
					content = set_response;
					break;
				default:
					byte[] arr;
					ret += DecodeOctets(buffer, dcp_block_length, out arr);
					content = arr;
					break;
			}
			block = new KeyValuePair<BlockOptions, object>(options, content);

			//padding
			if ((dcp_block_length % 2) != 0)
			{
				buffer.ReadByte();
				ret++;
			}

			return ret;
		}

		public static int DecodeAllBlocks(System.IO.Stream buffer, ushort dcp_length, out Dictionary<BlockOptions, object> blocks)
		{
			int ret = 0, r;
			KeyValuePair<DCP.BlockOptions, object> value;
			blocks = new Dictionary<BlockOptions, object>();
			while ((r = DCP.DecodeNextBlock(buffer, (ushort)(dcp_length - ret), out value)) > 0)
			{
				ret += r;
				if (!blocks.ContainsKey(value.Key)) blocks.Add(value.Key, value.Value);
				else Trace.TraceError("Multiple blocks in reply: " + value.Key);
			}
			if (r < 0) return r;    //error
			else return ret;
		}

		public static int EncodeIdentifyRequest(System.IO.Stream buffer, UInt32 Xid)
		{
			int ret = 0;

			//Header
			ret += EncodeHeader(buffer, ServiceIds.Identify_Request, Xid, 1, 4);

			//optional filter (instead of the ALL block)
			//[NameOfStationBlock] ^ [AliasNameBlock], IdentifyReqBlock

			//IdentifyReqBlock
			/*  DeviceRoleBlock ^ DeviceVendorBlock ^ DeviceIDBlock ^
                DeviceOptionsBlock ^ OEMDeviceIDBlock ^ MACAddressBlock ^
                IPParameterBlock ^ DHCPParameterBlock ^
                ManufacturerSpecificParameterBlock */

			//AllSelectorType
			ret += EncodeBlock(buffer, BlockOptions.AllSelector_AllSelector, 0);

			return ret;
		}

		public static int EncodeSetRequest(System.IO.Stream buffer, UInt32 Xid, BlockOptions options, BlockQualifiers qualifiers, byte[] data)
		{
			int ret = 0;
			int data_length = 0;
			bool do_pad = false;

			if (data != null) data_length = data.Length;
			if ((data_length % 2) != 0) do_pad = true;

			//The following is modified by F.Chaxel. 
			//TODO: Test that decode still work

			ret += EncodeHeader(buffer, ServiceIds.Set_Request, Xid, 0, (ushort)(12 + data_length + (do_pad ? 1 : 0)));
			ret += EncodeBlock(buffer, options, (ushort)(2 + data_length));

			ret += EncodeU16(buffer, (ushort)0); // Don't care

			//data
			EncodeOctets(buffer, data);

			//pad (re-ordered by f.chaxel)
			if (do_pad) ret += EncodeU8(buffer, 0);

			//BlockQualifier
			ret += EncodeBlock(buffer, BlockOptions.Control_Stop, (ushort)(2));
			ret += EncodeU16(buffer, (ushort)qualifiers);

			return ret;
		}

		public static int EncodeGetRequest(System.IO.Stream buffer, UInt32 Xid, BlockOptions options)
		{
			int ret = 0;

			ret += EncodeHeader(buffer, ServiceIds.Get_Request, Xid, 0, 2);
			ret += EncodeU16(buffer, (ushort)options);

			return ret;
		}

		public static int EncodeHelloRequest(System.IO.Stream buffer, UInt32 Xid)
		{
			throw new NotImplementedException();
		}

		public static int EncodeGetResponse(System.IO.Stream buffer, UInt32 Xid, BlockOptions options, object data)
		{
			int ret = 0;
			long dcp_length;

			ret += EncodeHeader(buffer, ServiceIds.Get_Response, Xid, 0, 0, out dcp_length);
			ret += EncodeNextBlock(buffer, new KeyValuePair<BlockOptions, object>(options, data));
			ReEncodeDCPDataLength(buffer, dcp_length);

			return ret;
		}

		public static int EncodeSetResponse(System.IO.Stream buffer, UInt32 Xid, BlockOptions options, BlockErrors status)
		{
			int ret = 0;
			long dcp_length;

			ret += EncodeHeader(buffer, ServiceIds.Set_Response, Xid, 0, 0, out dcp_length);
			ret += EncodeNextBlock(buffer, new KeyValuePair<BlockOptions, object>(BlockOptions.Control_Response, new ResponseStatus(options, status)));
			ReEncodeDCPDataLength(buffer, dcp_length);

			return ret;
		}
	}
}
