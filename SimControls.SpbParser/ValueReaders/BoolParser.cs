using System.Buffers;

namespace SimControls.SpbParser.ValueReaders;

internal class BoolParser : ValueParser<bool>
{
    public override bool Skip(ref SequenceReader<byte> sourceBytes) =>
        sourceBytes.TryAdvance(4);

    public override bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out bool value) => 
        sourceBytes.TryReadBlt(out uint intValue).WithAssignment(intValue != 0, out value);
}