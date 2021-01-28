using System.Buffers;
using System.IO.Pipelines;
using Melville.P2P.Raw.BinaryObjectPipes;

namespace SimControls.NetworkCommon.DataClasses
{
    public record TerminateConnection():ICanWriteToPipe
    {
        public static TerminateConnection? ReadFromPipe(ref SequenceReader<byte> src) => new();
        public void WriteToPipe(PipeWriter write)
        {
        }
    }
}