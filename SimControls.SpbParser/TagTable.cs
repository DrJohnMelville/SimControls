using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace SimControls.SpbParser;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct TagData
{
    public readonly Guid Guid;
    public readonly uint Flags;
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
        var sr = new SequenceReader<byte>(source);
        sr.TryConsumeBytes(MemoryMarshal.Cast<TagData, byte>(tags.AsSpan()));
    }
}