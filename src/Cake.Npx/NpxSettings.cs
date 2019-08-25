using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Npx
{
    /// <summary>
    /// Npx tool settings.
    /// </summary>
    public class NpxSettings : ToolSettings
    {
        [Obsolete("Only need for ToolFixture constraint")]
        public NpxSettings() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpxSettings"/> class.
        /// </summary>
        /// <param name="command">Command to run</param>
        /// <param name="additionalArguments">Additional arguments to pass to the command</param>
        public NpxSettings(string command, ProcessArgumentBuilder additionalArguments)
        {
            Command = command;
            AdditionalArguments = additionalArguments;
            Packages = new HashSet<string>();
        }

        /// <summary>
        /// Gets the command to be run.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Get the additional arguments to pass to the command.
        /// </summary>
        public ProcessArgumentBuilder AdditionalArguments { get; }

        /// <summary>
        /// Gets the list of packages which should be installed.
        /// </summary>
        public HashSet<string> Packages { get; }

        /// <summary>
        /// Instruct npx not to look in $PATH, or in the current package's node_modules/.bin for an
        /// existing version before deciding whether to install
        /// </summary>
        public bool IgnoreExisting { get; private set; }

        /// <summary>
        /// Suppressed any output from npx itself (progress bars, error messages, install reports).
        /// Subcommand output itself will not be silenced.
        /// </summary>
        public bool IsQuiet { get; private set; }

        /// <summary>
        /// Install a package by name.
        /// </summary>
        /// <param name="packageName">Name of the package.</param>
        /// <returns>The settings instance with <paramref name="packageName"/> added to <see cref="NpxSettings.Packages"/>.</returns>
        public NpxSettings AddPackage(string packageName)
        {
            if (string.IsNullOrWhiteSpace(packageName))
                throw new ArgumentException("Package name cannot be null or empty.", nameof(packageName));

            Packages.Add(packageName);

            return this;
        }

        /// <summary>
        /// Instruct npx not to look in $PATH, or in the current package's node_modules/.bin for an
        /// existing version before deciding whether to install
        /// </summary>
        /// <returns>The settings instance with ignore existing set.</returns>
        public NpxSettings IgnoringExisting()
        {
            IgnoreExisting = true;
            return this;
        }

        /// <summary>
        /// Suppressed any output from npx itself (progress bars, error messages, install reports).
        /// Subcommand output itself will not be silenced.
        /// </summary>
        /// <returns>The settings instance with quiet set.</returns>
        public NpxSettings Quiet()
        {
            IsQuiet = true;
            return this;
        }

        /// <summary>
        /// Evaluates the settings and writes them to <paramref name="arguments"/>.
        /// </summary>
        /// <param name="arguments">The argument builder into which the settings should be written.</param>
        public void Evaluate(ProcessArgumentBuilder arguments)
        {
            if (IsQuiet)
            {
                arguments.Append("--quiet");
            }

            foreach (var package in Packages)
            {
                arguments.AppendSwitchQuoted("-p", package);
            }

            if (IgnoreExisting)
            {
                arguments.Append("--ignore-existing");
            }

            arguments.Append(Command);

            if (!AdditionalArguments.IsNullOrEmpty())
            {
                arguments.Append(AdditionalArguments.Render());
            }
        }
    }
}