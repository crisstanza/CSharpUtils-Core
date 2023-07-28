using System;

namespace io.github.crisstanza.csharputils
{
	public class BooleanUtils
	{
		public bool FromInt(string value)
		{
			return this.FromInt(Convert.ToInt32(value));
		}
		public bool FromInt(string value, bool fallback)
		{
			try
			{
				return this.FromInt(value);
			}
			catch (FormatException)
			{
				return fallback;
			}
		}
		public bool FromInt(int value)
		{
			return value == 1;
		}
		public int ToInt(bool value)
		{
			return value ? 1 : 0;
		}
	}
}
