using System;
using SimControls.SpbParser;
using SimControls.SpbViewer.PropertyAndSetDeclarations;

namespace SimControls.SpbViewer.Views;

public partial class SpbNodeParser : IParseTarget
{
    [FromConstructor]private IPropertyRegistry registry;
    [FromConstructor]public SpbNode Target { get; }
    public ValueTask ParseItem(ISingleField field)
    {
        return registry.Get(field.Guid) switch
        {
            SetDecl sd => CreateSet(sd, field),
            PropertyDecl pd=> CreateProp(pd, field),
            _ => throw new InvalidOperationException("Unknown Node type")
        };
    }

    private async ValueTask CreateProp(PropertyDecl pd, ISingleField field)
    {
        if (pd.Guid == Guid.Empty) return;
        var leaf = new SpbLeaf(pd, await pd.Parser.Parse(field));
        Target.Children.Add(leaf);
    }

    private ValueTask CreateSet(SetDecl sd, ISingleField field)
    {
        var node = new SpbNode(sd);
        Target.Children.Add(node);
        field.PushSetDeclaration(new SpbNodeParser(registry, node));
        return ValueTask.CompletedTask;
    }

    public void EndScope()
    {
    }
}