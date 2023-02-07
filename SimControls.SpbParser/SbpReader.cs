using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Melville.INPC;
using SimControls.SpbParser.PropertyAndSetDeclarations;

namespace SimControls.SpbParser;

public partial class SbpReader
{
    [FromConstructor]private readonly PipeReader pipe;
    [FromConstructor] private readonly IParseTarget target;
    [FromConstructor] private readonly IPropertyRegistry properties;
    private const int headerBlockSize = (4*7)+2;

    public async ValueTask Read()
    {
        var blockResult = await pipe.ReadAtLeastAsync(headerBlockSize);
        var tagTable = ParseHeaderBlock(blockResult.Buffer);
        pipe.AdvanceTo(blockResult.Buffer.GetPosition(headerBlockSize));

        blockResult = await pipe.ReadAtLeastAsync(tagTable.DiskSizeInBytes);
        tagTable.ReadTags(blockResult.Buffer);
        pipe.AdvanceTo(blockResult.Buffer.GetPosition(tagTable.DiskSizeInBytes));
    }

    private TagTable ParseHeaderBlock(ReadOnlySequence<byte> buffer)
    {
        var reader = new SequenceReader<byte>(buffer);
        if (!reader.TryReadBlt(out ushort flag) || flag != 0xEBAC)
            throw new InvalidDataException("Invalid SBP checksum");
        reader.Advance(4*6);
        if (!reader.TryReadBlt(out uint numberOfTags))
            throw new InvalidDataException("Cannot Read Tag Count");

        return new TagTable(numberOfTags, properties);
    }
}