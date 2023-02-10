using SimControls.SpbParser;

namespace SimControls.SpbViewer.ValueReaders;

internal class BltParser<T> : ValueParser<T> where T: unmanaged 
{
    public override async ValueTask<string> Parse(ISingleField field) => 
        (await field.GetByteSequence()).Read<T>().ToString()??"";
}