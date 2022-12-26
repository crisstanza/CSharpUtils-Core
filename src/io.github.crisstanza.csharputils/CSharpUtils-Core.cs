using System.Reflection;

namespace io.github.crisstanza.csharputils
{
	public class CSharpUtilsCore
	{
		public static string Version()
		{
			return Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}
	}
}
