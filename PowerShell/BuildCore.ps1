# Build core components and CLI
Write-Host "Build core..."
$PrevPath = Get-Location
Set-Location $PSScriptRoot

$PublishFolder = "$PSScriptRoot\..\Publish"
$WindowsPublishFolder = "$PublishFolder\Windows"

# Publish Executables
$PublishExecutables = @(
    "Frontends\Pure\Pure.csproj"
)
foreach ($Item in $PublishExecutables) {
    dotnet publish $PSScriptRoot\..\$Item --use-current-runtime --output $PublishFolder
}
# Publish Windows-only Executables
if ($IsWindows) {
    $PublishWindowsExecutables = @(
        "Frontends\Notebook\Notebook.csproj"
    )
    foreach ($Item in $PublishWindowsExecutables) {
        # In .Net 8, those windows-specific builds might interfere with other non-windows build
        dotnet publish $PSScriptRoot\..\$Item --runtime win-x64 --self-contained --output $WindowsPublishFolder
    }
}

Set-Location $PrevPath