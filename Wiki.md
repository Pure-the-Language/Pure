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

Use `Import(<Library Name>)` to import libraries. Libraries should be available under PATH as .Net 8 (or .Net Standard) DLL files.

A library is a collection of C# functionalities. ~~It can optionally expose a static Main class, the methods of which will become available at global scope upon import~~ (deprecating, consider using `using static` explicitly instead).

### 1D Vectot Numerics

Pure supports simple vector numerics through a library. See Vector library for more details. Below is some basic example:

```c#
// Define vectors using values or strings
var a = Vector(1, 2, 3)
var b = Vector("2 3 4")

// Vectors support arithmetics using operators
a + b
a * 5
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

## Chapter N - Proper Usage Tipcs -> Best Practice

Pure is designed for quick one-shot scripts that are short and functional. As a rule of thumb, it's intended for things that do not exceed a few hundred lines - assuming proper code management is already implemented.

As scripts grows and code is refactored for proper management, one might use `Include` for simple code management; But as code grows in complexity, either one of the two must be done for more reasonable code management:

1. Refactor shared code into proper C# DLL as library;
2. Refactor code project into a proper standalone C# solution.

New: "Develop in Visual Studio, Run in Pure" methodology. For easier debugging and styling for beginners.

### Scale Up

As code logic becomes more intricate, without a proper debugger (which is planned as a VS Code extension), it's slow to debug compared to traditional compiled languages. In this case, one can utilize the power of `dotnet` CLI utility and Visual Studio to enable quick development.

(In short):

* `dotnet new console`
* `dotnet run <project-name.csproj>`

This creates a minimum 2-file template for any single file script. Syntax of certain things might need tweaking and referencing Pure standard libraries need tweaking as well (pending documentation).