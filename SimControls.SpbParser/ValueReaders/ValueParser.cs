using System;
using System.Buffers;

namespace SimControls.SpbParser.ValueReaders;

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
}

internal abstract class ValueParser<T> : ValueParser, ICanParseTo<T>
{
    public abstract bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out T value);
    public override bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out string value) =>
        InnerTryParse(ref sourceBytes, out T result).WithAssignment(
            result?.ToString()??"<Value is Null>", out value);

}