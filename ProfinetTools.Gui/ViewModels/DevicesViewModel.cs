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
using ReactiveUI;

namespace ProfinetTools.Gui.ViewModels
{
	public class DevicesViewModel : ViewModelBase
	{
		private readonly IDeviceService deviceService;
		private readonly IAdaptersService adaptersService;
		private List<Device> devices = new List<Device>();
		private Device selectedDevice;
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

			this.WhenAnyValue(model => model.SelectedDevice)
				.Subscribe(deviceService.SelectDevice)
				.AddDisposableTo(Disposables)
				;
		}

		private async Task<Unit> RefreshDevicesList()
		{
			var adapter =  await adaptersService.SelectedAdapter.FirstAsync().ToTask();
			if(adapter == null) return Unit.Default;

			try
			{
				Devices = await deviceService.GetDevices(adapter, TimeSpan.FromSeconds(0.5));
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
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

		public Device SelectedDevice
		{
			get { return selectedDevice; }
			set
			{
				if (Equals(value, selectedDevice)) return;
				selectedDevice = value;
				raisePropertyChanged();
			}
		}
	}
}