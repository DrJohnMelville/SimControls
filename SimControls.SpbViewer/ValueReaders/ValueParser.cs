using System;
using System.Buffers;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Melville.INPC;
using SimControls.SpbParser;

namespace SimControls.SbpViewer.ValueReaders;

public interface ICanParseTo<T>
{
}

public abstract class ValueParser
{
    public abstract ValueTask<string> Parse(ISingleField field);
    public abstract string TypeString { get; }
}

internal abstract class ValueParser<T> : ValueParser, ICanParseTo<T>
{

    public override string TypeString => typeof(T).Name;
}

public partial class UndefinedValueParser: ValueParser
{
    [FromConstructor] private readonly string name;
    public override async ValueTask<string> Parse(ISingleField field)
    {
        var seq = await field.GetByteSequence();
        return String.Create((int)seq.Length * 2, seq, PrintHex);
    }

    private void PrintHex(Span<char> span, ReadOnlySequence<byte> arg)
    {
        var reader = new SequenceReader<byte>(arg);
        for (int i = 0; i < arg.Length; i++)
        {
            reader.TryRead(out byte b);
            b.TryFormat(span.Slice(2 * i), out _, "X2");
        }
    }

    public override string TypeString => name;
}