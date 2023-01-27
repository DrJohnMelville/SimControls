using Melville.INPC;

namespace SimControls.SpbParser.ValueReaders;

[MacroItem("LONG", "BltParser<int>")]
[MacroItem("LONG2", "BltParser<(int,int)>")]
[MacroItem("LONG4", "BltParser<(int,int,int,int)>")]
[MacroItem("ULONG", "BltParser<uint>")]
[MacroItem("BOOL", "BoolParser")]
[MacroItem("FLOAT", "BltParser<float>")]
[MacroItem("FLOAT2", "BltParser<(float,float)>")]
[MacroItem("FLOAT4", "BltParser<(float,float,float,float)>")]
[MacroItem("DOUBLE", "BltParser<double>")]
[MacroItem("BYTE4", "BltParser<(byte,byte,byte,byte)>")]
[MacroItem("GUID", "BltParser<System.Guid>")]
[MacroItem("PBH", "BltParser<PbhStruct>")]
[MacroItem("LLA", "BltParser<LlaStruct>")]
[MacroItem("TEXT", "StringParser")]
[MacroCode("public static readonly ValueParser Parse~0~ = new ~1~();")]
internal static partial class ValueReaderFactory
{
    public static ValueParser ParsePHB32 => ParsePBH;
    public static ValueParser ParseMLTEXT => ParseTEXT;
}