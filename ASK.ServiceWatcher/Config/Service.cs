using System.Configuration;

namespace ASK.ServiceWatcher.Config
{
	/// <summary>
	/// Summary description for Service.
	/// </summary>
	public class Service : ConfigurationElement
	{
		private CronRunStrategy _theStrategy;

		[ConfigurationProperty("name", IsKey = true, IsRequired = true)]
		public string ServiceName
		{
			get { return (string) this["name"]; }
			set { this["name"] = value; }
		}

		[ConfigurationProperty("cronStrategy", DefaultValue = "* * * * * *")]
		public string RunCronStrategy
		{
			get { return (string) this["cronStrategy"]; }
			set { this["cronStrategy"] = value; }
		}

		public CronRunStrategy Strategy
		{
			get
			{
				if (_theStrategy == null)
				{
					_theStrategy = new CronRunStrategy(RunCronStrategy);
				}
				return _theStrategy;
			}
		}
	}
}
