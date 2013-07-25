using System;
using System.Collections;

namespace ASK.ServiceWatcher
{
	/// <summary>
	/// Summary description for FixedDateStrategy.
	/// </summary>
	public class CronRunStrategy
	{
		private readonly ArrayList _theConfig = new ArrayList();

		/// <summary>
		/// loopDelay Format : Second Minute Hour Day Month DayofWeek |
		/// 
		/// Position: 	Values:
		/// Second     0-59
		/// Minute     0-59
		/// Hour       0-23
		/// Day        1-31
		/// Month      1-12
		/// Dayofweek  0-6 (0=Sunday, ..., 6=Saterday)
		/// Instead of minute, hour, day, month or day of week it's also possible to specify a *.
		/// A * represents all possible values for that position (e.g. a * on 2nd position is 
		/// the same as specifying all the possible values for hour)
		/// 
		/// Several values can be separated by commas: e.g. if a command is to be executed every 10th 
		/// minute so you can specify 0,10,20,30,40,50 for minute. A range of values can be specified 
		/// with a -: e.g. value 0-12 for hour -> every hour a.m.
		/// 
		/// Several Formats can be separated by '|' character
		/// </summary>
		/// <example>
		///   Format : "* * * * * *"    => Every second
		///   Format : "0 * * * * *"    => Every minute
		///   Format : "0 0 * * * *"    => Every Hour
		///   Format : "0 0,30 * * * *" => Every Hour and Half Hour
		///   Format : "0 0 12 * * 1"   => Every Monday at 12AM
		///   Format : "0 0 0 1-7 * 6"  => First Saterday of the month at midnight
		///   Format : "0 0 0 1-7 * 6|0 0 0 1 1 1"  => First Saterday of the month at midnight and First January.
		/// </example>
		/// <param name="sleepFormat">the Timer Format</param>
		public CronRunStrategy(string sleepFormat)
		{
			LoadConfig(sleepFormat);
		}

		public bool MustRun
		{
			get
			{
				DateTime now = DateTime.Now;

				for(int i=0; i<_theConfig.Count; i++)
				{
					bool[][] config = (bool[][])_theConfig[i];
					if(((config[0][now.Second])&&
						(config[1][now.Minute])&&
						(config[2][now.Hour])&&
						(config[3][now.Day])&&
						(config[4][now.Month])&&
						(config[5][(int)now.DayOfWeek])))
					{
						return true;
					}
				}
				return false;
			}
		}

		private void LoadConfig(string sleepFormat)
		{
			try
			{
				string[] chronTokens = sleepFormat.Split('|');
				for(int i=0; i<chronTokens.Length; i++)
				{
					string[] tokens = chronTokens[i].Split(' ');
					if(tokens.Length != 6)
					{
						throw new FormatException("The timer format cannot have more than 6 fields");
					}

					bool[][] chronConfig = new[]
						                       {
							                       new bool[60], // Seconds
							                       new bool[60], // Minutes 
							                       new bool[24], // Hours
							                       new bool[31 + 1], // Days (Start at 1 not, at 0)
							                       new bool[12 + 1], // Months (Start at 1 not, at 0)
							                       new bool[7] // Day of week
						                       };
					for(int j=0; j<tokens.Length; j++)
					{
						ParseToken(tokens[j].Trim(), j, ref chronConfig);
					}

					_theConfig.Add(chronConfig);
				}
			}
			catch(Exception e)
			{
				throw new FormatException("Error parsing timer format", e);
			}
		}

		private void ParseToken(string token, int tokenId, ref bool[][] chronConfig)
		{
			int rangePos = token.IndexOf('-');
			int listPos = token.IndexOf(',');

			if(rangePos > 0) // Range
			{
				int from = int.Parse(token.Substring(0, rangePos));
				int to = int.Parse(token.Substring(rangePos+1));
				for(int i=0; i<chronConfig[tokenId].Length; i++)
				{
					if((i >= from)&&(i<=to))
					{
						chronConfig[tokenId][i] = true;
					}
					else
					{
						chronConfig[tokenId][i] = false;
					}
				}
			}
			else if(listPos > 0) // list
			{
				string[] list = token.Split(',');

				// Clean all
				for(int i=0; i<chronConfig[tokenId].Length; i++)
				{
					chronConfig[tokenId][i] = false;
				}

				for(int i=0; i<list.Length; i++)
				{
					int val = int.Parse(list[i]);
					chronConfig[tokenId][val] = true;
				}
			}
			else // Simple value
			{
				// test wildcard
				if(token == "*")
				{
					// Create an array with all the values
					for(int i=0; i<chronConfig[tokenId].Length; i++)
					{
						chronConfig[tokenId][i] = true;
					}
				}
				else
				{
					// Cleanup
					for(int i=0; i<chronConfig[tokenId].Length; i++)
					{
						chronConfig[tokenId][i] = false;
					}

					int val = int.Parse(token);
					chronConfig[tokenId][val] = true;
				}
			}
		}
	}
}
