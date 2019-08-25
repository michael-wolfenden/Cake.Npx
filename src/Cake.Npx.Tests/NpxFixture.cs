using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.IO;
using Cake.Testing;
using Cake.Testing.Fixtures;
using NSubstitute;

namespace Cake.Npx.Tests
{
    public class NpxFixture : ToolFixture<NpxSettings>
    {
        public FakeLog Log { get; } = new FakeLog();

        public NpxFixture() : base("npx")
        {
        }

        public NpxFixture SetStandardOutput(IEnumerable<string> standardOutput)
        {
            ProcessRunner.Process.SetStandardOutput(standardOutput);
            return this;
        }

        public NpxFixture SetExitCode(int exitCode)
        {
            ProcessRunner.Process.SetExitCode(exitCode);
            return this;
        }

        public ToolFixtureResult Execute(Action<CakeContext> action)
        {
            var context = new CakeContext(
                FileSystem,
                Environment,
                Globber,
                Log,
                Substitute.For<ICakeArguments>(),
                ProcessRunner,
                Substitute.For<IRegistry>(),
                Tools,
                Substitute.For<ICakeDataService>(),
                Substitute.For<ICakeConfiguration>()
            );

            action(context);

            return ProcessRunner.Results.LastOrDefault();
        }

        protected override void RunTool() =>
            throw new NotImplementedException();
    }
}