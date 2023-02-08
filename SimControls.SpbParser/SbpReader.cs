using System.Buffers;
using System.Drawing;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Melville.INPC;

namespace SimControls.SpbParser;

public partial class SbpReader
{
    [FromConstructor]private readonly PipeReader pipe;
    [FromConstructor] private readonly IParseTarget target;
    private const int headerBlockSize = (4*7)+2;
    private const int TagCountOffset = 26;

    public async ValueTask Read()
    {
        var blockResult = await pipe.ReadAtLeastAsync(headerBlockSize);
        var tagTable = ParseHeaderBlock(blockResult.Buffer);
        pipe.AdvanceTo(blockResult.Buffer.GetPosition(headerBlockSize));

        blockResult = await pipe.ReadAtLeastAsync(tagTable.DiskSizeInBytes);
        tagTable.ReadTags(blockResult.Buffer);
        pipe.AdvanceTo(blockResult.Buffer.GetPosition(tagTable.DiskSizeInBytes));

        await new FieldVisitor(pipe, tagTable, target).Parse();
    }

    private TagTable ParseHeaderBlock(ReadOnlySequence<byte> buffer)
    {
        if (buffer.Read<ushort>() != 0xEBAC)
            throw new InvalidDataException("Invalid SBP checksum");
        return new TagTable(buffer.Slice(TagCountOffset).Read<uint>());
    }

}

public interface IParseTarget
{
    ValueTask ParseItem(ISingleField field);
    void EndScope();
}