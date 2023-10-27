using Core;
using RestSharp;

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

        #region Methods
        public static void Pull(string snippetIdentifier, bool disableSSL = false)
        {
            string content = GetContent(SnippetsHostSite, SnippetsRootFolder, snippetIdentifier, disableSSL);
            // Remark-cz: We need to split script first to allow handling of specific Pure constructs (e.g. Import)
            foreach(var segment in Interpreter.SplitScripts(content))
                Core.Utilities.Construct.Parse(segment);
        }
        public static void Preview(string snippetIdentifier, bool disableSSL = false)
        {
            string content = GetContent(SnippetsHostSite, SnippetsRootFolder, snippetIdentifier, disableSSL);
            Console.WriteLine(content);
        }
        /// <summary>
        /// Download text or binary content
        /// </summary>
        public static void Download(string resourceIdentifier, string outputPath, bool disableSSL = false)
        {
            DownloadContent(SnippetsHostSite, SnippetsRootFolder, resourceIdentifier, outputPath, disableSSL);
        }
        /// <summary>
        /// Download from HTTP
        /// </summary>
        public static void DownloadUrl(string url, string outputPath, bool disableSSL = false)
        {
            var options = new RestClientOptions(url);
            if (disableSSL)
                options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            var client = new RestClient(options);
            RestRequest request = new();
            var bytes = client.DownloadData(request);
            File.WriteAllBytes(outputPath, bytes);
        }
        #endregion

        #region Private Helpers
        private static void DownloadContent(string url, string rootFolder, string secondaryPath, string outputPath, bool disableSSL = false)
        {
            if (url.ToLower().StartsWith("http"))
            {
                var options = new RestClientOptions("https://centralsnippets.pure.totalimagine.com");
                if (disableSSL)
                    options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                var client = new RestClient(options);
                string path = string.IsNullOrEmpty(rootFolder) ? secondaryPath : $"{rootFolder}/{secondaryPath}";
                RestRequest request = new($"Snippets/Demos/HelloWorld.cs");
                var bytes = client.DownloadData(request);
                if (bytes == null)
                    throw new Exception($"Cannot get data from: {url}/{path} (Try disable SSL and see whether it works)");
                File.WriteAllBytes(outputPath, bytes);
            }
            else if (Directory.Exists(url))
            {
                string filePath = string.IsNullOrEmpty(rootFolder) ? Path.Combine(url, secondaryPath) : Path.Combine(url, rootFolder, secondaryPath);
                if (File.Exists(filePath))
                    File.Copy(filePath, outputPath);
                else throw new ArgumentException($"Cannot locate file: {filePath}");
            }
            else throw new ArgumentException($"Cannot parse url: {url}");
        }
        private static string GetContent(string url, string rootFolder, string secondaryPath, bool disableSSL = false)
        {
            if (url.ToLower().StartsWith("http"))
            {
                var options = new RestClientOptions(url);
                if (disableSSL)
                    options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
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