using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using ProfinetTools.Interfaces.Extensions;
using ProfinetTools.Interfaces.Models;
using ProfinetTools.Interfaces.Services;

namespace ProfinetTools.Gui.ViewModels
{
	public class DevicesViewModel : ViewModelBase
	{
		private readonly IDeviceService deviceService;
		private readonly IAdaptersService adaptersService;
		private List<Device> devices;
		public ReactiveUI.ReactiveCommand RefreshCommand { get; set; }

		public DevicesViewModel(IDeviceService deviceService, IAdaptersService adaptersService)
		{
			this.deviceService = deviceService;
			this.adaptersService = adaptersService;
		}

		public override void Init()
		{
			RefreshCommand = ReactiveUI.ReactiveCommand.CreateFromTask(RefreshDevicesList)
				.AddDisposableTo(Disposables);
		}

		private async Task<Unit> RefreshDevicesList()
		{
			var adapter =  await adaptersService.SelectedAdapter.FirstAsync().ToTask();
			if(adapter == null) return Unit.Default;

			Devices = await deviceService.GetDevices(adapter, TimeSpan.FromSeconds(3));
			return Unit.Default;
		}

		public List<Device> Devices
		{
			get { return devices; }
			set
			{
				if (Equals(value, devices)) return;
				devices = value;
				raisePropertyChanged();
			}
		}
	}
}