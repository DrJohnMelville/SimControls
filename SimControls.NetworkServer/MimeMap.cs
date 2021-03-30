using System.Text.RegularExpressions;

namespace SimControls.NetworkServer
{
    public static class MimeMap
    {
        private static Regex extensionFinder = new Regex(@"[^\.]*$");
        public static string FromPath(string path)
        {
            return FromExtension(extensionFinder.Match(path).Value);
        }

        private static string FromExtension(string value)
        {
             return value.ToLower() switch
             {
                 "htm" or "html" => "text/html",
                 "css" => "text/css",
                 "js" => "text/javascript",
                 "json"=> "application/json",
                 "wasm"=> "application/wasm",
                 _=> "application/octet-stream"
             };
        }
    }
}