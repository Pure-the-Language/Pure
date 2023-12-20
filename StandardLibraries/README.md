# Standard Libraries

Lean wrappers and thin dependencies, scripting-oriented. Moving forward we generally do not wish to implement a `Main` in new libraries and rely on meta-packages to expose/import interfaces.

* Meta-Package: Mwta-packages are implemented as proper C# dlls but can have instructs that's running on scripting scope.