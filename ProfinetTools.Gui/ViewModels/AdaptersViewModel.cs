using System;
using System.Collections.Generic;
using System.Linq;
using ProfinetTools.Interfaces.Services;
using SharpPcap;

namespace ProfinetTools.Gui.ViewModels
{
	public class AdaptersViewModel : ViewModelBase
	{
		private readonly IAdaptersService adaptersService;

		public AdaptersViewModel(IAdaptersService adaptersService)
		{
			this.adaptersService = adaptersService;
		}
		public override void Init()
		{
			Adapters = adaptersService.GetAdapters();
		}

		public List<ICaptureDevice> Adapters { get; set; }
	}
}