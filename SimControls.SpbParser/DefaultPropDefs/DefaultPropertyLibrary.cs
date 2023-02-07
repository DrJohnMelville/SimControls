using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SimControls.SpbParser.PropertyAndSetDeclarations;

namespace SimControls.SpbParser.DefaultPropDefs;

public static class DefaultPropertyLibrary
{
    public static Stream GetDefaultStream(string fileName) => File.OpenRead(PathFor(fileName));

    public static IEnumerable<Stream> GetAllStreams() =>
        Directory.EnumerateFiles(PathFor(""), "*.xml").Select(i => File.Open(i, FileMode.Open));

    private static string PathFor(string fileName) =>
        Path.Combine(Path.GetDirectoryName(typeof(DefaultPropertyLibrary).Assembly.Location)!,
            "DefaultPropDefs", fileName);

    public static Task<IPropertyRegistry> GlobalRegistryAsync() =>
        PropertyRegistryParser.ParseAsync(GetAllStreams());
}