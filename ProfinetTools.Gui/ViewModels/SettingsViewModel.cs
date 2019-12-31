using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ProfinetTools.Interfaces.Extensions;
using ProfinetTools.Interfaces.Models;
using ProfinetTools.Interfaces.Services;
using ReactiveUI;

namespace ProfinetTools.Gui.ViewModels
{
	public class SettingsViewModel : ViewModelBase 
    {
		private readonly IDeviceService deviceService;
		private readonly IAdaptersService adaptersService;
		private readonly ISettingsService settingsService;
		private Device _selectedDevice;

        private bool _signalActive = false;

        private string _locateButtonText;
        public string LocateButtonText
        {
            get { return _locateButtonText; }
            set { this.RaiseAndSetIfChanged(ref _locateButtonText, value); }
        }

        private bool _isDeviceSelected;
        public bool IsDeviceSelected
        {
            get { return _isDeviceSelected; }
            set { this.RaiseAndSetIfChanged(ref _isDeviceSelected, value); }
        }


        public ReactiveUI.ReactiveCommand SaveCommand { get; set; }
		public ReactiveUI.ReactiveCommand ResetCommand { get; set; }

        public ReactiveUI.ReactiveCommand SavePermanentCommand { get; set; }

        public ReactiveUI.ReactiveCommand LocateDeviceCommand { get; set; }

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
				.ObserveOnDispatcher()
				.Subscribe()
				.AddDisposableTo(Disposables);

			SaveCommand = ReactiveUI.ReactiveCommand.CreateFromTask(SaveDeviceSettings)
				.AddDisposableTo(Disposables);

            SavePermanentCommand = ReactiveUI.ReactiveCommand.CreateFromTask(SavePermanentDeviceSettings)
                .AddDisposableTo(Disposables);

            ResetCommand = ReactiveUI.ReactiveCommand.CreateFromTask(ResetDevice)
				.AddDisposableTo(Disposables);

            LocateDeviceCommand = ReactiveUI.ReactiveCommand.CreateFromTask(LocateDevice)
                .AddDisposableTo(Disposables);
            LocateButtonText = "Start Blinking";
            IsDeviceSelected = false;
        }


		private async Task<Unit> ResetDevice()
		{
			var adapter = await adaptersService.SelectedAdapter.FirstAsync().ToTask();
			if (adapter == null) return Unit.Default;

			if(Device == null) return Unit.Default;

			var result = await settingsService.FactoryReset(adapter, Device.MAC);
			if (!result.Success)
				MessageBox.Show("Device refuse: " + result.ErrorMessage, "Device Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			else
				MessageBox.Show("All done!", "Device Info", MessageBoxButton.OK, MessageBoxImage.Information);


			return Unit.Default;
		}

        // TODO put some code for flashing in here
        private async Task<Unit> LocateDevice()
        {
            var adapter = await adaptersService.SelectedAdapter.FirstAsync().ToTask();
            if (_signalActive)
            {
                _signalActive = false;
                LocateButtonText = "Start Blinking";
            }
            else
            {
                _signalActive = true;
                var newThread = new Thread(new ThreadStart(LocationService));
                newThread.Start();
                LocateButtonText = "Stop Blinking";
            }
            return Unit.Default;
        }

        // Send Signal request endlessly, until the button is pressed again
        private async void LocationService()
        {
            SaveResult result;
            var adapter = await adaptersService.SelectedAdapter.FirstAsync().ToTask();

            if (adapter == null) return;
            if (Device == null) return;

            while (_signalActive)
            {
                result = await settingsService.SendSignalRequest(adapter, Device.MAC);

                if(!result.Success)
                {
                    MessageBox.Show("Device refuse: " + result.ErrorMessage, "Device Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    _signalActive = false;
                    return;
                }
                Thread.Sleep(4000);
            }

        }

        private async Task<Unit> SaveDeviceSettings()
		{
			var adapter = await adaptersService.SelectedAdapter.FirstAsync().ToTask();
			if (adapter == null) return Unit.Default;

			if (Device == null || !settingsService.TryParseNetworkConfiguration(Device)) return Unit.Default;

			var result = await settingsService.SendSettings(adapter, Device.MAC, Device, false); 
			if (!result.Success)
				MessageBox.Show("Device refuse: " + result.ErrorMessage, "Device Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			else
				MessageBox.Show("All done!", "Device Info", MessageBoxButton.OK, MessageBoxImage.Information);

			return Unit.Default;
		}

        private async Task<Unit> SavePermanentDeviceSettings()
        {
            // TODO permanent saveing setting
            var adapter = await adaptersService.SelectedAdapter.FirstAsync().ToTask();
            if (adapter == null) return Unit.Default;

            if (Device == null || !settingsService.TryParseNetworkConfiguration(Device)) return Unit.Default;

            var result = await settingsService.SendSettings(adapter, Device.MAC, Device, true);
            if (!result.Success)
                MessageBox.Show("Device refuse: " + result.ErrorMessage, "Device Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else
                MessageBox.Show("All done!", "Device Info", MessageBoxButton.OK, MessageBoxImage.Information);

            return Unit.Default;
        }

        public Device Device
		{
			get { return _selectedDevice; }
			set
			{
				if (Equals(value, _selectedDevice)) return;
                _selectedDevice = value;
				raisePropertyChanged();
                IsDeviceSelected = _selectedDevice != null;
            }
        }
	}
}