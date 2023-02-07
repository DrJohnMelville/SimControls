using System.Buffers;

namespace SimControls.SbpViewer.ValueReaders;

internal class BltParser<T> : ValueParser<T> where T: unmanaged 
{
    public override bool Skip(ref SequenceReader<byte> sourceBytes) =>
        sourceBytes.TryAdvanceOver<T>();
   
    public override bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out T value) =>
        sourceBytes.TryReadBlt(out value);
}