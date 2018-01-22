using System;
using System.Collections.Generic;
using System.Linq;
using SharpPcap;

namespace ProfinetTools.Interfaces.Services
{
	public interface IAdaptersService
	{
		List<ICaptureDevice> GetAdapters();

		IObservable<ICaptureDevice> SelectedAdapter { get; }
		void SelectAdapter(ICaptureDevice adapter);
	}
}