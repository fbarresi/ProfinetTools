using System;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.Win32;
using ProfinetTools.Interfaces.Commons;
using ReactiveUI.Legacy;

namespace ProfinetTools.Gui.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private readonly IViewModelFactory m_ViewModelFactory;

		public MainWindowViewModel(IViewModelFactory viewModelFactory)
		{
			m_ViewModelFactory = viewModelFactory;
		}
		public override void Init()
		{
			//todo
		}
	}
}