using System;
using System.Buffers;
using System.Linq;
using System.Runtime.InteropServices;

namespace SimControls.SpbParser;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct TagData
{
    public readonly Guid Guid;
    public readonly uint ItemLength;
}

internal readonly struct TagTable
{
    private readonly TagData[] tags;

    public unsafe int DiskSizeInBytes => sizeof(TagData) * tags.Length;

    public TagTable(uint size)
    {
        tags = new TagData[size];
    }

    public void ReadTags(ReadOnlySequence<byte> source)
    {
        source.CopyTo(MemoryMarshal.Cast<TagData, byte>(tags.AsSpan()));
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
            tags.Select(FormatLine));
    }

    private string FormatLine(TagData arg)
    {
        return $"{arg.ItemLength:X}, {arg.Guid}";
    }
}