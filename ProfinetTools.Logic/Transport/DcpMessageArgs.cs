using System;
using System.Collections.Generic;
using System.Linq;
using ProfinetTools.Logic.Protocols;

namespace ProfinetTools.Logic.Transport
{
	public class DcpMessageArgs
	{
		public DCP.ServiceIds ServiceID { get; }
		public uint Xid { get; }
		public ushort ResponseDelayFactor { get; }
		public Dictionary<DCP.BlockOptions, object> Blocks { get; }

		public DcpMessageArgs(DCP.ServiceIds serviceID, uint xid, ushort responseDelayFactor, Dictionary<DCP.BlockOptions, object> blocks)
		{
			ServiceID = serviceID;
			Xid = xid;
			ResponseDelayFactor = responseDelayFactor;
			Blocks = blocks;
		}
	}
}