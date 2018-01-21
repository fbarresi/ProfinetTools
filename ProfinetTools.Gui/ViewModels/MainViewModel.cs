using System;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.Win32;
using ProfinetTools.Interfaces.Commons;
using ProfinetTools.Interfaces.Extensions;
using ReactiveUI.Legacy;

namespace ProfinetTools.Gui.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private readonly IViewModelFactory viewModelFactory;

		public MainWindowViewModel(IViewModelFactory viewModelFactory)
		{
			this.viewModelFactory = viewModelFactory;
		}
		public override void Init()
		{
			AdaptersViewModel = viewModelFactory.CreateViewModel<AdaptersViewModel>();
			AdaptersViewModel.Init();
			AdaptersViewModel.AddDisposableTo(Disposables);
		}

		public AdaptersViewModel AdaptersViewModel { get; set; }
	}
}