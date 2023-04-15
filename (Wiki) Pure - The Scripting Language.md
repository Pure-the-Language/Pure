# Pure - The Scripting Language

As a scripting language, pure has the following features:

* It's strongly typed
* It requires a semi-colon to end a statement
* Its names are case sensitive

## Introduction

### Basic Usage

Download [BaseRepl] to get familiar with CSharp syntax and REPL with C#.

Play with Pure just like BaseRepl (v0.0.1 [pre-release is available on [Itch.io](https://charles-zhang.itch.io/pure)).

### Create a Library for Use in Pure

The intended usage scenario of Pure is like this:

* Pure provides a complete syntax and all of the standard features of .Net 7 runtime
* On top of the basics, Pure provides some syntactic sugars and global utility methods (those features are optional when using **Purer**)
* On top of that, Pure provides native support for library importing (using `Import()`) and scripting importing (using `Include()`)
* When extending existing functionalities of Pure or simply developing libraries around re-usable code, a developer would write actual C# Class Library projects and expose those as DLLs for end consumption in Pure.

A library is just any regular C# Net 7 DLL.

Create a static class named `Main` - all static members will be available globally when using `Import()` in Pure. By default, when importing in Pure, all namespaces of the DLL module containing public classes will be imported.

### Who should use Pure

* Programming Beginner
* C# Veterans
* Those looking for alternative to Python for automation.

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

As a scripting language, REPL and scripts are two primary use of Pure. REPL refers to read–eval–print loop, it's done by interactively executing Pure expressions with one of the four interpreter terminals: Pure, Virgin, Purer, and Aurora.

### Get Information (NOT IMPLEMENTED)

At any time during REPL (read–eval–print loop), use `Help(<name>)` to get information on variables, types, namespaces, and libraries.

### Save Session

After you've done some REPL exercise, you can output you inputs in this session by using the `Save(<File Path>)` command. After it's saved, you can modify and clean up the saved history of commands for proper script re-use.