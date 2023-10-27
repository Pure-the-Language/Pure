using RestSharp;
using System.Net;

namespace CentralSnippets
{
    public static class Main
    {
        /// <summary>
        /// When using HTTP, will fetch using GET; When specifying local disk, will read file
        /// </summary>
        public static string SnippetsHostSite { get; set; } = @"https://centralsnippets.pure.totalimagine.com";
        /// <summary>
        /// Can be null/empty if no need for appendix
        /// </summary>
        public static string SnippetsRootFolder { get; set; } = @"Snippets";

        public static void Pull(string snippetIdentifier)
        {
            string content = GetContent(SnippetsHostSite, SnippetsRootFolder, snippetIdentifier);
            Core.Utilities.Construct.Parse(content);
        }
        public static void Preview(string snippetIdentifier)
        {
            string content = GetContent(SnippetsHostSite, SnippetsRootFolder, snippetIdentifier);
            Console.WriteLine(content);
        }
        public static void SetProxy(string httpProxy, string httpsProxy, string noProxy)
        {
            Environment.SetEnvironmentVariable("http_proxy", httpProxy, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("https_proxy", httpsProxy, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("no_proxy", noProxy, EnvironmentVariableTarget.Process);
        }
        public static void DisableSSLCheck()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }

        #region Private Helpers
        private static string GetContent(string url, string rootFolder, string secondaryPath)
        {
            if (url.ToLower().StartsWith("http"))
            {
                var options = new RestClientOptions(url)
                {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                };
                var client = new RestClient(options);   // Remark-cz: Notice we can't automatically and easily append ".cs" to the file name, so for network snippets, full names are required
                RestRequest request = string.IsNullOrEmpty(rootFolder) ? new (secondaryPath) : new($"{rootFolder}/{secondaryPath}");
                string response = client.Get(request).Content;
                return response;
            }
            else if (Directory.Exists(url))
            {
                string filePath = string.IsNullOrEmpty(rootFolder) ? Path.Combine(url, secondaryPath) : Path.Combine(url, rootFolder, secondaryPath);
                if (File.Exists(filePath))
                    return File.ReadAllText(filePath);
                else if (File.Exists(filePath + ".cs"))
                    return File.ReadAllText(filePath + ".cs");
                else throw new ArgumentException($"Cannot locate file: {filePath}");
            }
            else throw new ArgumentException($"Cannot parse url: {url}");
        }
        #endregion
    }
}