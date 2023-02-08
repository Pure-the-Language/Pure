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

## Syntax

* For single line assignment and variable creation, no need to append `;` at end of line

## Limits

This problem is written in Net 7.

Currently limited to Net Standard 2.0 and maybe Net Core 3.1 contexts. For instance, System.Data.Odbc will throw exception "Platform not Supported" even though the platform (e.g. win64) is supported - it's the runtime that's causing the issue.

Solution: All plugins should target Net Standard 2.0+ or Net Core 3.1+ or Net Framework 4.6.1+.
ACTUALLY THIS IS NOT TRUE. THE RUNTIME IS INDEED 7.0.1 (within Roslyn), and CSharpREPL can consume System.Data.Odbc without problem.

Alternatively, use CSharpREPL to find the correct DLL that we need (e.g. for ODBC, it should be `AppData\Roaming\.csharprepl\packages\System.Data.Odbc.7.0.0\runtimes\win\lib\net7.0\System.Data.Odbc.dll`).