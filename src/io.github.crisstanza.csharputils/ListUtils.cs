using System.Collections.Generic;

namespace io.github.crisstanza.csharputils
{
	public class ListUtils
	{
		public int Count<T>(List<T> items)
		{
			return items == null ? 0 : items.Count;
		}
	}
}
