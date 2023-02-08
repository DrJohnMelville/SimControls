using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Melville.INPC;

namespace SimControls.SpbParser;

public partial class CountingPipe
{
    [FromConstructor] private readonly PipeReader reader;
    private ReadOnlySequence<byte> current;
    public long Position { get; private set; }

    public async ValueTask<ReadOnlySequence<byte>> GetBytes(int length) =>
        current = (await reader.ReadAtLeastAsync(length)).Buffer.Slice(0,length);

    public void Consume()
    {
        Position += current.Length;
        reader.AdvanceTo(current.End);
    }
}