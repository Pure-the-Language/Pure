# Pure

(Definition) Pure is the scripting version of C# with:

1. Default global scope Math functions
2. Minimal syntactic sugar for more friendly REPL usage
3. Native 1D "Vector" for numerical data processing
4. Making simple and common things simpler

Features:

* Single-word package names.
* Syntactic sugars.
* Package management (Aurora).

## URGENT

Currently Pure has two main issues that stops it from being used for production use (but it's OK right now to use it for one-liners and quick REPL purpose):

1. The REPL interpreter right now (along with the core engine) aka. Pure.exe cannot parse statements that span multiple lines, this makes stuff like defining functions or classes very inconvinient if not at all impossible.
    * The Pure.exe program itself apparently needs work, along with tne engine code.
    * We are yet to integrate and implement BaseRepl for Aurora from CSharpRepl.
2. Importing and consuming Nuget packages is not fully implemented yet.
3. Currently "Help" is not showing extension methods.

As it goes, it is actually possible at the moment to use Righteous along with manually downloaded DLL packages to implement all the needed functions.

Currently for simplifying parsing of scripts we require script C# syntaxes - however it's really NOT nice to have semi-colons in scripts. Ideally we can work around that but it's hard.

## Syntax & Special Commands

* Use `Import()` to import modules from PATH
* Use `Help(name)` to get help about namespaces, types, and specific variables
* Create 2D arrays of doubles directly using the syntax `var name = [<elements>]`
* (Only applicable at REPL) For single line assignment and variable creation, no need to append `;` at end of line - for script files, it must be properly formatted
* (Deprecated) Use `#` add the beginning of line for line-style comment

## Limits

This problem is written in Net 7.

Currently limited to Net Standard 2.0 and maybe Net Core 3.1 contexts. For instance, System.Data.Odbc will throw exception "Platform not Supported" even though the platform (e.g. win64) is supported - it's the runtime that's causing the issue.

Solution: All plugins should target Net Standard 2.0+ or Net Core 3.1+ or Net Framework 4.6.1+.
ACTUALLY THIS IS NOT TRUE. THE RUNTIME IS INDEED 7.0.1 (within Roslyn), and CSharpREPL can consume System.Data.Odbc without problem.

Alternatively, use CSharpREPL to find the correct DLL that we need (e.g. for ODBC, it should be `AppData\Roaming\.csharprepl\packages\System.Data.Odbc.7.0.0\runtimes\win\lib\net7.0\System.Data.Odbc.dll`).

### Troubleshooting

System.Drawing is not supported on this platform. Notice the scenario is like that when using ODBC libraries - likely because it has many runtime versions. This is because at the moment "Import" cannot properly make use of the likely-redirection dlls. One solution for this is to select specific runtime when build instead of target "Portable" runtime.

### Library Requirements

Note that libraries CANNOT directly target (or indirectly target) .Net 7 Windows because the hosting environment (aka. Pure) target .Net 7.
The solution for this is to isolate such components and trigger as sub-process (so some sort of data tranmission or IPC is needed).

## To Do

- [ ] (Pipeline Library) Provide Utilities.Run that streamlines running command and gets output as string.
- [ ] (Data Library) Provide `Data` (standalone).
- [ ] (Core) Enhance "Arguments" with all kinds of command line argument utilities like Elixir and how we usually use it.
