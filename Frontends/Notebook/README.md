# Notebook (Pure)

A desktop-local WPF based Jupyter-Notebook-like literate programming client for Pure that is native to Pure (and Windows) and serves as a quick debug-development friendly environment/alternative to Pure CLI/REPL in a fashion similar to iPython. Unlike Jupyter notebook, this is completely version-control friendly.

When triggered in command line (CMD and PowerShell), will enter CLI mode.

## TODO

See in-code "TODO" for implementation specific todo items (also viewable in VS Task List).  
See standard library-specific README for TODO items for standard libraries.

Advanced features:

* [GUI] CPU and MEM usage monitoring
* [GUI] `// @Param <Description>` for GUI controls of primitive inputs
* [GUI] Ability to hide/collape code cell blocks by default (when opening new files)
* [System] Allow self-referencing in Notebook, i.e. somehow inject a Notebook only property like "Arguments" that allows the running cell to access previous cell outputs (and maybe outputs of all other cells); This will allow a single cell script to have self-sustained state. May implement this as an extension of base class Interpreter.

### Issues

1. Currently we have no way to limit max height for Avalon Edit - the Text Editor control either is fixed at a certain height or will not show scrollbar automatically. And it eats our scrolling event and that makes scolling and editing long code blocks inconvinient. This issue is mentioned here: https://github.com/icsharpcode/AvalonEdit/issues/394
2. Notice at the moment the core Interpreter supports only a single (ever) instance of Roslyn context - thus when opening new files the old state is maintained. This means if we try to work with different files in a clean state, we should start new process instances instead of relying on "Open" file.
3. Per [this](https://github.com/pythonnet/pythonnet/discussions/1794) and [this](https://github.com/pythonnet/pythonnet/issues/1501): Currently cannot get output from Python using `print()`. There is a workaround being implemented in [as C#-side implementation](https://github.com/Pure-the-Language/Pure/commit/03b04fe3fbf0f87999a51e2b599b3d4185004f73).

## CLI Arguments

* `--debug`: Show parent process name
* `Notebook <Filepath>`: Batch run process
* `Notebook <Filepath> [<Arguments>]`: Batch run process with command line arguments

## Notebook Format

Notebook is stored in native (GitHub Flavoured) Markdown/HTML including documents, codes, and code result caches.

* The seperation between "Markdown" and "Code" cell blocks are delimited only by the boundary of code blocks (as in markdown).
* Those code cells have special meaning and native Markdown codes should try to avoid: 
	1. `C#`
	2. `Python`
	3. Code block with starting line as: `(Collapsed)`
	4. `Output Cache`