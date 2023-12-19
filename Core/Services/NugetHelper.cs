using Microsoft.Extensions.DependencyModel;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Diagnostics;

namespace Core.Services
{
    #region Placeholder Solution
    /// <remarks>
    /// Before the formal implementation is complete, here we provide an easy win-x64 use only implementation that makes use of `dotnet` cli;
    /// This relies on `pwsh` (PowerShell 7)
    /// </remarks>
    internal static class QuickEasyDirtyNugetPreparer
    {
        public static string TryDownloadNugetPackage(string packageName, string nugetRepoIdentifier)
        {
            string loadDllAs = null;
            if (packageName.Contains(" as "))
            {
                loadDllAs = packageName[(packageName.IndexOf(" as ") + " as ".Length)..].Trim();
                packageName = packageName[..packageName.IndexOf(" as ")].Trim();
            }
            if (NugetHelper.FindLatestVersion(packageName) == null)
                return null;

            string downloadFolder = Path.Combine(Path.GetTempPath(), "Pure", "NugetDownloads");
            Directory.CreateDirectory(downloadFolder);

            if (nugetRepoIdentifier == null)
                nugetRepoIdentifier = "Default";
            string packageFolder = Path.Combine(downloadFolder, nugetRepoIdentifier, packageName);
            string dllPath = Path.Combine(packageFolder, "CompiledDLLs", $"{SpecialHandlePackages(packageName, loadDllAs)}.dll");
            if (File.Exists(dllPath))
                return dllPath;

            if (Directory.Exists(packageFolder))
                Directory.Delete(packageFolder, true);
            Directory.CreateDirectory(packageFolder);

            string output;
            string errors;
            try
            {
                string scriptPath = Path.Combine(packageFolder, "Script.ps1");
                File.WriteAllText(scriptPath, $"""
                    dotnet new console --language C# --name TempProject --framework net8.0
                    cd TempProject
                    dotnet add package {packageName}
                    dotnet build --runtime win-x64 --no-self-contained --output ..\CompiledDLLs
                    """);
                IssuePowerShellCommands(scriptPath, out output, out errors);
            }
            catch (Exception)
            {
                return null;
            }

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"Cannot locate package DLL - the package is built successfully but the package DLL cannot be found at {dllPath}");
                Console.WriteLine($"Consult folder {Path.Combine(packageFolder, "CompiledDLLs")} for information on entrance dll name and use overload of `Import` to import proper dll.");
                Console.WriteLine("(If the module you are trying to import targets specific platforms e.g. Windows, then the generated .csproj file might be the cause of error)");
                Console.WriteLine("Syntax: `Import(LibraryName as DLLName)`, e.g. `Import(pythonnet as Python.Runtime)`");
                Console.WriteLine($"""
                    This is the output/error from build process:
                    {output}

                    [Errors]
                    {errors}
                    """);
            }
            return dllPath;

            static void IssuePowerShellCommands(string scriptFile, out string output, out string errors)
            {
                scriptFile = Path.GetFullPath(scriptFile);
                Process process = new()
                {
                    StartInfo = new()
                    {
                        FileName = "pwsh.exe",
                        Arguments = Path.GetFileName(scriptFile),
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WorkingDirectory = Path.GetDirectoryName(scriptFile)
                    }
                };
                process.Start();

                output = process.StandardOutput.ReadToEnd();
                errors = process.StandardError.ReadToEnd();
            }
            static string SpecialHandlePackages(string packageName, string loadDllAs)
            {
                if (loadDllAs != null)
                    return loadDllAs;
                else return packageName;
            }
        }
    }
    #endregion

    #region Formal Implementation (Not Complete)
    public class NugetPackageMetadata
    {
        public class NugetPackageDependancyGroup
        {
            public string Name { get; set; }
            public (string Name, string Version)[] Dependencies { get; set; }
        }
        public string ID { get; set; }
        public string Version { get; set; }
        public bool IsListed { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }
        public string Authors { get; set; }
        public NugetPackageDependancyGroup[] DependencyGroups { get; set; }
        public string[] Files { get; set; }

        public NugetPackageMetadata(string iD, string version, bool isListed, string tags, string description)
        {
            ID = iD;
            Version = version;
            IsListed = isListed;
            Tags = tags;
            Description = description;
        }

        public NugetPackageMetadata(string iD, string version, bool isListed, string tags, string description, string authors)
        {
            ID = iD;
            Version = version;
            IsListed = isListed;
            Tags = tags;
            Description = description;
            Authors = authors;
        }
    }
    public static class NugetHelper
    {
        #region Sync
        public static string[] FindVersions(string packageName, bool containsBeta = false)
        {
            return FindVersionsAsync(packageName, containsBeta).Result;
        }
        public static string FindLatestVersion(string packageName, bool containsBeta = false)
        {
            return FindLatestVersionAsync(packageName, containsBeta).Result;
        }
        #endregion

        #region Async
        public static async Task DownloadLatestPackageAsync(string packageName)
        {
            string version = await FindLatestVersionAsync(packageName);
            await DownloadPackageAsync(packageName, version);
        }
        public static async Task<NugetPackageMetadata[]> GetPackageMetadataAsync(string packageName)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceCacheContext cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            PackageMetadataResource resource = await repository.GetResourceAsync<PackageMetadataResource>();

            IEnumerable<IPackageSearchMetadata> packages = await resource.GetMetadataAsync(
                packageName,
                includePrerelease: true,
                includeUnlisted: false,
                cache,
                logger,
                cancellationToken);

            return packages
                .Select(p => new NugetPackageMetadata(packageName, p.Identity.Version.ToString(), p.IsListed, p.Tags, p.Description, p.Authors))
                .ToArray();
        }
        public static async Task DownloadPackageAsync(string packageName, string version)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceCacheContext cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            string packageId = packageName;
            NuGetVersion packageVersion = new NuGetVersion(version);
            using MemoryStream packageStream = new MemoryStream();

            await resource.CopyNupkgToStreamAsync(
                packageId,
                packageVersion,
                packageStream,
                cache,
                logger,
                cancellationToken);

            Console.WriteLine($"Downloaded package {packageId} {packageVersion}");

            using PackageArchiveReader packageReader = new PackageArchiveReader(packageStream);
            NuspecReader nuspecReader = await packageReader.GetNuspecReaderAsync(cancellationToken);

            Console.WriteLine($"Tags: {nuspecReader.GetTags()}");
            Console.WriteLine($"Description: {nuspecReader.GetDescription()}");
        }
        public static async Task<string[]> FindVersionsAsync(string packageName, bool containsBeta = false)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceCacheContext cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            IEnumerable<NuGetVersion> versions = await resource.GetAllVersionsAsync(
                packageName,
                cache,
                logger,
                cancellationToken);

            return versions
                .Where(v => v.IsPrerelease == containsBeta)
                .Select(v => v.OriginalVersion)
                .ToArray();
        }
        public static async Task<string> FindLatestVersionAsync(string packageName, bool containsBeta = false)
            => (await FindVersionsAsync(packageName, containsBeta)).OrderBy(v => v).LastOrDefault();
        #endregion

        #region Helpers
        private static void ReadPackage(string filePath)
        {
            using FileStream inputStream = new FileStream("MyPackage.nupkg", FileMode.Open);
            ReadPackage(inputStream);
        }
        private static void ReadPackage(Stream stream)
        {
            using PackageArchiveReader reader = new PackageArchiveReader(stream);
            NuspecReader nuspec = reader.NuspecReader;
            NugetPackageMetadata metadata = new NugetPackageMetadata(nuspec.GetId(), nuspec.GetVersion().ToString(), false, nuspec.GetTags(), nuspec.GetDescription(), nuspec.GetAuthors());
            if (nuspec.GetDependencyGroups().Count() != 0)
            {
                metadata.DependencyGroups = nuspec
                    .GetDependencyGroups()
                    .Select(g => new NugetPackageMetadata.NugetPackageDependancyGroup()
                    {
                        Name = g.TargetFramework.GetShortFolderName(),
                        Dependencies = g.Packages.Select(p => (p.Id, p.VersionRange.ToString())).ToArray()
                    })
                    .ToArray();
            }
            metadata.Files = reader.GetFiles().ToArray();
        }
        private static bool DependencySuppliedByHost(DependencyContext hostDependencies, PackageDependency dep)
        {
            RuntimeLibrary runtimeLib = hostDependencies.RuntimeLibraries.FirstOrDefault(r => r.Name == dep.Id);

            if (runtimeLib is not null)
            {
                NuGetVersion parsedLibVersion = NuGetVersion.Parse(runtimeLib.Version);

                if (parsedLibVersion.IsPrerelease)
                {
                    // Always use pre-release versions from the host, otherwise it becomes
                    // a nightmare to develop across multiple active versions.
                    return true;
                }
                else
                {
                    // Does the host version satisfy the version range of the requested package?
                    // If so, we can provide it; otherwise, we cannot.
                    return dep.VersionRange.Satisfies(parsedLibVersion);
                }
            }

            return false;
        }
        #endregion
    }
    #endregion
}
