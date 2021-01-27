using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using Melville.P2P.Raw.BinaryObjectPipes;
using SequenceReaderExtensions = Melville.P2P.Raw.BinaryObjectPipes.SequenceReaderExtensions;

namespace SimControls.NetworkCommon.DataClasses
{
    public record BindingRequest(string Name, string Unit, string SimType, ushort Index): ICanWriteToPipe
    {
           
        public void WriteToPipe(PipeWriter write)
        {
            var len = 2 + 
                      SerialPipeWriter.SpaceForString(Name) + 
                      SerialPipeWriter.SpaceForString(Unit) +
                      SerialPipeWriter.SpaceForString(SimType);
            using var writer = new SerialPipeWriter(write, len);
            writer.Write(Name);
            writer.Write(Unit);
            writer.Write(SimType);
            writer.Write(Index);
        }

        public static BindingRequest? ReadFromPipe(ref SequenceReader<byte> src) =>
            src.TryRead(out string? name) &&
            src.TryRead(out string? unit) &&
            src.TryRead(out string? simType) &&
            src.TryReadLittleEndian(out ushort index)
                ? new BindingRequest(name, unit, simType, index)
                : null;
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