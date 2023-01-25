using System;
using System.Buffers;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Input;
using SimControls.SpbParser.ValueReaders;
using Xunit;

namespace SimControls.Test.SpbParser.ValueReaders;

public class ValueReaders
{
    [Fact]
    public void RoundTripGuid()
    {
        var result = Guid.NewGuid();
        var buffer = new byte[16];
        result.TryWriteBytes(buffer);
        var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(buffer));
        var parser = new BltParser<Guid>();
        Assert.True(parser.TryParse(ref reader, out Guid pv));
        Assert.Equal(result,pv);
    }
    [Theory]
    [InlineData("00000000", 0)]
    [InlineData("01000000", 1)]
    [InlineData("01010000", 257)]
    [InlineData("FFFFFF7F", int.MaxValue)]
    [InlineData("00000080", int.MinValue)]
    [InlineData("FFFFFFFF", -1)]
    public void QuadToInt(string input, int value)
    {
        var reader = input.BitsFromHex().AsSequenceReader();
        var parser = new BltParser<int>();
        Assert.True(parser.TryParse(ref reader, out int pv));
        Assert.Equal(value, pv);
    }
    [Theory]
    [InlineData("00000000", false)]
    [InlineData("00000001", true)]
    [InlineData("00010000", true)]
    public void QuadToBool(string input, bool value)
    {
        var reader = input.BitsFromHex().AsSequenceReader();
        var parser = new BoolParser();
        Assert.True(parser.TryParse(ref reader, out bool pv));
        Assert.Equal(value, pv);
    }
    [Theory]
    [InlineData("00000000", 0)]
    [InlineData("01000000", 1)]
    [InlineData("01010000", 257)]
    [InlineData("FFFFFFFF", uint.MaxValue)]
    [InlineData("00000000", uint.MinValue)]
    public void QuadToUInt(string input, uint value)
    {
        var reader = input.BitsFromHex().AsSequenceReader();
        var parser = new BltParser<uint>();
        Assert.True(parser.TryParse(ref reader, out uint pv));
        Assert.Equal(value, pv);
    }

    [Fact]
    public void OctetToint2()
    {
        var reader = "01000000 02000000".BitsFromHex().AsSequenceReader();
        var parser = new BltParser<(int,int)>();
        Assert.True(parser.TryParse(ref reader, out (int,int) pv));
        Assert.Equal(1, pv.Item1);
        Assert.Equal(2, pv.Item2);
    }

    [Fact]
    public void SkipOverValue()
    {
        var reader = "12345678 02000000".BitsFromHex().AsSequenceReader();
        var parser = new BltParser<int>();
        parser.Skip(ref reader);
        Assert.True(parser.TryParse(ref reader, out int pv));
        Assert.Equal(2, pv);
    }

    [Fact]
    public void ParsePbh()
    {
        var reader = "00000000 00000080 FFFFFFFF 00000000".BitsFromHex().AsSequenceReader();
        var parser = new BltParser<PbhStruct>();
        Assert.True(parser.InnerTryParse(ref reader, out PbhStruct output));
        Assert.Equal(0.0, output.P, 2);
        Assert.Equal(180, output.B, 2);
        Assert.Equal(360.0, output.H, 2);
        Assert.Equal("(P: 0.0, B: 180.0, H: 360.0)", output.ToString());
    }

    [Theory]
    [InlineData("00000000", 0, "Zero")]
    [InlineData("01000000", 1, "One")]
    [InlineData("02000000", 2, "Two")]
    [InlineData("03000000", 3, "<Undefined>")]
    [InlineData("030FF000", 15732483, "<Undefined>")]
    public void ParseEnum(string source, int value, string name)
    {
        var reader = source.BitsFromHex().AsSequenceReader();
        var parser = new EnumParser("Zero", "One", "Two");
        AssertRead(reader, parser, value);
        AssertRead(reader, parser, name);
        AssertRead(reader, parser, (value,name));
    }

    private static void AssertRead<T>(SequenceReader<byte> reader, EnumParser parser, T value)
    {
        Assert.True(parser.TryParse(ref reader, out T parsedValue));
        Assert.Equal(value, parsedValue);
    }
}