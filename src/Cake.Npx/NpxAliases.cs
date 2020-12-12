using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.Npx
{
    [CakeAliasCategory("Npm")]
    public static class NpxAliases
    {
        /// <summary>
        /// Executes npx with the specified command.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="command">The command to execute</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        ///     Npx("semantic-release");
        /// ]]>
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Npx")]
        public static void Npx(
            this ICakeContext context,
            string command)
        =>
            context.Npx(command, new ProcessArgumentBuilder());

        /// <summary>
        /// Executes npx with the specified command and captures the standard output.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="command">The command to execute</param>
        /// <param name="redirectedStandardOutput">The captured standard output</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        ///     string[] redirectedStandardOutput = null;
        ///
        ///     Npx("semantic-release",out redirectedStandardOutput);
        /// ]]>
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Npx")]
        public static void Npx(
            this ICakeContext context,
            string command,
            out string[] redirectedStandardOutput)
        =>
            context.Npx(command, new ProcessArgumentBuilder(), out redirectedStandardOutput);

        /// <summary>
        /// Executes npx with the specified command and additional arguments.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="command">The command to execute</param>
        /// <param name="arguments">Additional arguments to pass to the command</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        ///     Npx("semantic-release", "--dry-run");
        /// ]]>
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Npx")]
        public static void Npx(
            this ICakeContext context,
            string command,
            ProcessArgumentBuilder arguments)
        =>
            context.Npx(new NpxSettings(command, arguments));

        /// <summary>
        /// Executes npx with the specified command and additional arguments and captures the standard output.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="command">The command to execute</param>
        /// <param name="arguments">Additional arguments to pass to the command</param>
        /// <param name="redirectedStandardOutput">The captured standard output</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        ///     string[] redirectedStandardOutput = null;
        ///
        ///     Npx("semantic-release","--dry-run", out redirectedStandardOutput);
        /// ]]>
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Npx")]
        public static void Npx(
            this ICakeContext context,
            string command,
            ProcessArgumentBuilder arguments,
            out string[] redirectedStandardOutput)
        =>
            context.Npx(new NpxSettings(command, arguments), out redirectedStandardOutput);

        /// <summary>
        /// Executes npx with the specified command and additional arguments.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="command">The command to execute</param>
        /// <param name="configureArguments">Action to configure additional arguments to pass to the command</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        ///     Npx("semantic-release", args => args.Append("--dry-run"));
        /// ]]>
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Npx")]
        public static void Npx(
            this ICakeContext context,
            string command,
            Action<ProcessArgumentBuilder> configureArguments)
        {
            var arguments = new ProcessArgumentBuilder();
            configureArguments?.Invoke(arguments);

            context.Npx(command, arguments);
        }

        /// <summary>
        /// Executes npx with the specified command and additional arguments and captures the standard output.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="command">The command to execute</param>
        /// <param name="configureArguments">Action to configure additional arguments to pass to the command</param>
        /// <param name="redirectedStandardOutput">The captured standard output</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        ///     string[] redirectedStandardOutput = null;
        ///
        ///     Npx("semantic-release",
        ///         args => args.Append("--dry-run"),
        ///         out redirectedStandardOutput);
        /// ]]>
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Npx")]
        public static void Npx(
            this ICakeContext context,
            string command,
            Action<ProcessArgumentBuilder> configureArguments,
            out string[] redirectedStandardOutput)
        {
            var arguments = new ProcessArgumentBuilder();
            configureArguments?.Invoke(arguments);

            context.Npx(command, arguments, out redirectedStandardOutput);
        }

        /// <summary>
        /// Executes npx with the specified command and configured settings.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="command">The command to execute</param>
        /// <param name="configureSettings">Action to configure the settings</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        ///     Npx("semantic-release", settings => settings.AddPackage("@semantic-release/git"));
        /// ]]>
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Npx")]
        public static void Npx(
            this ICakeContext context,
            string command,
            Action<NpxSettings> configureSettings)
        =>
            context.Npx(command, default(Action<ProcessArgumentBuilder>), configureSettings);

        /// <summary>
        /// Executes npx with the specified command and configured settings and captures the standard output.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="command">The command to execute</param>
        /// <param name="configureSettings">Action to configure the settings</param>
        /// <param name="redirectedStandardOutput">The captured standard output</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        ///     string[] redirectedStandardOutput = null;
        ///
        ///     Npx("semantic-release",
        ///         settings => settings.AddPackage("@semantic-release/git"),
        ///         out redirectedStandardOutput);
        /// ]]>
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Npx")]
        public static void Npx(
            this ICakeContext context,
            string command,
            Action<NpxSettings> configureSettings,
            out string[] redirectedStandardOutput)
        =>
            context.Npx(
                command,
                default(Action<ProcessArgumentBuilder>),
                configureSettings,
                out redirectedStandardOutput);

        /// <summary>
        /// Executes npx with the specified command, additional arguments and configured settings.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="command">The command to execute</param>
        /// <param name="arguments">Additional arguments to pass to the command</param>
        /// <param name="configureSettings">Action to configure the settings</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        ///     Npx("semantic-release",
        ///         "--dry-run",
        ///         settings => settings.AddPackage("@semantic-release/git"));
        /// ]]>
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Npx")]
        public static void Npx(
            this ICakeContext context,
            string command,
            ProcessArgumentBuilder arguments,
            Action<NpxSettings> configureSettings)
        {
            var npxSettings = new NpxSettings(command, arguments);
            configureSettings?.Invoke(npxSettings);

            context.Npx(npxSettings);
        }

        /// <summary>
        /// Executes npx with the specified command, additional arguments and configured settings and captures the standard output.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="command">The command to execute</param>
        /// <param name="arguments">Additional arguments to pass to the command</param>
        /// <param name="configureSettings">Action to configure the settings</param>
        /// <param name="redirectedStandardOutput">The captured standard output</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        ///     string[] redirectedStandardOutput = null;
        ///
        ///     Npx("semantic-release",
        ///         "--dry-run",
        ///         settings => settings.AddPackage("@semantic-release/git"),
        ///         out redirectedStandardOutput);
        /// ]]>
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Npx")]
        public static void Npx(
            this ICakeContext context,
            string command,
            ProcessArgumentBuilder arguments,
            Action<NpxSettings> configureSettings,
            out string[] redirectedStandardOutput)
        {
            var npxSettings = new NpxSettings(command, arguments);
            configureSettings?.Invoke(npxSettings);

            context.Npx(npxSettings, out redirectedStandardOutput);
        }

        /// <summary>
        /// Executes npx with the specified command, additional arguments and configured settings.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="command">The command to execute</param>
        /// <param name="configureArguments">Action to configure additional arguments to pass to the command</param>
        /// <param name="configureSettings">Action to configure the settings</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        ///     Npx("semantic-release",
        ///         args => args.Append("--dry-run"),
        ///         settings => settings.AddPackage("@semantic-release/git"));
        /// ]]>
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Npx")]
        public static void Npx(
            this ICakeContext context,
            string command,
            Action<ProcessArgumentBuilder> configureArguments,
            Action<NpxSettings> configureSettings)
        {
            var arguments = new ProcessArgumentBuilder();
            configureArguments?.Invoke(arguments);

            context.Npx(command, arguments, configureSettings);
        }

        /// <summary>
        /// Executes npx with the specified command, additional arguments and configured settings and captures the standard output.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="command">The command to execute</param>
        /// <param name="configureArguments">Action to configure additional arguments to pass to the command</param>
        /// <param name="configureSettings">Action to configure the settings</param>
        /// <param name="redirectedStandardOutput">The captured standard output</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        ///     string[] redirectedStandardOutput = null;
        ///
        ///     Npx("semantic-release",
        ///         args => args.Append("--dry-run"),
        ///         settings => settings.AddPackage("@semantic-release/git"),
        ///         out redirectedStandardOutput);
        /// ]]>
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Npx")]
        public static void Npx(
            this ICakeContext context,
            string command,
            Action<ProcessArgumentBuilder> configureArguments,
            Action<NpxSettings> configureSettings,
            out string[] redirectedStandardOutput)
        {
            var arguments = new ProcessArgumentBuilder();
            configureArguments?.Invoke(arguments);

            context.Npx(command, arguments, configureSettings, out redirectedStandardOutput);
        }

        private static void Npx(this ICakeContext context, NpxSettings npxSettings)
        {
            var npxTool = new NpxTool(
                context.FileSystem,
                context.Environment,
                context.ProcessRunner,
                context.Tools,
                context.Log);

            npxTool.Execute(npxSettings);
        }

        private static void Npx(
            this ICakeContext context,
            NpxSettings npxSettings,
            out string[] redirectedStandardOutput)
        {
            var npxTool = new NpxTool(
                context.FileSystem,
                context.Environment,
                context.ProcessRunner,
                context.Tools,
                context.Log);

            npxTool.Execute(npxSettings, out redirectedStandardOutput);
        }
    }
}