using System;
using System.Buffers;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using Moq;
using SimControls.SbpViewer.ValueReaders;
using SimControls.SpbParser;
using SimControls.SpbParser.FsTypes;
using Xunit;

namespace SimControls.Test.SpbParser.ValueReaders;

public static class Foo
{
}
public class ValueReaders
{
    public static ISingleField SingleField(byte[] data)
    {
        var ret = new Mock<ISingleField>();
        ret.SetupGet(i => i.Size).Returns(data.Length);
        ret.Setup(i => i.GetByteSequence()).Returns(new ValueTask<ReadOnlySequence<byte>>(
            new ReadOnlySequence<byte>(data)));
        return ret.Object;
    }
    [Fact]
    public async Task RoundTripGuid()
    {
        var result = Guid.NewGuid();
        var buffer = new byte[16];
        result.TryWriteBytes(buffer);
        var reader = SingleField(buffer);
        var parser = new BltParser<Guid>();
        await AssertAllRead(reader, parser, result);
    }
    [Theory]
    [InlineData("00000000", 0)]
    [InlineData("01000000", 1)]
    [InlineData("01010000", 257)]
    [InlineData("FFFFFF7F", int.MaxValue)]
    [InlineData("00000080", int.MinValue)]
    [InlineData("FFFFFFFF", -1)]
    public async Task QuadToInt(string input, int value)
    {
        var reader = SingleField(input.BitsFromHex());
        var parser = new BltParser<int>();
        await AssertAllRead(reader, parser, value);
    }
    [Theory]
    [InlineData("00000000", false)]
    [InlineData("01000000", true)]
    public async Task QuadToBool(string input, bool value)
    {
        var reader = SingleField(input.BitsFromHex());
        var parser = new BltParser<BoolStruct>();
        await AssertAllRead(reader, parser, new BoolStruct(value));
    }
    [Theory]
    [InlineData("00000000", 0)]
    [InlineData("01000000", 1)]
    [InlineData("01010000", 257)]
    [InlineData("FFFFFFFF", uint.MaxValue)]
    [InlineData("00000000", uint.MinValue)]
    public async Task QuadToUInt(string input, uint value)
    {
        var reader = SingleField(input.BitsFromHex());
        var parser = new BltParser<uint>();
        await AssertAllRead(reader, parser, value);
    }

    [Fact]
    public async Task OctetToint2()
    {
        var reader = SingleField("01000000 02000000".BitsFromHex());
        var parser = new BltParser<(int,int)>();
        await AssertAllRead(reader, parser, (1,2));
    }

    [Fact]
    public async Task ParsePbh()
    {
        var reader = SingleField("00000000 00000080 FFFFFFFF 00000000".BitsFromHex());
        var parser = new BltParser<PbhStruct>();
        var expected = "(P: 0.0, B: 180.0, H: 360.0, Pad: 0)";

        await AssertAllRead(reader, parser, expected);
     }

    [Theory]
    [InlineData("00000000", 0, "Zero (0)")]
    [InlineData("01000000", 1, "One (1)")]
    [InlineData("02000000", 2, "Two (2)")]
    [InlineData("03000000", 3, "<Undefined> (3)")]
    [InlineData("030FF000", 15732483, "<Undefined> (15732483)")]
    public async Task ParseEnum(string source, int value, string name)
    {
        var reader = SingleField(source.BitsFromHex());
        var parser = new EnumParser("Zero", "One", "Two");
        await AssertAllRead(reader, parser, name);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("4100", "A")]
    [InlineData("410042004300", "ABC")]
    public async Task ParseString(string source, string value)
    {
        var reader = SingleField(source.BitsFromHex());
        var parser = new StringParser();
        await AssertAllRead(reader, parser, value);
    }

 
    private async Task AssertAllRead<T>(ISingleField reader, ValueParser parser, T value)
    {
        Assert.Equal(value.ToString(), await parser.Parse(reader));
    }
 
}