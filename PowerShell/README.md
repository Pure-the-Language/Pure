# Pure Powershell Scripts

Contains automation scripts targeting PowerShell 7.

```mermaid
flowchart TD
    PublishAll
    BuildCore
    BuildLibraries
    BuildFrameworks

    PublishAll --> BuildCore
    PublishAll --> BuildLibraries
    PublishAll --> BuildFrameworks
```