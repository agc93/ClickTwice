using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTwice.Publisher.Core;
using ClickTwice.Publisher.Core.Exceptions;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.MSBuild.Loggers;
using Microsoft.Build.Framework;

namespace ClickTwice.Publisher.MSBuild
{
    public class DirectPublisher : BasePublishManager
    {
        private readonly string _msBuildPath;

        public DirectPublisher(string projectFilePath, InformationSource source = InformationSource.Both) : base(projectFilePath)
        {
            _msBuildPath = MSBuildResolver.GetMSBuildPath();
        }

        private ConcurrentQueue<string> OutputQueue { get; set; } = new ConcurrentQueue<string>();

        protected override bool BuildProject(Dictionary<string, string> props, List<string> targets)
        {
            Log("Preparing MSBuild");
            var args = new ArgBuilder();
            args.Append(string.Join(Environment.NewLine, props.Select(p => $"/p:{p.Key}={p.Value}")))
                .Append($"/t:{string.Join(",", targets)}")
                .Append(ProjectFilePath);
            bool success;
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo(_msBuildPath, args.ToString())
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = Environment.CurrentDirectory
                }
            };
            proc.OutputDataReceived += Proc_OutputDataReceived;
            proc.OutputDataReceived += TryWriteOutput;
            proc.ErrorDataReceived += Proc_OutputDataReceived;
            try
            {
                Log("Invoking MSBuild process");
                proc.Start();
                proc.BeginOutputReadLine();
                proc.WaitForExit();
                Log($"Process completed - {proc.ExitCode}");
                WriteAllOutput();
                success = proc.ExitCode == 0;
            }
            catch (Exception ex)
            {
                Log($"{ex.GetType()} - {ex.Message}");
                Log($"Failed to invoke MSBuild from {_msBuildPath}");
                Log(proc.StandardError.ReadToEnd());
                success = false;
            }
            return success;
        }

        private void WriteAllOutput()
        {
            while (!OutputQueue.IsEmpty)
            {
                string item;
                if (OutputQueue.TryDequeue(out item)) Log(item, true);
            }
        }

        private void TryWriteOutput(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            if (OutputQueue.IsEmpty) return;
            string item;
            if (OutputQueue.TryDequeue(out item)) Log(item, true);
        }

        protected override void PostBuild(FileSystemInfo targetPath)
        {
            base.PostBuild(targetPath);
            var m = PrepareManifestManager(targetPath.FullName);
            m?.DeployManifest(m.CreateAppManifest());
        }

        private ManifestManager PrepareManifestManager(string targetPath)
        {
            return !string.IsNullOrWhiteSpace(targetPath) ? new ManifestManager(ProjectFilePath, targetPath) : null;
        }

        private void Proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null) OutputQueue.Enqueue(e.Data);
        }
    }
}
