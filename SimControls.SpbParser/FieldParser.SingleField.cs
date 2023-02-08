using Melville.INPC;
using System.Buffers;
using System.Threading.Tasks;
using System;
using Melville.Hacks;

namespace SimControls.SpbParser;

internal partial class FieldVisitor
{
    public partial class SingleField : ISingleField
    {
        [FromConstructor] private readonly FieldVisitor parent;

        public void InitializeSingleField(Guid guid, int size)
        {
            Guid = guid;
            Size = size;
        }

        private void VerifyCanDoOperation(OperationState next)
        {
            if (needsOperation != OperationState.Ready)
                throw new InvalidOperationException(
                    "Multiple calls to GetByteSequence, Skip, or PushDeclaration detected.");
            needsOperation = next;
        }

        public void VerifyOperationDone()
        {
            switch (needsOperation)
            {
                case OperationState.Read:
                    parent.pipe.Consume();
                    break;
                case OperationState.Pushed:
                    break;
                default:
                    throw new InvalidOperationException(
                        "Missed a required call to GetByteSequence, Skip, or PushDeclaration.");
            }

            needsOperation = OperationState.Ready;
        }

        private OperationState needsOperation = OperationState.Ready;
        public Guid Guid { get; private set; }
        public int Size { get; private set; }

        public ValueTask<ReadOnlySequence<byte>> GetByteSequence()
        {
            VerifyCanDoOperation(OperationState.Read);
            return parent.pipe.GetBytes(Size);
        }

        public ValueTask Skip()
        {
            return GetByteSequence().AsValueTask();
        }

        public void PushSetDeclaration(IParseTarget target)
        {
            VerifyCanDoOperation(OperationState.Pushed);
            parent.context.Push((target, parent.pipe.Position + Size));
        }

        private enum OperationState
        {
            Ready,
            Read,
            Pushed
        }
    }
}