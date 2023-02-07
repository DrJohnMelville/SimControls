using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using SimControls.SbpViewer.ValueReaders;

namespace SimControls.SbpViewer.PropertyAndSetDeclarations;

internal static class PropertyRegistryParser
{
    public static async Task<PropertyRegistry> ParseAsync(Stream source)
    {

        return new PropertyRegistry( await ParseSingleFile(source));
    }
    public static async Task<IPropertyRegistry> ParseAsync(IEnumerable<Stream> source)
    {
        IEnumerable<ISetOrProperty> ret = Array.Empty<ISetOrProperty>();
        foreach (var item in source)
        {
            ret = ret.Concat(await ParseSingleFile(item));
        }

        return new PropertyRegistry(ret);
    }

    private static async Task<IEnumerable<ISetOrProperty>> ParseSingleFile(Stream source)
    {
        using (source)
        {
            var xml = await XElement.LoadAsync(source, LoadOptions.None, CancellationToken.None);

            return xml.Descendants("SetDef").Select(ParseSet)
                .Concat(xml.Descendants("PropertyDef").Select(ParseProperty));

        }
    }

    private static ISetOrProperty ParseSet(XElement arg) =>
        new SetDecl(ParseIdGuid(arg),
            AttributeOrBlank(arg, "name"), AttributeOrBlank(arg, "descr"));

    private static Guid ParseIdGuid(XElement arg) => 
        Guid.Parse(MandatoryAttribute(arg, "id").AsSpan()[1..^1]);

    private static string MandatoryAttribute(XElement arg, string name) =>
        (arg.Attribute(name) ?? arg.Attribute(name.ToUpper()) ??
         throw new InvalidDataException("Id GUID expected")).Value;

    private static string AttributeOrBlank(XElement arg, string name) => 
        arg.Attribute(name)?.Value ??"";

    private static ISetOrProperty ParseProperty(XElement arg) =>
        new PropertyDecl(ParseIdGuid(arg), 
            AttributeOrBlank(arg, "name"),
            AttributeOrBlank(arg, "descr"),
            AttributeOrBlank(arg, "default"),
            ParseType(arg));

    private static ValueParser ParseType(XElement item) => 
        AttributeOrBlank(item, "type").ToUpper() switch
    {
        "" => ValueReaderFactory.ParseUNDEFINED,
        "ENUM" => ParseEnum(item),
        var key=> ValueReaderFactory.Create(key)
    };

    private static ValueParser ParseEnum(XElement item)
    {
        return new EnumParser(item
            .Descendants("EnumVal")
            .Select(e => AttributeOrBlank(e, "name"))
            .ToSmallArray()
        );
    }
}