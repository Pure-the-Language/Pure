# Python Library Usage in Notebook

* Use as regular python inside Python code cells; Cannot use `print()` because it is not implemented
* Use ReadPythonScope() to read and call functions from python scope within C#
* Use ModifyPythonScope() to put stuff inside python
* More details see Python.Net documentation

```Python
import sys
```

```C#
WriteLine(ReadPythonScope(scope => scope.sys.version));
ModifyPythonScope(scope => scope.var = 15);
```

```Cache
3.11.3 (tags/v3.11.3:f3909b8, Apr  4 2023, 23:49:59) [MSC v.1934 64 bit (AMD64)]
```

```Python
newVar = var * 2
```

```C#
WriteLine(ReadPythonScope(scope => scope.newVar));
```

```Cache
30
```

## Passing .Net Functions

.Net funcs cannot be passed directly using above methods, but we can pass entire objects, so we just need a think wrapper.

```C#
class Wrapper
{
	public string Func() => "Hello";
}
Wrapper ins = new Wrapper();
ModifyPythonScope(scope => scope.ins = ins);
```

```Python
result = ins.Func()
```

```C#
WriteLine(ReadPythonScope(scope => scope.result).ToString());
```