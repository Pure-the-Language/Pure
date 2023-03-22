# Notebook (Pure)

A desktop-local WPF based Jupyter-Notebook-like literate programming client for Pure that is native to Pure (and Windows) and serves as a quick debug-development friendly environment/alternative to Pure CLI/REPL in a fashion similar to iPython. This is the GUI version for Pure that is easier to write and maintain than Righteous and before the C# Terminal port becomes a thing. Unlike Jupyter notebook, this is completely version-control friendly.

## TODO

Advanced features:

* [GUI] CPU and MEM usage monitoring
* [GUI] `// @Param <Description>` for GUI controls of primitive inputs
* [GUI] Ability to hide/collape code cell blocks by default (when opening new files)

## Issues

1. Currently we have no way to limit max height for Avalon Edit - the Text Editor control either is fixed at a certain height or will not show scrollbar automatically. And it eats our scrolling event and that makes scolling and editing long code blocks inconvinient.

## Notebook Format

Notebook is stored in native (GitHub Flavoured) Markdown/HTML including documents, codes, and code result caches.

* The seperation between "Markdown" and "Code" cell blocks are delimited only by the boundary of code blocks (as in markdown).
* Those code cells have special meaning and native Markdown codes should try to avoid: 
	1. `C#`
	2. `Python`
	3. Code block with starting line as: `(Collapsed)`
	4. `Output Cache`