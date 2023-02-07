using System;
using System.Buffers;
namespace SimControls.SbpViewer.ValueReaders;

internal class EnumParser : BltParser<int>, ICanParseTo<(int,string)>
{
    private string[] Names { get; }

    public EnumParser(params string[] names)
    {
        Names = names;
    }

    public override bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out string value) =>
        InnerTryParse(ref sourceBytes, out int intVal).WithAssignment(StringFromValue(intVal), out value);


    public bool InnerTryParse(ref SequenceReader<byte> sourceBytes, out (int, string) value) =>
        InnerTryParse(ref sourceBytes, out int intVal).WithAssignment(
            (intVal, StringFromValue(intVal)), out value);

    public string StringFromValue(int intValue) =>
        intValue >= 0 && intValue < Names.Length ? Names[intValue] : "<Undefined>";
}