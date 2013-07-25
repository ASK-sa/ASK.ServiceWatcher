using System.Configuration;

namespace ASK.ServiceWatcher.Config
{
	internal class Configuration : ConfigurationSection
	{
		[ConfigurationCollection(typeof (Service), AddItemName = "service")]
		[ConfigurationProperty("services", IsRequired = true)]
		public ServiceCollection Services
		{
			get { return (ServiceCollection) this["services"]; }
			set { this["services"] = value; }
		}

		[ConfigurationCollection(typeof (DiskMonitoring), AddItemName = "disk")]
		[ConfigurationProperty("disks", IsRequired = true)]
		public DiskMonitoringCollection Disks
		{
			get { return (DiskMonitoringCollection) this["disks"]; }
			set { this["disks"] = value; }
		}
	}
}
