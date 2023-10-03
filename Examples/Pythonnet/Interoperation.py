# Interoperation
# This script demonstrates calling Pure Core functionalities (through Interpreter) through Pythonet
# Notice pythonnet finds modules from sys.path, which only appends paths from PYTHONPATH but not PATH, so we will need to explicitly append the path to Core.dll in, or add that to PYTHONPATH

from pythonnet import load
load("coreclr")

import clr
clr.AddReference("Core")
from Core import Interpreter

interpreter = Interpreter("", None, None, None, None)
interpreter.Start()
interpreter.Parse("""
// Cannot declare namespace in script code
double MyFunc() => 15;

public record Sample(int Value);
public static class Tester
{
    public static Sample Get() => new Sample(30);
}
""")

v = interpreter.Evaluate("Tester.Get()")
print(v)

# Additional Resource:
# - [Where is Python's sys.path initialized from](https://stackoverflow.com/questions/897792/where-is-pythons-sys-path-initialized-from)