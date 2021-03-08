using System.Buffers;
using System.IO.Pipelines;
using System.Net.Http.Headers;
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
}