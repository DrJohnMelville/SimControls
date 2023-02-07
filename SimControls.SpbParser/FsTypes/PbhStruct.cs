namespace SimControls.SpbParser.FsTypes;

#pragma warning disable 0649
// These structures are read out of the file in unsafe code that the compiler does not know about so the
// field is never writeen warning is inaccurate.

public readonly struct PbhStruct
{
    private readonly uint p;
    private readonly uint b;
    private readonly uint h;
    private readonly uint pad;

    private const double Factor = 360.0/ uint.MaxValue;
    public double P => p * Factor;
    public double B => b * Factor;
    public double H => h * Factor;

    public override string ToString() => $"(P: {P:##0.0}, B: {B:##0.0}, H: {H:##0.0}, Pad: {pad})";
}