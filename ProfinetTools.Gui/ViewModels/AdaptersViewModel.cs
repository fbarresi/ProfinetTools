using System;
using System.Collections.Generic;
using System.Linq;
using ProfinetTools.Interfaces.Extensions;
using ProfinetTools.Interfaces.Services;
using ReactiveUI;
using SharpPcap;

namespace ProfinetTools.Gui.ViewModels
{
	public class AdaptersViewModel : ViewModelBase
	{
		private readonly IAdaptersService adaptersService;
		private ICaptureDevice selectedAdapter;

		public AdaptersViewModel(IAdaptersService adaptersService)
		{
			this.adaptersService = adaptersService;
		}
		public override void Init()
		{
			Adapters = adaptersService.GetAdapters();

			this.WhenAnyValue(model => model.SelectedAdapter)
				.Subscribe(adaptersService.SelectAdapter)
				.AddDisposableTo(Disposables);

			SelectedAdapter = Adapters.FirstOrDefault(device => device.Description.Contains("Ethernet") && !device.Description.Contains("Virtual"));
		}

		public List<ICaptureDevice> Adapters { get; set; }

		public ICaptureDevice SelectedAdapter
		{
			get { return selectedAdapter; }
			set
			{
				if (Equals(value, selectedAdapter)) return;
				selectedAdapter = value;
				raisePropertyChanged();
			}
		}
	}
}