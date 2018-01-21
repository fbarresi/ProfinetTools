using System;
using System.Linq;

namespace ProfinetTools.Interfaces.Serialization
{
	public interface IProfinetDeserialize   //Should be merged with IProfinetSerialize, at some point
	{
		int Deserialize(System.IO.Stream buffer);
	}
}