using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using ProfinetTools.Interfaces.Extensions;
using ProfinetTools.Interfaces.Models;
using ProfinetTools.Interfaces.Services;

namespace ProfinetTools.Gui.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		private readonly IDeviceService deviceService;
		private readonly IAdaptersService adaptersService;
		private readonly ISettingsService settingsService;
		private Device device1;

		public ReactiveUI.ReactiveCommand SaveCommand { get; set; }
		public ReactiveUI.ReactiveCommand ResetCommand { get; set; }


		public SettingsViewModel(IDeviceService deviceService, IAdaptersService adaptersService, ISettingsService settingsService)
		{
			this.deviceService = deviceService;
			this.adaptersService = adaptersService;
			this.settingsService = settingsService;
		}

		public override void Init()
		{
			deviceService.SelectedDevice
				.Do(device => Device = device)
				.Do(device => DeviceName = device?.Name ?? string.Empty)
				.ObserveOnDispatcher()
				.Subscribe()
				.AddDisposableTo(Disposables);

			SaveCommand = ReactiveUI.ReactiveCommand.CreateFromTask(SaveDeviceSettings)
				.AddDisposableTo(Disposables);

			ResetCommand = ReactiveUI.ReactiveCommand.CreateFromTask(ResetDevice)
				.AddDisposableTo(Disposables);
		}

		public string DeviceName { get; set; }

		private async Task<Unit> ResetDevice()
		{
			var adapter = await adaptersService.SelectedAdapter.FirstAsync().ToTask();
			if (adapter == null) return Unit.Default;

			if(Device == null || string.IsNullOrEmpty(DeviceName)) return Unit.Default;

			var result = await settingsService.FactoryReset(adapter, DeviceName);
			if (!result.Success)
				MessageBox.Show("Device refuse: " + result.ErrorMessage, "Device Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			else
				MessageBox.Show("All done!", "Device Info", MessageBoxButton.OK, MessageBoxImage.Information);


			return Unit.Default;
		}

		private async Task<Unit> SaveDeviceSettings()
		{
			var adapter = await adaptersService.SelectedAdapter.FirstAsync().ToTask();
			if (adapter == null) return Unit.Default;

			if (Device == null || string.IsNullOrEmpty(DeviceName) || !settingsService.TryParseNetworkConfiguration(Device)) return Unit.Default;

			var result = await settingsService.SendSettings(adapter, DeviceName, Device);
			if (!result.Success)
				MessageBox.Show("Device refuse: " + result.ErrorMessage, "Device Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			else
				MessageBox.Show("All done!", "Device Info", MessageBoxButton.OK, MessageBoxImage.Information);

			return Unit.Default;
		}


		public Device Device
		{
			get { return device1; }
			set
			{
				if (Equals(value, device1)) return;
				device1 = value;
				raisePropertyChanged();
			}
		}
	}
}