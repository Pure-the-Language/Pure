# Pipeline (NON-ALPHA)

(THIS IS WORK-IN-PROGRESS AND NOT EVEN ALPHA YET, INTERFACE MAY CHANGE ANYTIME)

Run CLI commands perl style. Streamlines running command and gets output as string.

Provides:

* `string Run(string, string)`
* `string Run(string, string[])`
* `string Run(string, Dictionary<string, string>)`

## TODO

`Run` should return a `Session` object instead and provide Fluent API to chain program runs together and allow piping inputs/ouputs. The session object may provide "Result" for the last run result instead of returning a string output directly from `Run()`, and optionally save all intermediate results inside the Session object.

## Changelog

Versions:

* v0.0.1/v0.1.0: Pre-Alpha
* v0.2.0: Support standard input redirect from string; Implement Pipeline Fluent API.