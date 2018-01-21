using System;
using System.Linq;

namespace ProfinetTools.Interfaces.Serialization
{
	public interface IProfinetSerialize
	{
		int Serialize(System.IO.Stream buffer);
	}
}