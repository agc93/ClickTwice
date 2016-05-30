using System;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Testing;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Loggers;
using Xunit;
using Xunit.Abstractions;

namespace Cake.ClickTwice.Tests
{
    public class UnitTest1
    {
        public UnitTest1(ITestOutputHelper helper)
        {
            Output = helper;
        }

        private ITestOutputHelper Output { get; set; }

        [Fact]
        public void CakeAliasTest()
        {
            var fixture = new ClickTwiceFixture(Output);
            var mgr =
                fixture.Run(
                    @"C:\Users\alist\Source\ACN\myTaxFramework\FormDocuments\DocumentConversion\DocumentConversion.csproj");
            mgr.BuildAction = manager =>
            {
                foreach (var logger in manager.Loggers)
                {
                    logger.Log("Building project...");
                }
            };
            mgr.SetConfiguration("Debug")
                .ThrowOnHandlerFailure()
                .ForceRebuild()
                .WithHandler(new AppInfoHandler(new AppInfoManager()))
                .PublishTo(fixture.FileSystem.GetDirectory("./artifacts").Path);

        }
    }

    public class ClickTwiceFixture
    {
        public ClickTwiceFixture(ITestOutputHelper output)
        {
            var environment = FakeEnvironment.CreateWindowsEnvironment();
            Environment = environment;
            var fileSystem = new FakeFileSystem(environment);
            fileSystem.CreateDirectory("./artifacts");
            //fileSystem.CreateFile(PackageFilePath.FullPath).SetContent(SamplePackageJson);
            FileSystem = fileSystem;
            Logger = new UnitTestLogger(output);
        }

        public IFileSystem FileSystem { get; set; }

        public ICakeEnvironment Environment { get; set; }
        private ICakeLog Log { get; set; } = new FakeLog();
        private IPublishLogger Logger { get; }

        public ClickTwiceManager Run(FilePath projectFile)
        {
            var manager = new ClickTwiceManager(projectFile.FullPath, Log, Environment, FileSystem,
                new ProcessRunner(Environment, Log), new Globber(FileSystem, Environment));
            manager.LogTo(Logger);
            return manager;
        }
    }

    public class UnitTestLogger : IPublishLogger
    {
        public UnitTestLogger(ITestOutputHelper helper)
        {
            Logger = helper;
            IncludeBuildMessages = true;
        }

        private ITestOutputHelper Logger { get; set; }

        public void Log(string content)
        {
            Logger.WriteLine(content);
        }

        public bool IncludeBuildMessages { get; set; }
        public string Close(string outputPath)
        {
            throw new NotImplementedException();
        }
    }
}
