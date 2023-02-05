using System;
using System.Buffers;
using SimControls.SpbParser;
using SimControls.SpbParser.PropertyAndSetDeclarations;
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
        AssertAllRead(reader, parser, result);
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
        AssertAllRead(reader, parser, value);
    }
    [Theory]
    [InlineData("00000000", false)]
    [InlineData("00000001", true)]
    [InlineData("00010000", true)]
    public void QuadToBool(string input, bool value)
    {
        var reader = input.BitsFromHex().AsSequenceReader();
        var parser = new BoolParser();
        AssertAllRead(reader, parser, value);
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
        AssertAllRead(reader, parser, value);
    }

    [Fact]
    public void OctetToint2()
    {
        var reader = "01000000 02000000".BitsFromHex().AsSequenceReader();
        var parser = new BltParser<(int,int)>();
        AssertAllRead(reader, parser, (1,2));
    }

    [Fact]
    public void ParsePbh()
    {
        var reader = "00000000 00000080 FFFFFFFF 00000000".BitsFromHex().AsSequenceReader();
        var parser = new BltParser<PbhStruct>();
        var expected = "(P: 0.0, B: 180.0, H: 360.0, Pad: 0)";

        AssertRead(reader, parser, expected);
        AssertSkip(reader, parser);

        Assert.True(parser.InnerTryParse(ref reader, out PbhStruct output));
        Assert.Equal(0.0, output.P, 2);
        Assert.Equal(180, output.B, 2);
        Assert.Equal(360.0, output.H, 2);
        Assert.Equal(expected, output.ToString());
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
        AssertAllRead(reader, parser, name);
        AssertRead(reader, parser, (value,name));
    }

    [Theory]
    [InlineData("00000000", "")]
    [InlineData("02000000 4100", "A")]
    [InlineData("06000000 410042004300", "ABC")]
    public void ParseString(string source, string value)
    {
        var reader = source.BitsFromHex().AsSequenceReader();
        var parser = new StringParser();
        AssertAllRead(reader, parser, value);
    }

    private static void AssertAllRead<T>(SequenceReader<byte> reader, ValueParser parser, T value)
    {
        AssertRead(reader, parser, value);
        AssertAccessorRead(reader, parser, value);
        AssertRead(reader, parser, value?.ToString());
        AssertSkip(reader, parser);
        var missingOneByte = new SequenceReader<byte>(reader.Sequence.Slice(0, reader.Remaining-1));
        var onlyOneByte = new SequenceReader<byte>(reader.Sequence.Slice(0, 1));
        AssertSkipFail(missingOneByte, parser);
        AssertSkipFail(onlyOneByte, parser);
        AssertReadFail(missingOneByte, parser);
        AssertReadFail(onlyOneByte, parser);
    }

    private static void AssertAccessorRead<T>(SequenceReader<byte> reader, ValueParser parser, T value)
    {
        var pa = new PropertyAccessor(ref reader, new PropertyDecl(Guid.NewGuid(), "Title", parser));
        Assert.True(pa.PropertyDisplayString(out var strValue));
        Assert.Equal(0, reader.UnreadSequence.Length);
        Assert.Equal("Title: "+value.ToString(), strValue);
    }
    private static void AssertRead<T>(SequenceReader<byte> reader, ValueParser parser, T value)
    {
        Assert.True(parser.TryParse(ref reader, out T parsedValue));
        Assert.Equal(value, parsedValue);
        Assert.Equal(0, reader.Remaining);
    }
    private static void AssertSkip(SequenceReader<byte> reader, ValueParser parser)
    {
        Assert.True(parser.Skip(ref reader));
        Assert.Equal(0, reader.Remaining);
    }
    private static void AssertSkipFail(SequenceReader<byte> reader, ValueParser parser)
    {
        var remaining = reader.Remaining;
        Assert.False(parser.Skip(ref reader));
        Assert.Equal(remaining, reader.Remaining);
    }
    private static void AssertReadFail(SequenceReader<byte> reader, ValueParser parser)
    {
        var remaining = reader.Remaining;
        Assert.False(parser.InnerTryParse(ref reader, out string _));
        Assert.Equal(remaining, reader.Remaining);
    }
}