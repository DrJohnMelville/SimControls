using System;
using System.Buffers;
using System.Threading.Tasks;
using SimControls.SpbParser;

namespace SimControls.SbpViewer.ValueReaders;

internal class EnumParser : BltParser<int>, ICanParseTo<(int,string)>
{
    private string[] Names { get; }

    public EnumParser(params string[] names)
    {
        Names = names;
    }

    public override async ValueTask<string> Parse(ISingleField field)
    {
        return StringFromValue((await field.GetByteSequence()).Read<int>());
    }

    public string StringFromValue(int intValue) => $"{EnumName(intValue)} ({intValue})";

    private string EnumName(int intValue) => 
        intValue >= 0 && intValue < Names.Length ? Names[intValue] : "<Undefined>";
}