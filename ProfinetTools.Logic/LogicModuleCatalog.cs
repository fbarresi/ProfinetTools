using System;
using System.Linq;
using Ninject.Modules;
using ProfinetTools.Interfaces.Services;
using ProfinetTools.Logic.Services;

namespace ProfinetTools.Logic
{
    public class LogicModuleCatalog : NinjectModule
    {
        public override void Load()
        {
	        Bind<IAdaptersService>().To<AdaptersService>().InSingletonScope();
	        Bind<IDeviceService>().To<DeviceService>().InSingletonScope();
	        Bind<ISettingsService>().To<SettingsService>().InSingletonScope();
        }
    }
}