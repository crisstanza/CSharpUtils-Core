using System;

namespace io.github.crisstanza.csharputils
{
	public class StringUtils
	{
		public bool IsBlank(string value)
		{
			return value == null || value.Trim().Length == 0;
		}

		public string defaultString(string value)
		{
			return value == null ? "" : value;
		}
		public string[] defaultArray(string[] value)
		{
			return value == null ? new string[0] : value;
		}

		public bool NotStartsWith(string value, string prefix)
		{
			return value == null || !value.StartsWith(prefix);
		}
	}
}
