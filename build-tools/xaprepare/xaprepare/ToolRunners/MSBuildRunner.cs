using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Xamarin.Android.Prepare
{
	partial class MSBuildRunner : ToolRunner
	{
		protected override string DefaultToolExecutableName => "dotnet";
		protected override string ToolName                  => "dotnet";

		public List<string> StandardArguments { get; }

		public MSBuildRunner (Context context, Log? log = null, string? msbuildPath = null)
			: base (context, log, msbuildPath)
		{
			ProcessTimeout = TimeSpan.FromMinutes (30);

			StandardArguments = new List<string> {
				"build", // dotnet build includes implicit restore
				$"-p:Configuration={context.Configuration}",
			};
		}

		public async Task<bool> Run (string projectPath, string logTag, List<string>? arguments = null, string? binlogName = null, string? workingDirectory = null)
		{
			if (String.IsNullOrEmpty (logTag))
				throw new ArgumentException ("must not be null or empty", nameof (logTag));

			if (String.IsNullOrEmpty (workingDirectory))
				workingDirectory = BuildPaths.XamarinAndroidSourceRoot;

			ProcessRunner runner = CreateProcessRunner ();

			AddArguments (runner, StandardArguments);
			if (!String.IsNullOrEmpty (binlogName)) {
				string logPath = Utilities.GetRelativePath (workingDirectory!, Path.Combine (Configurables.Paths.BuildBinDir, $"msbuild-{Context.BuildTimeStamp}-{binlogName}.binlog"));
				runner.AddArgument ("-v:n");
				runner.AddQuotedArgument ($"-bl:{logPath}");
			}
			AddArguments (runner, arguments);
			runner.AddQuotedArgument (Utilities.GetRelativePath (workingDirectory!, projectPath));

			string message = GetLogMessage (runner);
			Log.Info (message, CommandMessageColor);
			Log.StatusLine ();

			try {
				return await RunTool (
					() => {
						using (var outputSink = (OutputSink)SetupOutputSink (runner, $"msbuild.{logTag}")) {
							runner.WorkingDirectory = workingDirectory;
							StartTwiddler ();
							return runner.Run ();
						}
					}
				);
			} finally {
				StopTwiddler ();
			}
		}

		public async Task<bool> Restore (string projectPath, string logTag, List<string>? arguments = null, string? binlogName = null, string? workingDirectory = null)
		{
			var args = new List<string> { "-t:Restore" };
			if (arguments != null)
				args.AddRange (arguments);

			return await Run (
				projectPath: projectPath,
				logTag: logTag,
				arguments: args,
				binlogName: binlogName,
				workingDirectory: workingDirectory
			);
		}

		protected override TextWriter CreateLogSink (string? logFilePath)
		{
			return new OutputSink (Log, logFilePath);
		}
	}
}
