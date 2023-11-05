$PrevPath = Get-Location

Write-Host "Publish for Final Packaging build."
Set-Location $PSScriptRoot

$PublishFolder = "$PSScriptRoot\..\Publish"
$LibraryPublishFolder = "$PublishFolder\Libraries"
$NugetPublishFolder = "$PublishFolder\Nugets"

# Delete current data
Remove-Item $PublishFolder -Recurse -Force

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
    dotnet publish $PSScriptRoot\..\$Item --runtime win-x64 --self-contained --output $PublishFolder
}
# Publish Loose Libraries
$PublishLibraries = @(
    "StandardLibraries\Python\Python.csproj"
    "StandardLibraries\ODBC\ODBC.csproj"
    "StandardLibraries\Pipeline\Pipeline.csproj"
    "StandardLibraries\Razor\Razor.csproj"
    "StandardLibraries\CentralSnippets\CentralSnippets.csproj"
    "StandardLibraries\CLI\CLI.csproj"
)
foreach ($Item in $PublishLibraries)
{
    dotnet publish $PSScriptRoot\..\$Item --use-current-runtime --output $LibraryPublishFolder
}
# Publish Nugets
$PublishNugets = @(
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

# Create archive
$Date = Get-Date -Format yyyyMMdd
$ArchiveFolder = "$PublishFolder\..\Packages"
$ArchivePath = "$ArchiveFolder\Pure_DistributionBuild_Windows_B$Date.zip"
New-Item -ItemType Directory -Force -Path $ArchiveFolder
Compress-Archive -Path $PublishFolder\* -DestinationPath $ArchivePath -Force

# Validation
if (-Not (Test-Path (Join-Path $PublishFolder "Pure.exe")))
{
    Write-Host "Build failed."
    Exit
}

Set-Location $PrevPath