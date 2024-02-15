# Migration Guide

> In general, this upgrade brings no breaking changes in terms of API or Pure language core construct behaviors. (The only known change is Notebook's executable path)

Below lists the steps we take from migrating from .Net 7 to .Net 8, which might serve as reference for future upgrades:

- [x] Replace all mentioning of .Net 7 in source code to .Net 8 and fix related comments/codes; Search for: `.net 7` and `net7`
- [x] Change all `.csproj` to target net 8
- [x] Make sure all project package references are upgraded accordingly, especially concerning code compilation services; We can search globally `7.0` to identify such locations; Update module dependancies related to `Microsoft.CodeAnalysis.CSharp`
- [x] Run all available unit tests
- [ ] Update `PublishAll.ps1` script if there is any `dotnet` CLI arguments syntax change
- [x] Increment and update CoreVersion variable and corresponding changelog string.
- [ ] Test and make sure `Import` still work (as implemented in `TryDownloadNugetPackage`), and targets .Net 8
- [x] Delete everything from ***Publish*** and ***.vs*** folder and build fresh; Delete `AppData\Local\Temp\Pure\NugetDownloads`
- [x] Follow and double check with https://learn.microsoft.com/en-us/aspnet/core/migration/70-80 for additional steps
- [x] Good thing we are not using Blazor or ASP.Net Core or any other larger framework otherwise migration can be a hassle - and we should keep avoid doing that in the future
- [ ] Core libraries: Make sure ODBC and Razor works
- [ ] Update organization page stating we are using .Net 8 instead of .Net 7

We can identify the following as "fragile" components that's .Net version sensitive:

* Core module's dependancy on Roslyn compilation services
* Razor module's dependancy on RazorEngine.NetCore
* Behavior of `Import` macro
* Any windows-specific modules (e.g. plotting)

User upgrade guide:

- [ ] Decide whether upgrade is necessary based on whether referenced NuGet packages in user scripts are incompatible with .Net 8
- [ ] Run .Net 8 runtime AND ASP.Net Core hosting bundle: (For windows) [ASP.Net Core Hosting Bundle](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.0-windows-hosting-bundle-installer), [.Net 8 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-8.0.0-windows-x64-installer), [.Net Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-8.0.0-windows-x64-installer)
- [ ] Install latest Pure distribution
- [ ] Make sure all modules in `PATH` that user script depends on are updated and built for. Net 8
- [ ] (THIS STEP IS NECESSARY) Delete contents in `AppData\Local\Temp\Pure\NugetDownloads` folder for a fresh start
- [ ] BREAKING CHANGE: If `Notebook` is used, its path has been moved, so one might need to modify Windows shortcut target path or update new executable path to environment `PATH` - it's now in `<Pure Distribution Folder>\Windows\Notebook.exe`

References:

* https://learn.microsoft.com/en-us/aspnet/core/migration/70-80
* https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0
* https://learn.microsoft.com/en-us/dotnet/core/compatibility/sdk/8.0/dotnet-publish-config