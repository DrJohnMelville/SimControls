using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Threading.Tasks;
using Melville.Hacks;
using Melville.INPC;

namespace SimControls.SpbParser;

public interface ISingleField
{
    Guid Guid { get; }
    int Size { get; }
    ValueTask<ReadOnlySequence<byte>> GetByteSequence();
    ValueTask Skip();
    void PushSetDeclaration(IParseTarget target);
}

internal partial class FieldVisitor
{
    private readonly CountingPipe pipe;
    private readonly TagTable tags;
    private readonly Stack<(IParseTarget Target, long EndPosition)> context = new();

    public FieldVisitor(PipeReader pipe, TagTable tags, IParseTarget initialTarget)
    {
        this.pipe = new CountingPipe(pipe);
        this.tags = tags;
        context.Push((initialTarget, long.MaxValue));
    }

    public async ValueTask Parse()
    {
        var field = new SingleField(this);
        do
        { 
            var next = tags[await ReadInt()];
            field.InitializeSingleField(next.Guid, await ComputeLength(next.ItemLength));
            await context.Peek().Target.ParseItem(field);
            await field.VerifyOperationDone();
            TryEndSets();
        } while (context.Count > 1);
    }


    private async ValueTask<int> ReadInt()
    {
        var buf = await pipe.GetBytes(4);
        var ret = buf.Read<int>();
        pipe.Consume();
        return ret;
    }

    private async ValueTask<int> ComputeLength(uint declaredLength) =>
        IsFixedLengthElement(declaredLength) ? (int)declaredLength : await ReadInt();

    private static bool IsFixedLengthElement(uint declaredLength) =>
        declaredLength != uint.MaxValue;
    private void TryEndSets()
    {
        while (context.Peek().EndPosition <= pipe.Position)
        {
            var (item, _) = context.Pop();
            item.EndScope();
        }
    }
}

