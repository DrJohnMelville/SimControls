using System;

namespace SimControls.SpbParser.PropertyAndSetDeclarations;

public interface ISetOrProperty
{
    Guid Guid { get; }
    string Name { get; }
}