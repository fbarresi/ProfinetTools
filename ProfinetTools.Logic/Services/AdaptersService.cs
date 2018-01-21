using System;
using System.Collections.Generic;
using ProfinetTools.Interfaces.Services;
using SharpPcap;

namespace ProfinetTools.Logic.Services
{
	public class AdaptersService : IAdaptersService
	{
		public List<ICaptureDevice> GetAdapters()
		{
			var devices = new List<ICaptureDevice>();

			try
			{
				foreach (ICaptureDevice dev in CaptureDeviceList.Instance)
					devices.Add(dev);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return devices;
		}
	}
}