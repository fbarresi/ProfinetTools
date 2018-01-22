using ProfinetTools.Interfaces.Commons;

namespace ProfinetTools.Interfaces.Models
{
	public class Device : NotifyPropertyChanged
	{
		private string mac;
		private string name;
		private string type;
		private string role;
		private string ip;
		private string subnetMask;
		private string gateway;

		public string MAC
		{
			get { return mac; }
			set
			{
				if (value == mac) return;
				mac = value;
				raisePropertyChanged();
			}
		}

		public string Name
		{
			get { return name; }
			set
			{
				if (value == name) return;
				name = value;
				raisePropertyChanged();
			}
		}

		public string Type
		{
			get { return type; }
			set
			{
				if (value == type) return;
				type = value;
				raisePropertyChanged();
			}
		}

		public string Role
		{
			get { return role; }
			set
			{
				if (value == role) return;
				role = value;
				raisePropertyChanged();
			}
		}

		public string IP
		{
			get { return ip; }
			set
			{
				if (value == ip) return;
				ip = value;
				raisePropertyChanged();
			}
		}

		public string SubnetMask
		{
			get { return subnetMask; }
			set
			{
				if (value == subnetMask) return;
				subnetMask = value;
				raisePropertyChanged();
			}
		}

		public string Gateway
		{
			get { return gateway; }
			set
			{
				if (value == gateway) return;
				gateway = value;
				raisePropertyChanged();
			}
		}
	}
}