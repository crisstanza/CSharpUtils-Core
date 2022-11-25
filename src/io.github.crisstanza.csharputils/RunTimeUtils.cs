using System;
using System.Diagnostics;
using System.Threading;

namespace io.github.crisstanza.csharputils
{
	public class RunTimeUtils
	{
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
			Log(workingDirectory, executable, arguments);
			Process process = new Process();
			process.StartInfo.FileName = executable;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			if (workingDirectory != null)
			{
				process.StartInfo.WorkingDirectory = workingDirectory;
			}
			process.Start();
			process.WaitForExit();
			if (process.ExitCode == 0)
			{
				string commandOutput = process.StandardOutput.ReadToEnd();
				return new ExecResult()
				{
					ExitCode = process.ExitCode,
					Output = commandOutput
				};
			}
			else
			{
				string commandErrorOutput = process.StandardError.ReadToEnd();
				return new ExecResult()
				{
					ExitCode = process.ExitCode,
					Output = commandErrorOutput
				};
			}
		}

		public int Run(string executable, string arguments)
		{
			Log(null, executable, arguments);
			Thread thread = new Thread(() =>
			{
				Process process = new Process();
				process.StartInfo.FileName = executable;
				process.StartInfo.Arguments = arguments;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.OutputDataReceived += (sender, args) => Console.WriteLine("Output: {0}", args.Data);
				process.ErrorDataReceived += (sender, args) => Console.WriteLine("Error: {0}", args.Data);
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
