# Pure - Lightweight Native C# Scripting Environment for .Net 7

(Definition) Pure is the scripting version of C# with:

1. Default global scope Math functions
2. Minimalist syntactic sugar for more friendly REPL usage
3. Native 1D "Vector" for numerical data processing
4. Making simple and common things simpler

Features:

* Single-word package names.
* Syntactic sugars.
* On prompto package invokation (through `Import`).
* Pure uses `PUREPATH` to search for scripts when using `Include` and when executing script file directly from command line as the first argument.

(Additional) Standard Libraries:

* ODBC
* Python
* Pipeline
* Razor

Syntax & Special Commands:

* Use `Import()` to import modules from PATH; Use `Include()` to transclude scripts from file name
* Use `Help(name)` to get help about namespaces, types, and specific variables
* Create 2D arrays of doubles directly using the syntax `var name = [<elements>]`
* (Only applicable at REPL) For single line assignment and variable creation, no need to append `;` at end of line - for script files, it must be properly formatted
* (Deprecated) Use `#` add the beginning of line for line-style comment

## Installation

The source code can be built on Windows/Linux with .Net 7 SDK.

* To use on Windows, just download prebuilt executables from [Release](./releases/latest).
* To build on Linux, either try `PublishAll.ps1` (require `pwsh`), or just do `otnet publish Frontends/Pure/Pure.csproj --use-current-runtime --output Publish`.

## TODO

- [ ] (Core)(Pipeline Library) Provide Utilities.Run that streamlines running command and gets output as string.
- [ ] (Data Library) Provide `Data` (standalone).
- [ ] (Core) Enhance "Arguments" with all kinds of command line argument utilities like Elixir and how we usually use it.

## Technicalities

### URGENT

Currently Pure has two main issues that stops it from being used for production use (but it's OK right now to use it for one-liners and quick REPL purpose):

1. The REPL interpreter right now (along with the core engine) aka. Pure.exe cannot parse statements that span multiple lines, this makes stuff like defining functions or classes very inconvinient if not at all impossible. The Pure.exe program itself apparently needs work, along with tne engine code - as a full re-write/replacement, we are yet to integrate BaseRepl from CSharpRepl and implement codename Aurora, though we must be careful to make sure the runtime and startup speed of Pure is acceptible, because at the moment the startup time of Pure is much slower than Python, Elixir or other cli programs.
2. Importing and consuming Nuget packages is functional but not fully streamlined/safe yet.
3. Currently `Help` is not showing extension methods.

Currently for simplifying parsing of scripts we require script C# syntaxes - however it's really NOT nice to have semi-colons in scripts. Ideally we can work around that but it's hard. (In practice when authoring long scripts this is not an issue)

## Create a Library for Use in Pure

The intended usage scenario of Pure is like this:

* Pure provides a complete syntax and all of the standard features of .Net 7 runtime
* On top of the basics, Pure provides some syntactic sugars and global utility methods (those features are optional when using **Purer**)
* On top of that, Pure provides native support for library importing (using `Import()`) and scripting importing (using `Include()`)
* When extending existing functionalities of Pure or simply developing libraries around re-usable code, a developer would write actual C# Class Library projects and expose those as DLLs for end consumption in Pure.

A library is just any regular C# Net 7 DLL.

Create a static class named `Main` - all static members will be available globally when using `Import()` in Pure. By default, when importing in Pure, all namespaces of the DLL module containing public classes will be imported.

### Troubleshooting

Pure is written in Net 7. However, when loading external DLLs, it's currently only safe to assume only Net Standard 2.0 and maybe Net Core 3.1 contexts are supported (i.e. just loading Net 7 may not work out of the box). For instance, System.Data.Odbc will throw exception "Platform not Supported" even though the platform (e.g. win64) is supported - it's the runtime that's causing the issue. A temporary workaround is when developing plugin/libraries, all plugins should target Net Standard 2.0+ or Net Core 3.1+ or Net Framework 4.6.1+. In reality, the issue is NOT within Net 7 as hosting context - THE RUNTIME IS INDEED 7.0.1 (within Roslyn), and [CSharpREPL](https://github.com/waf/CSharpRepl) (our reference interpreter) can consume System.Data.Odbc without problem, so we should be able to load Net 7 assemblies, it's just due to the way we handle DLL loading, we need to load exactly the correct DLL and cannot load an "entry" dll. We use CSharpREPL to find the correct DLL that we need (e.g. for ODBC, it should be `AppData\Roaming\.csharprepl\packages\System.Data.Odbc.7.0.0\runtimes\win\lib\net7.0\System.Data.Odbc.dll`).

System.Drawing is not (directly) supported on this platform. Notice the scenario is like that when using ODBC libraries - likely because it has many runtime versions. This is because at the moment "Import" cannot properly make use of the likely-redirection dlls. One solution for this is to select specific runtime when build instead of target "Portable" runtime.

Library Authoring Requirements: Note that any (plug-in) libraries being authored CANNOT (and thus should not) directly target (or indirectly target) *.Net 7 Windows* because the hosting environment (aka. Pure) target *.Net 7* (without specifying windows as target). The solution for this is to isolate such components and trigger as sub-process (so some sort of data tranmission or IPC is needed).

# (Wiki) Pure - The Scripting Language

<!-- This will be the main book for Pure usage; Consider it as a user guide; This shall be written independent from the rest of Pure solution documentations. -->

As a scripting language, pure has the following features:

* It's strongly typed
* It requires a semi-colon to end a statement
* Its names are case sensitive

Who should use Pure:

* Programming Beginner
* C# Veterans
* Those looking for alternative to Python for automation.

Basic Usage: 

* Download [BaseRepl](https://github.com/Pure-the-Language/BaseRepl) to get familiar with CSharp syntax and REPL with C#.
* Play with Pure just like BaseRepl (Releases are generally available on [Github](https://github.com/Pure-the-Language/Pure/releases) or [Itch.io](https://charles-zhang.itch.io/pure)).

## Chapter 1 - Basics

### Declare Variables

You can declare variables using keyworf `var`. Variables must be declared and initialized before it can be used. There is no need for semicolons when declare variables.

```C#
var a = 5 // Good
b = 7 // Bad: b is not defined
```

### Import Libraries

Use `Import(<Library Name>)` to import libraries. Libraries should be available under PATH as .Net 7 (or .Net Standard) DLL files.

A library is a collection of C# functionalities. It can optionally expose a static Main class, the methods of which will become available at global scope upon import.

### 1D Vectot Numerics

Pure supports simple MATLAB like vector numerics.

You can define a vector this way: `var a = [1, 2, 3, 4, 5]`. That is, use square brackets of numerical values to assign to a variable.

Vectors support arithmetics: 

```c#
var a = [1, 2, 3]
var b = 5
a + b
```

## Chapter 2 - REPL

As a scripting language, REPL and scripts are two primary use of Pure. REPL refers to read–eval–print loop, it's done by interactively executing Pure expressions with the interpreter terminal Pure.

### Get Information (NOT IMPLEMENTED)

At any time during REPL (read–eval–print loop), use `Help(<name>)` to get information on variables, types, namespaces, and libraries.

### Save Session

After you've done some REPL exercise, you can output you inputs in this session by using the `Save(<File Path>)` command. After it's saved, you can modify and clean up the saved history of commands for proper script re-use.

## Chapter 3 - Standard Libraries

A few standard libraries are provided as light wrappers of some conventional functionalities as encountered per author's experience and work needs.

### ODBC

```c#
DSN = "Some DSN";
// Create a type to use as return result
public record Result(string Name, double Value);
Select<Result>("""
SELECT
    Name,
    Value
FROM MyValues
"""); // Returns an array
```

## Chapter N - Proper Usage Tipcs

Pure is designed for quick one-shot scripts that are short and functional. As a rule of thumb, it's intended for things that do not exceed a few hundred lines - assuming proper code management is already implemented.

As scripts grows and code is refactored for proper management, one might use `Include` for simple code management; But as code grows in complexity, either one of the two must be done for more reasonable code management:

1. Refactor shared code into proper C# DLL as library;
2. Refactor code project into a proper standalone C# solution.
