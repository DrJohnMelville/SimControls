using SimControls.NetworkServer;
using Xunit;

namespace SimControls.Test.NetworkServer
{
    public class MimeMapTest
    {
        [Theory]
        [InlineData("/index.html", "text/html")]
        [InlineData("/index.htm", "text/html")]
        [InlineData("/index.js", "text/javascript")]
        [InlineData("/bine/etc/com//index.css", "text/css")]
        [InlineData("/bine/etc/com//index.json", "application/json")]
        [InlineData("/bine/etc/com//index.wasm", "application/wasm")]
        [InlineData("/bine/etc/com//index.dll", "application/octet-stream")]
        public void RunMapping(string path, string mime) => Assert.Equal(mime, MimeMap.FromPath(path));

    }
}