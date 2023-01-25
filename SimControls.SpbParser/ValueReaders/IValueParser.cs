using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Melville.INPC;
using SimControls.SpbParser.PropertyAndSetDeclarations;

namespace SimControls.SpbParser.ValueReaders;

public interface IValueParser
{
    bool Skip(ref SequenceReader<byte> sourceBytes);
    bool TryParse<T>(ref SequenceReader<byte> sourceBytes, out T value);
    string? TryParseToString(ref SequenceReader<byte> sourceBytes, PropertyDecl property);
}

public interface ICanParseTo<T>
{
    bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out T value);
}

internal abstract partial class ValueParser<T> : IValueParser, ICanParseTo<T>
{
    public bool TryParse<T1>(ref SequenceReader<byte> sourceBytes, out T1 value)
    {
        if (this is not ICanParseTo<T1> casted)
            throw new InvalidOperationException(
                $"Attempted to call TryParse with {typeof(T1)}, but {typeof(T)} was expected");
        return casted.InnerTryParse(ref sourceBytes, out value);
    }

    public string? TryParseToString(ref SequenceReader<byte> sourceBytes, PropertyDecl property)
    {
        if (!InnerTryParse(ref sourceBytes, out var result)) return null;
        return result?.ToString() ?? "<<Parsed a Null Result>>";
    }

    public abstract bool Skip(ref SequenceReader<byte> sourceBytes);
    public abstract bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out T value);
}

internal class BoolParser : ValueParser<bool>
{
    public override bool Skip(ref SequenceReader<byte> sourceBytes) =>
        sourceBytes.TryAdvance(4);

    public override bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out bool value) => 
        sourceBytes.TryReadBlt(out uint intValue).WithAssignment(intValue != 0, out value);
}

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
[MacroItem("GUID", "BltParser<Guid>")]
[MacroItem("PBH", "BltParser<PbhStruct>")]
[MacroItem("PBH32", "BltParser<PbhStruct>")]
[MacroItem("LLA", "BltParser<LlaStruct>")]
[MacroCode("public static readonly IValueParser Parse~0~ = new ~1~();")]
internal static partial class ValueReaderFactory
{
}