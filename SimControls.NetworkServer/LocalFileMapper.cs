using Melville.FileSystem;

namespace SimControls.NetworkServer
{
    public class LocalFileMapper
    {
        public readonly IDirectory root;
        private readonly AcceptEncodingParser mapper = new();

        public LocalFileMapper(IDirectory root)
        {
            this.root = root;
        }


        public (IFile?, string?) GetResponseFile(string path, string? acceptEncoding)
        {
            
            var baseFile = root.FileAtRelativePath(path);
            if (baseFile == null || !baseFile.Exists()) return (null, null);
            if (acceptEncoding != null)
            {
                foreach (var (ext, type) in mapper.Extensions(acceptEncoding))
                {
                    var candidate = baseFile.Directory?.File(baseFile.Name + ext);
                    if (candidate != null && candidate.Exists())
                    {
                        return (candidate, type);
                    }
                }
            }
            return (baseFile, null);
        }
    }

}