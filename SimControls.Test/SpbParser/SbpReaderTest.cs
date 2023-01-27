using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace SimControls.Test.SpbParser;

public class SbpReaderTest
{
    private Stream SourceStream() =>
        GetType().Assembly.GetManifestResourceStream(GetType(), "Alaska.sbp")??
        throw new InvalidOperationException("Cannot Find Source");

    [Fact]
    public async Task ReadFileAsync()
    {
        await Task.Yield();
    }
}