// This code is heavily adapted from similar code shiped as part of
// the Cake.Common library, licensed under the MIT license by 
// the .NET Foundation and contributors. See the LICENSE file in 
// the project root for more information.

using System;
using System.IO;

namespace ClickTwice.Publisher.MSBuild
{
    public static class MSBuildResolver
    {
        public static string GetBuildAssemblyPath(string assemblyName, MSBuildPlatform platform = MSBuildPlatform.Automatic, MSBuildToolVersion version = MSBuildToolVersion.Default, Action<string> logCallback = null)
        {
            logCallback = logCallback ?? Console.WriteLine;
            var binPath = version == MSBuildToolVersion.Default
                ? GetHighestAvailableMSBuildVersion(platform)
                : GetMSBuildPath((MSBuildVersion)version, platform);
            // Get the MSBuild path.
            if (binPath == null || !File.Exists(Path.Combine(binPath, $"{assemblyName}.dll")))
            {
                logCallback?.Invoke($"Could not locate assembly {assemblyName}");
                return null;
            }
            else
            {
                logCallback?.Invoke($"Found {assemblyName} at {binPath}");
                return Path.Combine(binPath, $"{assemblyName}.dll");
            }
        }

        public static string GetMSBuildPath(MSBuildPlatform platform = MSBuildPlatform.Automatic, MSBuildToolVersion version = MSBuildToolVersion.Default)
        {
            var binPath = version == MSBuildToolVersion.Default
                ? GetHighestAvailableMSBuildVersion(platform)
                : GetMSBuildPath((MSBuildVersion)version, platform);

            if (binPath == null)
            {
                throw new Exception("Could not resolve MSBuild.");
            }

            // Get the MSBuild path.
            return Path.Combine(binPath, "msbuild.exe");
        }

        private static string GetHighestAvailableMSBuildVersion(MSBuildPlatform buildPlatform)
        {
            var versions = new[]
            {
                MSBuildVersion.MSBuild14,
                MSBuildVersion.MSBuild12,
                MSBuildVersion.MSBuild4,
                MSBuildVersion.MSBuild35,
                MSBuildVersion.MSBuild20
            };

            foreach (var version in versions)
            {
                var path = GetMSBuildPath(version, buildPlatform);
                if (Directory.Exists(path))
                {
                    return path;
                }
            }
            return null;
        }

        private static string GetMSBuildPath(MSBuildVersion version, MSBuildPlatform platform)
        {
            switch (version)
            {
                case MSBuildVersion.MSBuild14:
                    return GetVisualStudioPath(platform, "14.0");
                case MSBuildVersion.MSBuild12:
                    return GetVisualStudioPath(platform, "12.0");
                case MSBuildVersion.MSBuild4:
                    return GetFrameworkPath(platform, "v4.0.30319");
                case MSBuildVersion.MSBuild35:
                    return GetFrameworkPath(platform, "v3.5");
                case MSBuildVersion.MSBuild20:
                    return GetFrameworkPath(platform, "v2.0.50727");
                default:
                    return null;
            }
        }

        private static string GetVisualStudioPath(MSBuildPlatform buildPlatform, string version)
        {
            // Get the bin path.
            var programFilesPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86);
            var binPath = Path.Combine(programFilesPath, "MSBuild", version, "Bin");
            if (buildPlatform == MSBuildPlatform.Automatic)
            {
                if (System.Environment.Is64BitOperatingSystem)
                {
                    binPath = Path.Combine(binPath, "amd64");
                }
            }
            if (buildPlatform == MSBuildPlatform.x64)
            {
                binPath = Path.Combine(binPath, "amd64");
            }
            return binPath;
        }

        private static string GetFrameworkPath(MSBuildPlatform buildPlatform, string version)
        {
            // Get the Microsoft .NET folder.
            var windowsFolder = Environment.GetFolderPath(System.Environment.SpecialFolder.Windows);
            var netFolder = Path.Combine(windowsFolder, "Microsoft.NET");

            if (buildPlatform == MSBuildPlatform.Automatic)
            {
                // Get the framework folder.
                var is64Bit = System.Environment.Is64BitOperatingSystem;
                var frameWorkFolder = is64Bit ? Path.Combine(netFolder, "Framework64") : Path.Combine(netFolder, "Framework");
                return Path.Combine(frameWorkFolder, version);
            }

            if (buildPlatform == MSBuildPlatform.x86)
            {
                return Path.Combine(netFolder, "Framework", version);
            }

            if (buildPlatform == MSBuildPlatform.x64)
            {
                return Path.Combine(netFolder, "Framework64", version);
            }

            throw new NotSupportedException();
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ClickTwice.Publisher.MSBuild
{
    /// <summary>
    /// Represents an MSBuild exe platform.
    /// </summary>
    public enum MSBuildPlatform
    {
        /// <summary>
        /// Will build using MSBuild version based on PlatformTarget/Host OS.
        /// </summary>
        Automatic = 0,

        /// <summary>
        /// MSBuildPlatform: <c>x86</c>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        x86 = 1,

        /// <summary>
        /// MSBuildPlatform: <c>x64</c>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        x64 = 2
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ClickTwice.Publisher.MSBuild
{
    /// <summary>
    /// Represents a MSBuild tool version.
    /// </summary>
    public enum MSBuildToolVersion
    {
        /// <summary>
        /// The highest available MSBuild tool version.
        /// </summary>
        Default = 0,

        /// <summary>
        /// MSBuild tool version: <c>.NET 2.0</c>
        /// </summary>
        NET20 = 1,

        /// <summary>
        /// MSBuild tool version: <c>.NET 3.0</c>
        /// </summary>
        NET30 = 1,

        /// <summary>
        /// MSBuild tool version: <c>Visual Studio 2005</c>
        /// </summary>
        VS2005 = 1,

        /// <summary>
        /// MSBuild tool version: <c>.NET 3.5</c>
        /// </summary>
        NET35 = 2,

        /// <summary>
        /// MSBuild tool version: <c>Visual Studio 2008</c>
        /// </summary>
        VS2008 = 2,

        /// <summary>
        /// MSBuild tool version: <c>.NET 4.0</c>
        /// </summary>
        NET40 = 3,

        /// <summary>
        /// MSBuild tool version: <c>.NET 4.5</c>
        /// </summary>
        NET45 = 3,

        /// <summary>
        /// MSBuild tool version: <c>Visual Studio 2010</c>
        /// </summary>
        VS2010 = 3,

        /// <summary>
        /// MSBuild tool version: <c>Visual Studio 2011</c>
        /// </summary>
        VS2011 = 3,

        /// <summary>
        /// MSBuild tool version: <c>Visual Studio 2012</c>
        /// </summary>
        VS2012 = 3,

        /// <summary>
        /// MSBuild tool version: <c>.NET 4.5.1</c>
        /// </summary>
        NET451 = 4,

        /// <summary>
        /// MSBuild tool version: <c>.NET 4.5.2</c>
        /// </summary>
        NET452 = 4,

        /// <summary>
        /// MSBuild tool version: <c>Visual Studio 2013</c>
        /// </summary>
        VS2013 = 4,

        /// <summary>
        /// MSBuild tool version: <c>Visual Studio 2015</c>
        /// </summary>
        VS2015 = 5,

        /// <summary>
        /// MSBuild tool version: <c>.NET 4.6</c>
        /// </summary>
        NET46 = 5,
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ClickTwice.Publisher.MSBuild
{
    internal enum MSBuildVersion
    {
        MSBuild20 = 1,
        MSBuild35 = 2,
        MSBuild4 = 3,
        MSBuild12 = 4,
        MSBuild14 = 5
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ClickTwice.Publisher.MSBuild
{
    /// <summary>
    /// Represents a MSBuild platform target.
    /// </summary>
    public enum PlatformTarget
    {
        /// <summary>
        /// Platform target: <c>MSIL</c> (Any CPU)
        /// </summary>
        MSIL = 0,

        /// <summary>
        /// Platform target: <c>x86</c>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        x86 = 1,

        /// <summary>
        /// Platform target: <c>x64</c>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        x64 = 2,

        /// <summary>
        /// Platform target: <c>ARM</c>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        ARM = 3,

        /// <summary>
        /// Platform target: <c>Win32</c>
        /// </summary>
        Win32 = 4
    }
}