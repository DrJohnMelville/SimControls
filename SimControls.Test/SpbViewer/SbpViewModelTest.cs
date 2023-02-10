using System;
using System.IO;
using System.Linq;
using Moq;
using SimControls.SpbViewer.PropertyAndSetDeclarations;
using SimControls.SpbViewer.Views;
using Xunit;

namespace SimControls.Test.SpbViewer;

public class SbpViewModelTest
{
    private readonly Mock<IClipboardWriter> writer = new();

    [Fact]
    public void WriteGuidTest()
    {
        var node = new SpbNode(new SetDecl(new Guid("\x1\x2\x3\x4\x5\x6\x7\x8\x9\xa\xb\xc\xd\xe\xf\x10"u8),
            "Name", "Description"));
        var model = new SpbViewModel(new object[] { node });
        model.CopyGuid(node, writer.Object);
        writer.Verify(i=>i.WriteTextString("04030201-0605-0807-090a-0b0c0d0e0f10"));
    }
}