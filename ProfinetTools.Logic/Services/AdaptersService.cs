using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ProfinetTools.Interfaces.Services;
using SharpPcap;

namespace ProfinetTools.Logic.Services
{
	public class AdaptersService : IAdaptersService
	{
		private readonly BehaviorSubject<ICaptureDevice> selectedAdapterSubject = new BehaviorSubject<ICaptureDevice>(null);
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

		public void SelectAdapter(ICaptureDevice adapter)
		{
			selectedAdapterSubject.OnNext(adapter);
		}

		public IObservable<ICaptureDevice> SelectedAdapter => selectedAdapterSubject.AsObservable();
	}
}