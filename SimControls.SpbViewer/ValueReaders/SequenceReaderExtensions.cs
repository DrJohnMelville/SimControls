using System;
using System.Buffers;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class SequenceReaderExtensions
{
    public static unsafe bool TryReadBlt<T>(
        this ref SequenceReader<byte> sourceBytes, out T value) where T : unmanaged
    {
        fixed (T* outPtr = &value)
        {
            return sourceBytes.TryConsumeBytes(new Span<byte>(outPtr, sizeof(T)));
        }
    }

    public static bool TryConsumeBytes(this ref SequenceReader<byte> source, Span<byte> output)
    {
        if (!source.TryCopyTo(output)) return false;
        source.Advance(output.Length);
        return true;
    }

    public static unsafe bool TryAdvanceOver<T>(
        this ref SequenceReader<byte> sourceBytes) where T:unmanaged =>
        sourceBytes.TryAdvance(sizeof(T));

    public static bool TryAdvance(this ref SequenceReader<byte> sourceBytes, long delta)
    {
        if (delta < 0 || delta > sourceBytes.Remaining) return false;
        sourceBytes.Advance(delta);
        return true;
    }
}