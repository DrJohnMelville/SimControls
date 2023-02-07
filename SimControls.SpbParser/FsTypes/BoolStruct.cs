namespace SimControls.SpbParser.FsTypes;

#pragma warning disable 0649
public readonly struct BoolStruct
{
    private readonly int value;

    public BoolStruct(bool b)
    {
        value = b?1:0;
    }

    public override string ToString() => value == 0 ? "false" : "true";
}