using System.Buffers;
using System.Threading.Tasks;
using SimControls.SpbParser;

namespace SimControls.SbpViewer.ValueReaders;

internal class BltParser<T> : ValueParser<T> where T: unmanaged 
{
    public override async ValueTask<string> Parse(ISingleField field) => 
        (await field.GetByteSequence()).Read<T>().ToString()??"";
}