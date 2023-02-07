using System;
using System.Buffers;
using System.IO;
using Melville.INPC;

namespace SimControls.SbpViewer.ValueReaders;

public interface ICanParseTo<T>
{
    bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out T value);
}

public abstract class ValueParser : ICanParseTo<string>
{
    public abstract bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out string value);
    public abstract bool Skip(ref SequenceReader<byte> sourceBytes);
    public bool TryParse<T>(ref SequenceReader<byte> sourceBytes, out T value)
    {
        if (this is not ICanParseTo<T> casted)
            throw new InvalidOperationException($"Cannot parse an a {typeof(T)} from {GetType()}");
        return casted.InnerTryParse(ref sourceBytes, out value);
    }

    public abstract string TypeString { get; }
}

internal abstract class ValueParser<T> : ValueParser, ICanParseTo<T>
{
    public abstract bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out T value);
    public override bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out string value) =>
        InnerTryParse(ref sourceBytes, out T result).WithAssignment(
            result?.ToString()??"<Value is Null>", out value);

    public override string TypeString => typeof(T).Name;
}

public partial class UndefinedValueParser: ValueParser
{
    [FromConstructor] private readonly string name;

    public override bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out string value) => 
        throw new InvalidDataException($"No parser type is defined for: {name}.");

    public override bool Skip(ref SequenceReader<byte> sourceBytes) =>
        throw new InvalidDataException($"No parser type is defined for: {name}.");

    public override string TypeString => name;
}