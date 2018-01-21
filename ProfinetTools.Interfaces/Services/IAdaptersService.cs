using System;
using System.Collections.Generic;
using System.Linq;
using SharpPcap;

namespace ProfinetTools.Interfaces.Services
{
	public interface IAdaptersService
	{
		List<ICaptureDevice> GetDevices();
	}
}