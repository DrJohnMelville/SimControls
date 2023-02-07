using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SimControls.SpbParser;

namespace SimControls.SbpViewer.ValueReaders;

internal class StringParser : ValueParser
{
    public override async ValueTask<string> Parse(ISingleField field)
    {
        var buf = await field.GetByteSequence();
        return string.Create((int)(buf.Length / 2), buf, StringFromBuf);
    }

    private void StringFromBuf(Span<char> span, ReadOnlySequence<byte> arg) => 
        arg.CopyTo(MemoryMarshal.Cast<char,byte>(span));

    public override string TypeString => "String";
}