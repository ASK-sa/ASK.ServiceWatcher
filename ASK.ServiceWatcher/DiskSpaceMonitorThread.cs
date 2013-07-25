using System;
using ASK.ServEasy;
using System.IO;
using ASK.ServiceWatcher.Config;
using log4net;

namespace ASK.ServiceWatcher
{
	internal class DiskSpaceMonitorThread : ModuleThread
	{
		#region log4net Declaration

		private static readonly ILog TheLogger = LogManager.GetLogger(typeof (DiskSpaceMonitorThread));

		#endregion

		private readonly ServiceWatcherModule _theModule;
		private readonly DiskMonitoringCollection _theDiskMonitoringConfig;

		public DiskSpaceMonitorThread(ServiceWatcherModule aModule, DiskMonitoringCollection aDiskMonitoringCollection)
			: base("DiskSpaceMonitor")
		{
			_theModule = aModule;
			_theDiskMonitoringConfig = aDiskMonitoringCollection;
		}

		protected override void Running()
		{
			bool diskFullFound = false;

			foreach (DiskMonitoring dm in _theDiskMonitoringConfig)
			{
				DriveInfo di = new DriveInfo(dm.Name);
				long remainingMegaBytes = (long) (di.AvailableFreeSpace/(1024*1024));

				TheLogger.DebugFormat("There is {0} Mb free on drive {1}", remainingMegaBytes, di.Name);
				if (remainingMegaBytes < dm.MinSize)
				{
					diskFullFound = true;

					if (!_theModule.DiskFullFlag)
					{
						string message =
							String.Format("Disk {0} Free space is too low ! ({1}Mb < {2}Mb). All monitored Services will be STOPPED", dm.Name,
							              remainingMegaBytes, dm.MinSize);
						System.Diagnostics.EventLog.WriteEntry("ASK Service Watcher", message, System.Diagnostics.EventLogEntryType.Error,
						                                       0, 0, null);
						TheLogger.WarnFormat(message);
					}
					break;
				}
			}

			_theModule.DiskFullFlag = diskFullFound;
			Sleep(TimeSpan.FromMinutes(1));
		}
	}
}
