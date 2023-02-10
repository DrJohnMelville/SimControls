using System; // needed for the ArgumentException type
using SimControls.SpbParser.FsTypes;

namespace SimControls.SpbViewer.ValueReaders;

[MacroItem("UNDEFINED", "UndefinedValueParser(\"Undefined\");//")]
[MacroItem("LONG", "BltParser<int>")]
[MacroItem("LONG2", "BltParser<(int,int)>")]
[MacroItem("LONG4", "BltParser<(int,int,int,int)>")]
[MacroItem("ULONG", "BltParser<uint>")]
[MacroItem("BOOL", "BltParser<BoolStruct>")]
[MacroItem("FLOAT", "BltParser<float>")]
[MacroItem("FLOAT2", "BltParser<(float,float)>")]
[MacroItem("FLOAT3", "BltParser<(float,float,float)>")]
[MacroItem("FLOAT4", "BltParser<(float,float,float,float)>")]
[MacroItem("DOUBLE", "BltParser<double>")]
[MacroItem("BYTE4", "BltParser<(byte,byte,byte,byte)>")]
[MacroItem("GUID", "BltParser<System.Guid>")]
[MacroItem("PBH", "BltParser<PbhStruct>")]
[MacroItem("LLA", "BltParser<LlaStruct>")]
[MacroItem("FILETIME", "BltParser<FileTime>")]
[MacroItem("TEXT", "StringParser")]
[MacroItem("MLTEXT", "StringParser")]
[MacroItem("BEZIERCURVE", "UndefinedValueParser(\"Bezier Curve\");//")]
[MacroItem("COLOR", "UndefinedValueParser(\"Color\");//")]
[MacroCode("public static readonly ValueParser Parse~0~ = new ~1~();")]
[MacroCode("    \"~0~\" => Parse~0~,",
    Prefix = "public static ValueParser Create(ReadOnlySpan<char> key) => key switch \r\n{",
    Postfix = "    _=> SecondChanceLookup(key) \r\n};")]
internal static partial class ValueReaderFactory
{
    private static ValueParser SecondChanceLookup(ReadOnlySpan<char> key) =>
        key switch
        {
            "PBH32" => ParsePBH,
            "OUTPUTVALUE" => ParseGUID,
            "VARIANT" => Create("INPUTFLOAT"),
            ['I', 'N', 'P', 'U', 'T', .. var tail] => Create(tail),
            _ => throw new ArgumentException($"Cannot create parser for: {key}.")
        };
}