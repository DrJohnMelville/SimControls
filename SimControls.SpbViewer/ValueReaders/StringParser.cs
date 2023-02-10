using System;
using System.Buffers;
using System.Runtime.InteropServices;
using SimControls.SpbParser;

namespace SimControls.SpbViewer.ValueReaders;

internal class StringParser : ValueParser
{
    public override async ValueTask<string> Parse(ISingleField field)
    {
        var buf = await field.GetByteSequence();
        return TextDecode.Decode(buf);
//        return string.Create((int)(buf.Length / 2), buf, StringFromBuf);
    }

    private void StringFromBuf(Span<char> span, ReadOnlySequence<byte> arg) =>
        arg.Fill(MemoryMarshal.Cast<char,byte>(span));

    public override string TypeString => "String";
}

