using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ProfinetTools.Interfaces.Commons
{
	public abstract class NotifyPropertyChanged : INotifyPropertyChanged
	{
		public virtual event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void raisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
				return;
			propertyChanged((object)this, new PropertyChangedEventArgs(propertyName));
		}
	}
}