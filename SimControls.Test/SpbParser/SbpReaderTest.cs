using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace SimControls.Test.SpbParser;

public class SbpReaderTest
{
    private Stream SourceStream() =>
        GetType().Assembly.GetManifestResourceStream(GetType(), "Alaska.spb")??
        throw new InvalidOperationException("Cannot Find Source");

    [Fact]
    public async Task ReadFileAsync()
    {
        // var reader = new SbpReader(PipeReader.Create(SourceStream()), target,
        //     await DefaultPropertyLibrary.GlobalRegistryAsync());
        Assert.Fail("Not Done Yet");
    }
}