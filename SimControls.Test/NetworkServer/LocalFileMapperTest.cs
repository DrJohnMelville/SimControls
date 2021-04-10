using Melville.FileSystem;
using Melville.Mvvm.TestHelpers.MockFiles;
using SimControls.NetworkServer;
using Xunit;

namespace SimControls.Test.NetworkServer
{
    public class LocalFileMapperTest
    {
        private readonly MockDirectory dir = new ("d:\\ssss");
        private readonly LocalFileMapper sut;

        public LocalFileMapperTest()
        {
            sut = new LocalFileMapper(dir);
        }

        private void VerifyResponse(string query, string? acceptEncoding, IFile? f, string? desiredEncoding)
        {
            var (file, encoding) = sut.GetResponseFile(query, acceptEncoding);
            Assert.Equal(f, file);
            Assert.Equal(desiredEncoding, encoding);
        }

        [Fact] public void GrabFile()
        {
            var f = dir.File("xx.htm");
            f.Create("sss");
            VerifyResponse("xx.htm", "gzip, deflate", f, null);
        }
        [Fact] public void NullIfNoFile()
        {
            VerifyResponse("xx.htm", "gzip, deflate", null, null);
        }

        [Fact]
        public void GZipWhenAvailable()
        {
            var f = dir.File("xx.htm");
            f.Create("sss");
            var gz = dir.File("xx.htm.gz");
            gz.Create("sss");
            VerifyResponse("xx.htm", "gzip, deflate", gz, "gzip");
        }
        [Fact]
        public void ProcessInOrder()
        {
            var f = dir.File("xx.htm");
            f.Create("sss");
            dir.File("xx.htm.gz.deflate").Create("lg sabj");
            var gz = dir.File("xx.htm.gz");
            gz.Create("sss");
            VerifyResponse("xx.htm", "gzip, deflate", gz, "gzip");
        }
        [Fact]
        public void FallbackIfNoEncodingsMatch()
        {
            var f = dir.File("xx.htm");
            f.Create("sss");
            dir.File("xx.htm.gz.deflate").Create("lg sabj");
            var gz = dir.File("xx.htm.gz");
            gz.Create("sss");
            VerifyResponse("xx.htm", "enca, encb", f, null);
        }
        [Fact]
        public void DeflateWhenAvailable()
        {
            var f = dir.File("xx.htm");
            f.Create("sss");
            var gz = dir.File("xx.htm.deflate");
            gz.Create("sss");
            VerifyResponse("xx.htm", "gzip, deflate", gz, "deflate");
        }
        [Fact]
        public void CompressoionOnlyIfRequested()
        {
            var f = dir.File("xx.htm");
            f.Create("sss");
            var gz = dir.File("xx.htm.gz");
            gz.Create("sss");
            VerifyResponse("xx.htm", "deflate", f, null);
        }


        [Fact] public void GrabFileAtPath()
        {
            var subDirectory = dir.SubDirectory("dir");
            subDirectory.Create();
            var f = subDirectory.File("xx.htm");
            f.Create("sss");
            var (file, encoding) = sut.GetResponseFile(@"\dir\xx.htm", "gzip, deflate");
            Assert.Equal(f, file);
            Assert.Null(encoding);            
        }
    }
}