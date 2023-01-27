using System;
using Melville.INPC;

namespace SimControls.SpbParser.PropertyAndSetDeclarations;

public partial class SetDecl: ISetOrProperty
{
    [FromConstructor] public Guid Guid { get; }
    [FromConstructor] public string Name { get; }
    [FromConstructor] public string Description { get; }

}