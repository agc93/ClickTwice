using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;
using Cake.Testing;
using ClickTwice.Publisher.Core.Loggers;
using Xunit.Abstractions;

namespace Cake.ClickTwice.Tests
{
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
                new ProcessRunner(Environment, Log), new ToolLocator(Environment, new ToolRepository(Environment), new ToolResolutionStrategy(FileSystem, Environment, new Globber(FileSystem, Environment), new FakeConfiguration())));
            manager.LogTo(Logger);
            return manager;
        }
    }
}