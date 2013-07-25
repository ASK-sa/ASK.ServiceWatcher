using System.Configuration;

namespace ASK.ServiceWatcher.Config
{
	public class ServiceCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new Service();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((Service) element).ServiceName;
		}
	}

	public class DiskMonitoringCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new DiskMonitoring();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((DiskMonitoring) element).Name;
		}
	}
}
