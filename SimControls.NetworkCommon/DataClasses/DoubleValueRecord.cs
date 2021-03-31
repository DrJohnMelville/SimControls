using System.Buffers;
using System.IO.Pipelines;
using Melville.P2P.Raw.BinaryObjectPipes;

namespace SimControls.NetworkCommon.DataClasses
{
    public interface IValueRecord : ICanWriteToPipe
    {
        ushort Index { get; }
    }
    public record DoubleValueRecord(ushort Index, double Value) : IValueRecord
    {
        public void WriteToPipe(PipeWriter write)
        {
            using var mem = new SerialPipeWriter(write, sizeof(ushort) + sizeof(double));
            mem.Write(Index);
            mem.Write(Value);
        }

        public static DoubleValueRecord? ReadFromPipe(ref SequenceReader<byte> src)
        {
            return src.TryReadLittleEndian(out ushort index) && src.TryReadLittleEndian(out double value)
                ? new DoubleValueRecord(index, value)
                : null;
        }
    }
}