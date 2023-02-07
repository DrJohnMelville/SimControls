using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Melville.INPC;

namespace SimControls.SpbParser;

public static class ReadOnlySequenceExtension
{
    public static unsafe void Read<T>(this ReadOnlySequence<byte> seq, out T value)
        where T : unmanaged
    {
        fixed (T* ptr = &value)
        {
            var span = new Span<byte>(ptr, sizeof(T));
            Debug.Assert(seq.Length >= span.Length);
            seq.CopyTo(span);
        }
    }

    public static T Read<T>(this ReadOnlySequence<byte> seq) where T : unmanaged
    {
        seq.Read(out T ret);
        return ret;
    }
}