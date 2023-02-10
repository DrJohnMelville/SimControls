using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Moq;
using SimControls.SpbParser;
using SimControls.SpbViewer.PropertyAndSetDeclarations;
using SimControls.SpbViewer.Views;
using Xunit;

namespace SimControls.Test.SpbViewer;

public class SpbTreeFactoryTest
{
    private readonly Mock<IPropertyRegistry> registry = new();
    private readonly Mock<ISingleField> nodeField = new();
    
    public SpbTreeFactoryTest()
    {
        nodeField.SetupGet(i=>i.Guid).Returns(Guid.NewGuid());
        nodeField.SetupGet(i=>i.Size).Returns(20);
    }

    [Fact]
    public async Task ParseNodeAsync()
    {
        var node = new SpbNode(new SetDecl(nodeField.Object.Guid, "Name", ""));
        var parser = new SpbNodeParser(registry.Object, node);
        registry.Setup(i => i.Get(node.Declaration.Guid)).Returns(node.Declaration);
        await parser.ParseItem(nodeField.Object);
        parser.EndScope();
        Assert.Single(node.Children);
        Assert.Equal(node.Declaration, ((SpbNode)node.Children[0]).Declaration);

    }
}