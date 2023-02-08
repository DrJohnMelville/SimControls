using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace SimControls.SbpViewer.ValueReaders;

static partial class TextDecode
{
    static readonly byte[][] S = new byte[256][];
    static readonly char[,] K = new char[256, 250];

    static void Transform()
    {
        for (int i = 0; i < K.GetLength(0); i++)
        {
            for (int j = 0; j < K.GetLength(1); j++)
            {
                K[i, j] = (char)0xff;
            }
        }

        for (int i = 0; i < S.Length; i++)
        {
            var row = S[i];
            if (row != null)
            {
                for (int j = 0; j < row.Length; j++)
                {
                    K[row[j], j] = (char)i;
                }
            }
        }
    }

    public static string Decode(in ReadOnlySequence<byte> input) =>
        string.Create((int)input.Length - 1, input, DecodeMethod);

    private static void DecodeMethod(Span<char> span, ReadOnlySequence<byte> arg)
    {
        var reader = new SequenceReader<byte>(arg);
        for (int i = 0; i < span.Length; i++)
        {
            reader.TryRead(out var source);
            span[i] = K[source, i % 250];
        }

    }

    [Conditional("DEBUG")]
    private static void CheckUnique()
    {
        // vertical
        for (int j = 0; j < 250; j++)
        {
            var bytes = new List<byte>();

            for (int i = 0; i < S.Length; i++)
            {
                var row = S[i];
                if (row != null)
                {
                    bytes.Add(row[j]);
                }
            }

            var u = bytes.Distinct().ToArray();
            var e = Enumerable.Range(0, 256).Select(x => (byte)x).Except(bytes).ToArray();

            Debug.Assert(u.Length == bytes.Count);
            Debug.Assert(u.Length == 195);
            Debug.Assert(e.Length == 61);
        }

        // horizontal
        for (int i = 0; i < S.Length; i++)
        {
            var row = S[i];
            if (row != null)
            {
                var bytes = new List<byte>();

                for (int j = 0; j < 250; j++)
                {

                    bytes.Add(row[j]);
                }

                var u = bytes.Distinct().ToArray();
                var e = Enumerable.Range(0, 256).Select(x => (byte)x).Except(bytes).ToArray();

                Debug.Assert(u.Length == bytes.Count);
                Debug.Assert(u.Length == 250);
                Debug.Assert(e.Length == 6);
                Debug.Assert(e.Contains((byte)i));
            }
        }
    }

    static TextDecode()
    {
        InitData();
        Transform();
        CheckUnique();
    }
}