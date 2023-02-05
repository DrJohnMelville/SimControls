using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melville.Hacks;
using Moq;
using SimControls.SpbParser;
using SimControls.SpbParser.PropertyAndSetDeclarations;
using Xunit;

namespace SimControls.Test.SpbParser;

public class IndentedWritingTarget : IParseTarget
{
    private IndentingStringBuilder target = new();
    public IParseTarget BeginSet(SetDecl set)
    {
        target.AppendLine($"{set.Name} ({set.Description})");
        target.BeginIndent();
        return this;
    }

    public void EndSet(SetDecl set) => target.EndIndent();

    public void PushProperty(ref PropertyAccessor accessor)
    {
        if (accessor.PropertyDisplayString(out var pds))
            target.AppendLine(pds);
    }

    public override string ToString() => target.ToString();
}

public class SbpReaderTest
{
    private Stream SourceStream() =>
        GetType().Assembly.GetManifestResourceStream(GetType(), "Alaska.spb")??
        throw new InvalidOperationException("Cannot Find Source");

    [Fact]
    public async Task ReadFileAsync()
    {
        var target = new IndentedWritingTarget();
        var reader = new SbpReader(PipeReader.Create(SourceStream()), target);
        await reader.Read();
        Assert.Equal("xx", target.ToString());
    }
}