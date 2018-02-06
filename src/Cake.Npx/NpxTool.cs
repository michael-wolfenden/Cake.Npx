using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Npx
{
    /// <summary>
    /// Tool for executing npx.
    /// </summary>
    public class NpxTool : Tool<NpxSettings>
    {
        private readonly ICakeLog _cakeLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpxTool" /> class.
        /// </summary>
        /// <param name="fileSystem">The file system</param>
        /// <param name="environment">The environment</param>
        /// <param name="processRunner">The process runner</param>
        /// <param name="tools">The tool locator</param>
        /// <param name="cakeLog">The log instance</param>
        public NpxTool(
            IFileSystem fileSystem,
            ICakeEnvironment environment,
            IProcessRunner processRunner,
            IToolLocator tools,
            ICakeLog cakeLog) : base(fileSystem, environment, processRunner, tools)
        {
            _cakeLog = cakeLog;
        }

        /// <summary>
        /// Gets the name of the tool
        /// </summary>
        /// <returns>the name of the tool</returns>
        protected override string GetToolName() => "Npx";

        /// <summary>
        /// Gets the name of the tool executable
        /// </summary>
        /// <returns>The tool executable name</returns>
        protected override IEnumerable<string> GetToolExecutableNames() => new[]
        {
            "npx.cmd",
            "npx"
        };

        /// <summary>
        /// Runs npx.
        /// </summary>
        /// <param name="settings">The settings</param>
        /// <param name="redirectedStandardOutput">The captured standard output</param>
        public void Execute(
            NpxSettings settings,
            out string[] redirectedStandardOutput)
        {
            string[] standardOutput = null;

            var arguments = GetArguments(settings);

            var processSettings = new ProcessSettings
            {
                RedirectStandardOutput = true
            };

            Action<IProcess> postAction = process =>
            {
                standardOutput = process.GetStandardOutput().ToArray();

                // even though we are redirecting standard out, if the
                // process exited with an error, we log standard out
                // for easier debugging as to why to the process failed
                if (process.GetExitCode() != 0)
                {
                    _cakeLog.Information(string.Join(Environment.NewLine, standardOutput));
                }
            };

            Run(settings, arguments, processSettings, postAction);

            redirectedStandardOutput = standardOutput;
        }

        /// <summary>
        /// Runs npx.
        /// </summary>
        /// <param name="settings">The settings</param>
        public void Execute(NpxSettings settings)
        {
            Run(settings, GetArguments(settings));
        }

        /// <summary>
        /// Builds the arguments for npx.
        /// </summary>
        /// <param name="settings">Settings used for building the arguments.</param>
        /// <returns>Argument builder containing the arguments based on <paramref name="settings"/>.</returns>
        private static ProcessArgumentBuilder GetArguments(NpxSettings settings)
        {
            var arguments = new ProcessArgumentBuilder();
            settings.Evaluate(arguments);

            return arguments;
        }
    }
}