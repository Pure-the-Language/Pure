# Central Snippets

Allow pulling snippets or definitions directly from https://centralsnippets.pure.totalimagine.com, hosted on [Github](https://github.com/Pure-the-Language/CentralSnippets).

For users inside enterprise network and have issues with SSL, it's necessary to setup proxies, on Windows using PowerShell, it can be done as below:

```powershel
$env:http_proxy="http://your_proxy_server:your_port"
$env:https_proxy="http://your_proxy_server:your_port"
$env:no_proxy="localhost,127.0.0.1,other_addresses"
```

Alternatively, call pass `true` to `disableSSL` check parameter of `Preview()` or `Pull()`.

## TODO

- [ ] Add `List` function to list available categories
- [ ] Add `Find` function to find and provide snippet path for specific snippets
