﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace io.github.crisstanza.csharputils
{
	public class RunTimeUtils
	{
		private bool debug;

		public RunTimeUtils() : this(false)
		{
		}

		public RunTimeUtils(bool debug)
		{
			this.debug = debug;
		}

		public class ExecResult
		{
			public int ExitCode { get; set; }
			public string Output { get; set; }
		}

		public ExecResult Exec(string executable, string arguments)
		{
			return this.Exec(executable, arguments, null);
		}

		public ExecResult Exec(string executable, string arguments, string workingDirectory)
		{
			return this.Exec(executable, arguments, workingDirectory, null);
		}

		public ExecResult Exec(string executable, string arguments, string workingDirectory, string answer)
		{
			if (this.debug)
			{
				Log(workingDirectory, executable, arguments);
			}
			Process process = new Process();
			process.StartInfo.FileName = executable;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			if (answer != null)
			{
				process.StartInfo.RedirectStandardInput = true;
			}
			if (workingDirectory != null)
			{
				process.StartInfo.WorkingDirectory = workingDirectory;
			}
			process.Start();
			StringBuilder output = new StringBuilder();
			if (answer != null)
			{
				process.StandardInput.WriteLine(answer);
			}
			ConsumeReader(process.StandardOutput, output).Wait();
			process.WaitForExit();
			if (process.ExitCode == 0)
			{
				// string commandOutput = process.StandardOutput.ReadToEnd();
				return new ExecResult()
				{
					ExitCode = process.ExitCode,
					Output = output.ToString() // commandOutput
				};
			}
			else
			{
				string commandErrorOutput = process.StandardError.ReadToEnd();
				return new ExecResult()
				{
					ExitCode = process.ExitCode,
					Output = output.ToString() + Environment.NewLine + commandErrorOutput
				};
			}
		}

		private async Task ConsumeReader(TextReader reader, StringBuilder acc)
		{
			string text;
			while ((text = await reader.ReadLineAsync()) != null)
			{
				acc.AppendLine(text);
			}
		}

		public int Run(string executable, string arguments)
		{
			if (this.debug)
			{
				Log(null, executable, arguments);
			}
			Thread thread = new Thread(() =>
			{
				Process process = new Process();
				process.StartInfo.FileName = executable;
				process.StartInfo.Arguments = arguments;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.OutputDataReceived += (sender, args) =>
				{
					if (debug)
					{
						Console.WriteLine("Output: {0}", args.Data);
					}
				};
				process.ErrorDataReceived += (sender, args) =>
				{
					if (debug)
					{
						Console.WriteLine("Error: {0}", args.Data);
					}
				};
				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				process.WaitForExit();
			});
			thread.IsBackground = true;
			thread.Start();
			return 0;
		}

		private void Log(string workingDirectory, string executable, string arguments)
		{
			Console.WriteLine((workingDirectory == null ? "" : workingDirectory + " | ") + executable + " " + arguments);
		}
	}
}
