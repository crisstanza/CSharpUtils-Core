using System;
using System.IO;

namespace io.github.crisstanza.csharputils
{
	public class FixedWidthColumnReader
	{
		public class Line
		{
			private readonly string line;
			private readonly int[] indexes;
			private int index;

			public Line(string line, int[] indexes)
			{
				this.line = line;
				this.indexes = indexes;
				this.index = 0;
			}

			public int NextValueAsInt()
			{
				return Int32.Parse(this.NextValue());
			}
			public string NextValue()
			{
				int nextIndex = this.index < this.indexes.Length - 1 ? this.indexes[this.index + 1] : this.line.Length;
				int length = nextIndex - this.indexes[this.index];
				return this.line.Substring(this.indexes[this.index++], length).Trim();
			}
		}

		private readonly StringReader reader;
		private int[] indexes;

		public FixedWidthColumnReader(string contents)
		{
			this.reader = new StringReader(contents);
		}

		public void Headers(params string[] headers)
		{
			string firstLine = reader.ReadLine();
			this.indexes = new int[headers.Length];
			int i = 0;
			foreach (string header in headers)
			{
				int previous = i == 0 ? 0 : this.indexes[i - 1] + headers[i - 1].Length;
				indexes[i++] = firstLine.IndexOf(header, previous);
			}
		}

		public bool HasLines()
		{
			return this.reader.Peek() > -1;
		}

		public Line NextLine()
		{
			return new Line(reader.ReadLine(), this.indexes);
		}
	}
}
