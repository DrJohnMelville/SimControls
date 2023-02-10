using System.Collections.Generic;
using SimControls.SpbViewer.PropertyAndSetDeclarations;

namespace SimControls.SpbViewer.Views;

public interface ISpbNode
{
    public ISetOrProperty Declaration { get; }
}

public partial class SpbNode: ISpbNode
{
    [FromConstructor]public SetDecl Declaration { get; }
    public IList<object> Children { get; } = new List<object>();
    ISetOrProperty ISpbNode.Declaration => Declaration;
}


public partial class SpbLeaf : ISpbNode
{
    [FromConstructor] public PropertyDecl Declaration { get; }
    [FromConstructor] public string Value { get; }
    ISetOrProperty ISpbNode.Declaration => Declaration;
}