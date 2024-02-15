# Use command line toggle --incremental to skip deleting publish folder and skip creating archive, thus making build faster.
Write-Host "Publish for final packaging build..."
$PrevPath = Get-Location
Set-Location $PSScriptRoot

$PublishFolder = "$PSScriptRoot\..\Publish"
$LibraryPublishFolder = "$PublishFolder\Libraries"

# Delete current data
if ($Args[0] -ne '--incremental') {
    Remove-Item $PublishFolder -Recurse -Force
}

# Build components
. $PSScriptRoot\BuildCore.ps1
. $PSScriptRoot\BuildLibraries.ps1
. $PSScriptRoot\BuildFrameworks.ps1

# Validation
$pureExePath = Join-Path $PublishFolder "Pure.exe"
if (-Not (Test-Path $pureExePath)) {
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