using System.Reactive.Linq;
using System.Reactive.Subjects;
using ProfinetTools.Interfaces.Extensions;
using ProfinetTools.Interfaces.Models;
using ProfinetTools.Interfaces.Services;

namespace ProfinetTools.Gui.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		private readonly IDeviceService deviceService;
		private readonly IAdaptersService adaptersService;

		public SettingsViewModel(IDeviceService deviceService, IAdaptersService adaptersService)
		{
			this.deviceService = deviceService;
			this.adaptersService = adaptersService;
		}

		public override void Init()
		{
		}

		

		public Device Device { get; set; }
	}
}