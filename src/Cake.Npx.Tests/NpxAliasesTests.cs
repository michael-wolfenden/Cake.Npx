using System;
using System.Linq;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Testing;
using FluentAssertions;
using Xunit;

namespace Cake.Npx.Tests
{
    public class NpxAliasesTests
    {
        [Fact]
        public void Should_add_command()
        {
            var result = new NpxFixture().Execute(context =>
            {
                context.Npx("command");
            });

            result.Args.Should().Be("command");
        }

        [Fact]
        public void Should_add_additional_arguments()
        {
            var result = new NpxFixture().Execute(context =>
            {
                context.Npx("command", "--argument");
            });

            result.Args.Should().Be("command --argument");
        }

        [Fact]
        public void Should_add_additional_arguments_when_provided_fluently()
        {
            var result = new NpxFixture().Execute(context =>
            {
                context.Npx("command", args => args
                    .Append("--argument")
                    .AppendSwitchQuoted("-switch", "switch argument"));
            });

            result.Args.Should().Be("command --argument -switch \"switch argument\"");
        }

        [Fact]
        public void Should_add_distinct_packages_to_arguments()
        {
            var result = new NpxFixture().Execute(context =>
            {
                context.Npx("command", settings => settings
                    .AddPackage("a package")
                    .AddPackage("a package")
                    .AddPackage("another package"));
            });

            result.Args.Should().Be("-p \"a package\" -p \"another package\" command");
        }

        [Fact]
        public void Should_add_distinct_packages_and_additional_arguments_to_arguments()
        {
            var result = new NpxFixture().Execute(context =>
            {
                context.Npx("command", "--argument", settings => settings
                    .AddPackage("a package")
                    .AddPackage("a package")
                    .AddPackage("another package"));
            });

            result.Args.Should().Be("-p \"a package\" -p \"another package\" command --argument");
        }

        [Fact]
        public void Should_add_distinct_packages_and_additional_fluent_arguments_to_arguments()
        {
            var result = new NpxFixture().Execute(context =>
            {
                context.Npx(
                    "command",
                    args => args.Append("--argument"),
                    settings => settings
                        .AddPackage("a package")
                        .AddPackage("a package")
                        .AddPackage("another package")
                );
            });

            result.Args.Should().Be("-p \"a package\" -p \"another package\" command --argument");
        }

        [Fact]
        public void Should_add_ignore_existing_flag_to_arguments()
        {
            var result = new NpxFixture().Execute(context =>
            {
                context.Npx(
                    "command",
                    settings => settings.IgnoringExisting()
                );
            });

            result.Args.Should().Be("--ignore-existing command");
        }

        [Fact]
        public void Should_add_quiet_flag_to_arguments()
        {
            var result = new NpxFixture().Execute(context =>
            {
                context.Npx(
                    "command",
                    settings => settings.Quiet()
                );
            });

            result.Args.Should().Be("--quiet command");
        }

        [Fact]
        public void Should_add_all_arguments()
        {
            var result = new NpxFixture().Execute(context =>
            {
                context.Npx(
                    "command",
                    args => args.Append("--argument"),
                    settings => settings
                        .AddPackage("a package")
                        .AddPackage("a package")
                        .AddPackage("another package")
                        .IgnoringExisting()
                        .Quiet()
                );
            });

            result.Args.Should().Be("--quiet -p \"a package\" -p \"another package\" --ignore-existing command --argument");
        }

        [Fact]
        public void Should_redirect_standard_output()
        {
            string[] actualStandardOutput = null;

            new NpxFixture()
                .SetStandardOutput(new[]
                {
                    "standard output line 1",
                    "standard output line 2"
                })
                .Execute(context =>
                {
                    context.Npx("command", out actualStandardOutput);
                });

            actualStandardOutput.Should().BeEquivalentTo(new[]
            {
                "standard output line 1",
                "standard output line 2"
            });
        }

        [Fact]
        public void Should_log_standard_output_to_information_when_process_fails()
        {
            var fixture = new NpxFixture()
                .SetStandardOutput(new[]
                {
                    "standard output line 1",
                    "standard output line 2"
                })
                .SetExitCode(1);

            try
            {
                // throws due to exit code being 1
                fixture.Execute(context => context.Npx("semantic-release", out _));
            }
            catch { }

            var logEntry = fixture.Log.Entries.Single();

            logEntry
                .Should()
                .BeEquivalentTo(new FakeLogMessage(
                    Verbosity.Normal,
                    LogLevel.Information,
                    $"standard output line 1{Environment.NewLine}standard output line 2"
                ));
        }
    }
}