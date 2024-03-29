﻿using System;

namespace SimControls.SpbViewer.PropertyAndSetDeclarations;

public partial class SetDecl: ISetOrProperty
{
    [FromConstructor] public Guid Guid { get; }
    [FromConstructor] public string Name { get; }
    [FromConstructor] public string Description { get; }

    public override string ToString() =>
        $"{Guid}, {Name}, {Description}";
}