using System.Configuration;

namespace ASK.ServiceWatcher.Config
{
	internal class DiskMonitoring : ConfigurationElement
	{
		[ConfigurationProperty("name", IsKey = true, IsRequired = true)]
		public string Name
		{
			get { return (string) this["name"]; }
			set { this["name"] = value; }
		}

		[ConfigurationProperty("minSize", IsRequired = true)]
		public int MinSize
		{
			get { return (int) this["minSize"]; }
			set { this["minSize"] = value; }
		}
	}
}
