using System;
using System.Buffers;
using SimControls.SpbParser.PropertyAndSetDeclarations;

namespace SimControls.SpbParser.ValueReaders;

public class EnumParser : IValueParser, ICanParseTo<int>, ICanParseTo<string>, ICanParseTo<(int,string)>
{
    private string[] Names { get; }

    public EnumParser(params string[] names)
    {
        Names = names;
    }
    public bool Skip(ref SequenceReader<byte> sourceBytes) => sourceBytes.TryAdvance(4);

    public bool TryParse<T>(ref SequenceReader<byte> sourceBytes, out T value)
    {
        if (this is not ICanParseTo<T> casted)
            throw new InvalidOperationException($"Cannot parse an enum to {typeof(T)}");
        return casted.InnerTryParse(ref sourceBytes, out value);
    }

    public string? TryParseToString(ref SequenceReader<byte> sourceBytes, PropertyDecl property)
    {
        throw new System.NotImplementedException();
    }

    public bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out int value) => 
        sourceBytes.TryReadBlt(out value);


    public bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out string value) =>
        InnerTryParse(ref sourceBytes, out int intVal).WithAssignment(StringFromValue(intVal), out value);


    public bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out (int, string) value) =>
        InnerTryParse(ref sourceBytes, out int intVal).WithAssignment(
            (intVal, StringFromValue(intVal)), out value);

    private string StringFromValue(int intValue) =>
        intValue >= 0 && intValue < Names.Length ? Names[intValue] : "<Undefined>";
}