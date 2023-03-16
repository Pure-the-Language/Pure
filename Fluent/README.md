# Fluent

A fluent-API style cross-domain strongly typed library suite currently specifically targeting data analysis use. In a sense, all Pure libraries should be Fluent-style, it's just this one is a standalone meta-package that targets specifically for data analysis use. 

As a meta-package, it's going to be huge in size, but the bright side is this: it provides more like an out-of-box experience and a standard ("the Pure Way") set of functionalities and consistent API access - thus avoid the need to reference specific NuGet packages (epsecially since as of Pure v0.0.2 the functionality of importing arbitrary NuGet packages is not implemented yet).

## Development

Conceptually, combined with Pure's simplistic language styles, this package aims to provide an alternative to some other proposed workflow constructs like Expresso or Parcel V6. Conceptually, if we shall call it "Parcel V7", it's like auto-generated C# nodes for strongly functional workflows + auto-Roslyn code-generation + C#/Python extension nodes. It's just as a mere library there is no internal caching mechanism - which we shall implement inside a host like Righteous.

In general, all standard Pure packages should be independent of each other, so is Fluent, and other packages should generally avoid depending on Fluent directly.