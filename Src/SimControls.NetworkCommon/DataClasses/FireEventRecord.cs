using System.Buffers;
using System.IO.Pipelines;
using Melville.P2P.Raw.BinaryObjectPipes;

namespace SimControls.NetworkCommon.DataClasses
{
    public record FireEventRecord(string EventName, uint Value): ICanWriteToPipe
    {
        public void WriteToPipe(PipeWriter write)
        {
            var len = SerialPipeWriter.SpaceForString(EventName) + sizeof(uint);
            using var mem = new SerialPipeWriter(write, len);
            mem.Write(EventName);
            mem.Write(Value);
        }

        public static FireEventRecord? ReadFromPipe(ref SequenceReader<byte> src) =>
            src.TryRead(out string? eventName) && src.TryReadLittleEndian(out uint value)
                ? new FireEventRecord(eventName, value)
                : null;
    }
}