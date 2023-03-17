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
    "Pure"
    "Purer"
    "Aurora"
    "Virgin"
    "Righteous"
)
foreach ($Item in $PublishExecutables)
{
    dotnet publish $PSScriptRoot\..\$Item\$Item.csproj --use-current-runtime --output $PublishFolder
}
# Publish Loose Libraries
$PublishLibraries = @(
    "Fluent"
    "Python"
    "ODBC"
)
foreach ($Item in $PublishLibraries)
{
    dotnet publish $PSScriptRoot\..\$Item\$Item.csproj --use-current-runtime --output $LibraryPublishFolder
}
# Publish Nugets
$PublishNugets = @(
    "Fluent"
    "Python"
    "ODBC"
)
foreach ($Item in $PublishNugets)
{
    dotnet pack $PSScriptRoot\..\$Item\$Item.csproj --output $NugetPublishFolder
}

# Create archive
$Date = Get-Date -Format yyyyMMdd
$ArchiveFolder = "$PublishFolder\..\Packages"
$ArchivePath = "$ArchiveFolder\Pure_DistributionBuild_Windows_B$Date.zip"
New-Item -ItemType Directory -Force -Path $ArchiveFolder
Compress-Archive -Path $PublishFolder\*.* -DestinationPath $ArchivePath -Force

# Validation
if (-Not (Test-Path (Join-Path $PublishFolder "Pure.exe")))
{
    Write-Host "Build failed."
    Exit
}

Set-Location $PrevPath