using System;
using System.Buffers;
using System.Linq;
using System.Runtime.InteropServices;
using SimControls.SpbParser.PropertyAndSetDeclarations;

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
    private readonly IPropertyRegistry propertyRegistry;

    public unsafe int DiskSizeInBytes => sizeof(TagData) * tags.Length;

    public TagTable(uint size, IPropertyRegistry registry)
    {
        tags = new TagData[size];
        propertyRegistry = registry;
    }

    public void ReadTags(ReadOnlySequence<byte> source)
    {
        var sr = new SequenceReader<byte>(source);
        sr.TryConsumeBytes(MemoryMarshal.Cast<TagData, byte>(tags.AsSpan()));
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
            tags.Select(FormatLine));
    }

    private string FormatLine(TagData arg)
    {
        var prop = propertyRegistry.Get(arg.Guid);

        return $"{arg.Flags:X}, {prop}";
    }
}