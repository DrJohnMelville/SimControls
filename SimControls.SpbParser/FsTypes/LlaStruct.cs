namespace SimControls.SpbParser.FsTypes;

#pragma warning disable 0649
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