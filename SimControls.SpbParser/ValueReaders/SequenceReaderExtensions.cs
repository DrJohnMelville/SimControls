using System;
using System.Buffers;
using System.Runtime.InteropServices;

public static class SequenceReaderExtensions
{
    public static unsafe bool TryReadBlt<T>(
        this ref SequenceReader<byte> sourceBytes, out T value) where T : unmanaged
    {
        fixed (T* outPtr = &value)
        {
            Span<byte> buff2 = new Span<byte>(outPtr, sizeof(T));
            return sourceBytes.TryCopyTo(buff2);
        }
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