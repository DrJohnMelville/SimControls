using System;
using System.Buffers;
using System.Text;

namespace SimControls.SbpViewer.ValueReaders;

internal class StringParser : ValueParser
{
    public override bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out string value)
    {
        value = "";
        var tempReader = sourceBytes;
        if (!tempReader.TryReadBlt(out int length)) return false;
        if (length <= 0)
        {
        }
        else
        {
            if (length > tempReader.Remaining)
                return false.WithAssignment("", out value);
            value = Encoding.Unicode.GetString(tempReader.UnreadSequence.Slice(0, length));
            tempReader.Advance(length);
        }
        sourceBytes = tempReader;
        return true;
    }

    public override bool Skip(ref SequenceReader<byte> sourceBytes)
    {
        var tempReader = sourceBytes;
        if (!(tempReader.TryReadBlt(out int length) && tempReader.TryAdvance(Math.Max(0,length))))
            return false;
        sourceBytes = tempReader;
        return true;
    }

    public override string TypeString => "String";
}