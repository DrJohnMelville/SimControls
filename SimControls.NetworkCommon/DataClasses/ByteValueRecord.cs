using System.Buffers;
using System.IO.Pipelines;
using Melville.P2P.Raw.BinaryObjectPipes;

namespace SimControls.NetworkCommon.DataClasses
{
    public record ByteValueRecord(ushort Index, byte Value): IValueRecord
    {
        public void WriteToPipe(PipeWriter write)
        {
            using var mem = new SerialPipeWriter(write, sizeof(ushort) + 1);
            mem.Write(Index);
            mem.Write(Value);
        }

        public static ByteValueRecord? ReadFromPipe(ref SequenceReader<byte> src) =>
            src.TryReadLittleEndian(out ushort index) && src.TryReadLittleEndian(out byte value)
                ? new ByteValueRecord(index, value)
                : null;
    }
}