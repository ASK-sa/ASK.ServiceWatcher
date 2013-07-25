using System;
using System.Configuration;
using ASK.ServEasy;
using ASK.ServEasy.Windows.Services;
using ASK.ServiceWatcher.Config;
using log4net;

namespace ASK.ServiceWatcher
{
	/// <summary>
	/// Summary description for ServiceWatcherModule.
	/// </summary>
	public class ServiceWatcherModule : Module
	{
		#region log4net Declaration
		private static readonly ILog TheLogger = LogManager.GetLogger(typeof(ServiceWatcherModule));
		#endregion

		private Config.Configuration _theConfiguration;

		public bool DiskFullFlag { get; set; }

		protected override void Initializing()
		{
			_theConfiguration = (Config.Configuration)ConfigurationManager.GetSection("serviceWatcher");
			AddThread(new DiskSpaceMonitorThread(this, _theConfiguration.Disks));
		}

		protected override void Running()
		{
			using(ServiceController controller = new ServiceController())
			{
				foreach(Service service in _theConfiguration.Services)
				{
					try
					{
						TheLogger.DebugFormat("Watch Service State for {0}",service.ServiceName);

						if(!controller.ServiceExists(service.ServiceName))
						{
							TheLogger.ErrorFormat("Service Named {0} did'nt exist !",service.ServiceName);
						}
						else
						{
							ServiceState serviceState = controller.GetServiceState(service.ServiceName);

							if(DiskFullFlag)
							{
								if(serviceState == ServiceState.RUNNING)
								{
									string message = String.Format("Disk full detected, Stopping service {0}...", service.ServiceName);

									System.Diagnostics.EventLog.WriteEntry("ASK Service Watcher", message, System.Diagnostics.EventLogEntryType.Error);
									TheLogger.WarnFormat(message);
									controller.StopServiceAndWait(service.ServiceName, new TimeSpan(0, 5, 0));
									TheLogger.Info("Service Stopped");
								}
							}
							else
							{
								if(service.Strategy.MustRun)
								{
									if(serviceState != ServiceState.RUNNING)
									{
										string message = String.Format("Starting Service {0}", service.ServiceName);
										System.Diagnostics.EventLog.WriteEntry("ASK Service Watcher", message, System.Diagnostics.EventLogEntryType.Information);
										TheLogger.InfoFormat(message);
										controller.StartServiceAndWait(service.ServiceName, new TimeSpan(0, 1, 0));
										TheLogger.Info("Service Started");
									}
								}
								else
								{
									if(serviceState != ServiceState.STOPPED)
									{
										string message = String.Format("Stopping Service {0}", service.ServiceName);
										System.Diagnostics.EventLog.WriteEntry("ASK Service Watcher", message, System.Diagnostics.EventLogEntryType.Information);
										TheLogger.InfoFormat(message);
										controller.StopServiceAndWait(service.ServiceName, new TimeSpan(0, 1, 0));
										TheLogger.Info("Service Stopped");
									}
								}
							}
							ResetWatchdog();
						}
					}
					catch(Exception e)
					{
						TheLogger.Error("Error while processing service "+service.ServiceName, e);
					}
				}
			}
			Sleep(TimeSpan.FromMinutes(1));
		}


		public static void Main(string[] argv)
		{
			Loader.LoadModule(new ServiceWatcherModule());
		}
	}
}
