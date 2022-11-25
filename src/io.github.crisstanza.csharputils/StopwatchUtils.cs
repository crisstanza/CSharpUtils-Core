using io.github.crisstanza.csharputils.constants;
using System;
using System.Diagnostics;

namespace io.github.crisstanza.csharputils
{
	public class StopwatchUtils
	{

		public Stopwatch StartedStopwatch()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			return stopwatch;
		}

		public string Duration(Stopwatch stopwatch)
		{
			TimeSpan elapsed = stopwatch.Elapsed;
			return String.Format(StopwatchConstants.DURATION, elapsed.Days, elapsed.Hours, elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds);
		}
	}
}
