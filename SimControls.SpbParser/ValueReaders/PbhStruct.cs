using System;
using Microsoft.VisualBasic.CompilerServices;

namespace SimControls.SpbParser.ValueReaders;

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

public readonly struct LlaStruct
{
    private readonly long latitude;
    private readonly long longitude;
    private readonly uint altitude;
    private readonly int altitude2;

    public double Latitude => latitude*(90.0 / (10001750.0 * 65536.0 * 65536.0));
    public double Longitude => longitude * (360.0 / (65536.0 * 65536.0 * 65536.0 * 65536.0));
    public double Altitude => altitude2 + (altitude / (65536.0 * 65536.0));

    public override string ToString() => $"(Lat: {Latitude}, Lon: {Longitude}, Alt:{Altitude}, )";
}

public readonly struct FileTime
{
    // this class is a total guess that Spb filetime records are just the filetime structure
    //from minwinbase written directly out to a file.
    public readonly UInt32 LowFileTime;
    public readonly UInt32 HighFileTime;
}