using System;
using Cake.Core;
using Cake.Core.IO;
using Cake.Testing;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Handlers;
using Xunit;

namespace Cake.ClickTwice.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void CakeAliasTest()
        {
            var fixture = new ClickTwiceFixture();
            var mgr =
                fixture.Run(
                    @"C:\Users\alist\Source\ACN\myTaxFramework\FormDocuments\DocumentConversion\DocumentConversion.csproj");
            mgr.SetConfiguration("Debug")
                .ThrowOnHandlerFailure()
                .ForceRebuild()
                .WithHandler(new AppInfoHandler(new AppInfoManager()))
                .PublishTo(fixture.FileSystem.GetDirectory("./artifacts").Path);

        }
    }

    public class ClickTwiceFixture
    {
        public ClickTwiceFixture()
        {
            var environment = FakeEnvironment.CreateUnixEnvironment();
            Environment = environment;
            var fileSystem = new FakeFileSystem(environment);
            fileSystem.CreateDirectory("./artifacts");
            //fileSystem.CreateFile(PackageFilePath.FullPath).SetContent(SamplePackageJson);
            FileSystem = fileSystem;
        }

        public IFileSystem FileSystem { get; set; }

        public ICakeEnvironment Environment { get; set; }

        public ClickTwiceManager Run(FilePath projectFile)
        {
            var manager = new ClickTwiceManager(projectFile, new FakeLog());
            return manager;
        }
    }
}
