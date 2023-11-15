# ODBC

Status: Official

This library provides standard ODBC access with special routines for strongly typed encapsulation.  
Behind the scene, it's a thin wrapper around Dapper.

## Changelog

* v0.0.1: First draft; Implement preliminary Select and other commands.
* v0.1.0: Provides transaction capability.
* v0.1.1: Enhance exception handling, add DSN null check. Update type methods documentaion. (Notice when DSN is null, writing a message to standard output instead of raises an exception is more user friendly but only for REPL, so we replaced it with a proper exception instead)