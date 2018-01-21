using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ProfinetTools.Interfaces.Commons;
using ReactiveUI;

namespace ProfinetTools.Gui.ViewModels
{
	public abstract class ViewModelBase : ReactiveObject, IDisposable, IInitializable
	{
		protected CompositeDisposable Disposables = new CompositeDisposable();
		private bool disposed;

		~ViewModelBase()
		{
			Dispose(false);
		}

		public virtual void Dispose()
		{
			Dispose(true);
		}

		public abstract void Init();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "Disposables")]
		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
				return;

			Disposables?.Dispose();
			Disposables = null;

			disposed = true;
		}

		[NotifyPropertyChangedInvocator]
		// ReSharper disable once InconsistentNaming
		protected void raisePropertyChanged([CallerMemberName] string propertyName = "")
		{
			this.RaisePropertyChanged(propertyName);
		}
	}
}