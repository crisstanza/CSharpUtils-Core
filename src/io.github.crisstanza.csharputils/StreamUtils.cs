using System.IO;
using System.Text;

namespace io.github.crisstanza.csharputils
{
	public class StreamUtils
	{
		public string ReadToEnd(Stream inputStream, Encoding contentEncoding)
		{
			StreamReader reader = new StreamReader(inputStream, contentEncoding);
			try
			{
				string all = reader.ReadToEnd();
				return all;
			}
			finally
			{
				reader.Close();
			}
		}
	}
}
