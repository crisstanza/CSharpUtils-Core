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
	}
}
