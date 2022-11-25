using System;
using System.IO;
using System.Reflection;

namespace io.github.crisstanza.csharputils
{
	public class FileSystemUtils
	{
		public string CurrentPath()
		{
			return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar;
		}

		public string[] ListFolders(string home)
		{
			return Directory.GetDirectories(home);
		}
		public string FolderName(string path)
		{
			return new DirectoryInfo(path).Name;
		}
		public string GetTextFromFile(string path)
		{
			return GetTextFromFile(path, false);
		}
		public string GetTextFromFile(string path, bool lenient)
		{
			if (lenient && !ExistsFile(path))
			{
				return null;
			}
			return File.ReadAllText(path).Trim();
		}
		public string[] GetLinesFromFile(string path)
		{
			return GetLinesFromFile(path, false);
		}
		public string[] GetLinesFromFile(string path, bool lenient)
		{
			if (lenient && !ExistsFile(path))
			{
				return null;
			}
			return File.ReadAllLines(path);
		}
		public bool ExistsFile(string path)
		{
			return File.Exists(path);
		}
	}
}
