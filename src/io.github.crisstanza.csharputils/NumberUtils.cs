namespace io.github.crisstanza.csharputils
{
	public class NumberUtils
	{
		private const double _k = 1024;
		private const double _m = _k * _k;
		private const double _g = _k * _k * _k;
		private const double _t = _k * _k * _k * _k;

		public string SizeFromBytes(int bytes)
		{
			if (bytes < _k)
			{
				return bytes + " byte" + (bytes == 1 ? "" : "s");
			}
			else if (bytes < _m)
			{
				return bytes / _k + " KB";
			}
			else if (bytes < _g)
			{
				return bytes / _m + " MB";
			}
			else if (bytes < _t)
			{
				return bytes / _g + " GB";
			}
			else
			{
				return bytes + " bytes";
			}
		}
	}
}
