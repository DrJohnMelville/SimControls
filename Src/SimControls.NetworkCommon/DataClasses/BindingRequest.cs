using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using Melville.P2P.Raw.BinaryObjectPipes;
using SequenceReaderExtensions = Melville.P2P.Raw.BinaryObjectPipes.SequenceReaderExtensions;

namespace SimControls.NetworkCommon.DataClasses
{
    public record BindingRequest(ushort Index): ICanWriteToPipe
    {
           
        public void WriteToPipe(PipeWriter write)
        {
            using var writer = new SerialPipeWriter(write, sizeof(ushort));
            writer.Write(Index);
        }

        public static BindingRequest? ReadFromPipe(ref SequenceReader<byte> src) =>
            src.TryReadLittleEndian(out ushort index)
                ? new BindingRequest(index)
                : null;
    }

    public record TerminateConnection():ICanWriteToPipe
    {
        public static TerminateConnection? ReadFromPipe(ref SequenceReader<byte> src) => new();
        public void WriteToPipe(PipeWriter write)
        {
        }
    }

    public record DoubleValueRecord(ushort Index, double Value) : ICanWriteToPipe
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