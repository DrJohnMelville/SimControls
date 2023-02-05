using System;
using System.Buffers;
using Melville.INPC;
using SimControls.SpbParser.PropertyAndSetDeclarations;
using SimControls.SpbParser.ValueReaders;

namespace SimControls.SpbParser;

public ref struct PropertyAccessor
{
    private enum AccessorState { UnRead, Succeeded, Failed }
    private AccessorState state = AccessorState.UnRead;
    #pragma warning disable 8500
    private unsafe readonly SequenceReader<byte>* data;
    private unsafe ref SequenceReader<byte> Data => ref *data;
    public PropertyDecl Property { get; }

    internal unsafe PropertyAccessor(ref SequenceReader<byte> data, PropertyDecl property)
    {
        fixed (SequenceReader<byte>* datePtr = &data)
        {
            this.data = datePtr;
        }
        this.Property= property;  
    }

    public bool Skip() => UpdateState(Property.Parser.Skip(ref Data));
    public bool Read<T>(out T value) => UpdateState(Property.Parser.TryParse(ref Data, out value));

    private bool UpdateState(bool success)
    {
        if (state != AccessorState.UnRead)
            throw new InvalidOperationException("Cannot Read a property twice.");
        state = success?AccessorState.Succeeded:AccessorState.Failed;
        return success;
    }

    internal bool ReadSuceeded() => state switch
    {
        AccessorState.Succeeded => true,
        AccessorState.Failed => false,
        _ => throw new InvalidOperationException("Push property must read or skip property"),
    };

    public bool PropertyDisplayString(out string value) =>
        Read(out string valueString).WithAssignment($"{Property.Name}: {valueString}", out value);
}

public interface IParseTarget
{ 
    IParseTarget BeginSet (SetDecl set);
    void EndSet (SetDecl set);
    void PushProperty(ref PropertyAccessor accessor);
}