using io.github.crisstanza.csharputils.constants;
using System;

namespace io.github.crisstanza.csharputils
{
	public class DateTimeUtils
	{
		public string Now()
		{
			return DateTime.Now.ToString(DateFormatConstants.DDMMYYYY_HHMMSSMICROS);
		}

	}
}
