using System;
using System.Buffers;
using System.Threading.Tasks;
using Moq;
using SimControls.SpbParser;
using SimControls.SpbParser.FsTypes;
using SimControls.SpbViewer.ValueReaders;
using Xunit;

namespace SimControls.Test.SpbParser.ValueReaders;

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
    public async Task RoundTripGuidAsync()
    {
        var result = Guid.NewGuid();
        var buffer = new byte[16];
        result.TryWriteBytes(buffer);
        var reader = SingleField(buffer);
        var parser = new BltParser<Guid>();
        await AssertAllReadAsync(reader, parser, result);
    }
    [Theory]
    [InlineData("00000000", 0)]
    [InlineData("01000000", 1)]
    [InlineData("01010000", 257)]
    [InlineData("FFFFFF7F", int.MaxValue)]
    [InlineData("00000080", int.MinValue)]
    [InlineData("FFFFFFFF", -1)]
    public async Task QuadToIntAsync(string input, int value)
    {
        var reader = SingleField(input.BitsFromHex());
        var parser = new BltParser<int>();
        await AssertAllReadAsync(reader, parser, value);
    }
    [Theory]
    [InlineData("00000000", false)]
    [InlineData("01000000", true)]
    public async Task QuadToBoolAsync(string input, bool value)
    {
        var reader = SingleField(input.BitsFromHex());
        var parser = new BltParser<BoolStruct>();
        await AssertAllReadAsync(reader, parser, new BoolStruct(value));
    }
    [Theory]
    [InlineData("00000000", 0)]
    [InlineData("01000000", 1)]
    [InlineData("01010000", 257)]
    [InlineData("FFFFFFFF", uint.MaxValue)]
    [InlineData("00000000", uint.MinValue)]
    public async Task QuadToUIntAsync(string input, uint value)
    {
        var reader = SingleField(input.BitsFromHex());
        var parser = new BltParser<uint>();
        await AssertAllReadAsync(reader, parser, value);
    }

    [Fact]
    public async Task OctetToint2Async()
    {
        var reader = SingleField("01000000 02000000".BitsFromHex());
        var parser = new BltParser<(int,int)>();
        await AssertAllReadAsync(reader, parser, (1,2));
    }

    [Fact]
    public async Task ParsePbhAsync()
    {
        var reader = SingleField("00000000 00000080 FFFFFFFF 00000000".BitsFromHex());
        var parser = new BltParser<PbhStruct>();
        var expected = "(P: 0.0, B: 180.0, H: 360.0, Pad: 0)";

        await AssertAllReadAsync(reader, parser, expected);
     }

    [Theory]
    [InlineData("00000000", "Zero (0)")]
    [InlineData("01000000", "One (1)")]
    [InlineData("02000000", "Two (2)")]
    [InlineData("03000000", "<Undefined> (3)")]
    [InlineData("030FF000", "<Undefined> (15732483)")]
    public async Task ParseEnumAsync(string source, string name)
    {
        var reader = SingleField(source.BitsFromHex());
        var parser = new EnumParser("Zero", "One", "Two");
        await AssertAllReadAsync(reader, parser, name);
    }

    [Theory]
    [InlineData("4142", "k")]
    [InlineData("4100", "k")]
    [InlineData("414243", "kE")]
    public async Task ParseStringAsync(string source, string value)
    {
        var reader = SingleField(source.BitsFromHex());
        var parser = new StringParser();
        await AssertAllReadAsync(reader, parser, value);
    }

 
    private async Task AssertAllReadAsync<T>(ISingleField reader, ValueParser parser, T value)
    {
        Assert.Equal(value!.ToString(), await parser.Parse(reader));
    }
 
}