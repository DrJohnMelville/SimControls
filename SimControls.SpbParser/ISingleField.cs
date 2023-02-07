using System;
using System.Buffers;
using System.Threading.Tasks;

namespace SimControls.SpbParser;

public interface ISingleField
{
    Guid Guid { get; }
    int Size { get; }
    ValueTask<ReadOnlySequence<byte>> GetByteSequence();
    ValueTask Skip();
    ValueTask PushSetDeclaration(IParseTarget target);
}
