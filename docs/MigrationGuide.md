# Migration Guide

Below lists the steps we take from migrating from .Net 7 to .Net 8, which might serve as reference for future upgrades:

- [x] Replace all mentioning of .Net 7 in source code to .Net 8 and fix related comments/codes; Search for: `.net 7` and `net7`
- [ ] Change all `.csproj` to target net 8
- [ ] Make sure all project package references are upgraded accordingly, especially concerning code compilation services; We can search globally `7.0` to identify such locations; Update module dependancies related to `Microsoft.CodeAnalysis.CSharp`
- [ ] Run all available unit tests
- [ ] Increment and update CoreVersion variable and corresponding changelog string.
- [ ] Test and make sure `Import` still work (as implemented in `TryDownloadNugetPackage`), and targets .Net 8
- [x] Delete everything from ***Publish*** and ***.vs*** folder and build fresh
- [x] Follow and double check with https://learn.microsoft.com/en-us/aspnet/core/migration/70-80 for additional steps
- [x] Good thing we are not using Blazor or ASP.Net Core or any other larger framework otherwise migration can be a hassle - and we should keep avoid doing that in the future

We can identify the following as "fragile" components that's .Net version sensitive:

* Core module's dependancy on Roslyn compilation services
* Razor module's dependancy on RazorEngine.NetCore
* Behavior of `Import` macro
* Any windows-specific modules (e.g. plotting)

References:

* https://learn.microsoft.com/en-us/aspnet/core/migration/70-80
* https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0
* https://learn.microsoft.com/en-us/dotnet/core/compatibility/sdk/8.0/dotnet-publish-config