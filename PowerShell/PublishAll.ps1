# Use command line toggle --incremental to skip deleting publish folder and skip creating archive, thus making build faster.
$PrevPath = Get-Location

Write-Host "Publish for Final Packaging build."
Set-Location $PSScriptRoot

$PublishFolder = "$PSScriptRoot\..\Publish"
$WindowsPublishFolder = "$PublishFolder\Windows"
$LibraryPublishFolder = "$PublishFolder\Libraries"
$NugetPublishFolder = "$PublishFolder\Nugets"

# Delete current data
if ($Args[0] -ne '--incremental') {
    Remove-Item $PublishFolder -Recurse -Force
}

# Publish Executables
$PublishExecutables = @(
    "Frontends\Pure\Pure.csproj"
)
foreach ($Item in $PublishExecutables)
{
    dotnet publish $PSScriptRoot\..\$Item --use-current-runtime --output $PublishFolder
}
# Publish Windows-only Executables
$PublishWindowsExecutables = @(
    "Frontends\Notebook\Notebook.csproj"
)
foreach ($Item in $PublishWindowsExecutables)
{
    # In .Net 8, those windows-specific builds might interfere with other non-windows build
    dotnet publish $PSScriptRoot\..\$Item --runtime win-x64 --self-contained --output $WindowsPublishFolder
}
# Publish Loose Libraries
$PublishLibraries = @(
    "StandardLibraries\Vector\Vector.csproj"
    "StandardLibraries\Python\Python.csproj"
    "StandardLibraries\ODBC\ODBC.csproj"
    "StandardLibraries\Pipeline\Pipeline.csproj"
    "StandardLibraries\Razor\Razor.csproj"
    "StandardLibraries\CentralSnippets\CentralSnippets.csproj"
    "StandardLibraries\CLI\CLI.csproj"
    "StandardLibraries\Plot\Plot.csproj"
)
foreach ($Item in $PublishLibraries)
{
    dotnet publish $PSScriptRoot\..\$Item --use-current-runtime --output $LibraryPublishFolder
}
# Publish Windows-only Library Components (executable)
$PublishWindowsLibraryComponents = @(
    "StandardLibraries\PlotWindow\PlotWindow.csproj"
)
foreach ($Item in $PublishWindowsLibraryComponents)
{
    dotnet publish $PSScriptRoot\..\$Item --runtime win-x64 --self-contained --output $WindowsPublishFolder
}
# Publish Nugets
$PublishNugets = @(
    "Core\Core.csproj"

    "StandardLibraries\Vector\Vector.csproj"
    "StandardLibraries\Python\Python.csproj"
    "StandardLibraries\ODBC\ODBC.csproj"
    "StandardLibraries\Pipeline\Pipeline.csproj"
    "StandardLibraries\Razor\Razor.csproj"
    "StandardLibraries\CentralSnippets\CentralSnippets.csproj"
    "StandardLibraries\CLI\CLI.csproj"
)
foreach ($Item in $PublishNugets)
{
    dotnet pack $PSScriptRoot\..\$Item --output $NugetPublishFolder
}

# Validation
$pureExePath = Join-Path $PublishFolder "Pure.exe"
if (-Not (Test-Path $pureExePath))
{
    Write-Host "Build failed."
    Exit
}

# Generate Documentation
& $pureExePath $PSScriptRoot\GenerateDocumentation.cs $LibraryPublishFolder $PublishFolder\APIDoc.md

# Create archive
if ($Args[0] -ne '--incremental') {
    $Date = Get-Date -Format yyyyMMdd
    $ArchiveFolder = "$PublishFolder\..\Packages"
    $ArchivePath = "$ArchiveFolder\Pure_DistributionBuild_B$Date.zip"
    New-Item -ItemType Directory -Force -Path $ArchiveFolder
    Compress-Archive -Path $PublishFolder\* -DestinationPath $ArchivePath -Force
}

Set-Location $PrevPath