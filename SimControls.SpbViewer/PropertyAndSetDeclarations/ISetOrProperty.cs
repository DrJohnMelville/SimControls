using System;

namespace SimControls.SpbViewer.PropertyAndSetDeclarations;

public interface ISetOrProperty
{
    Guid Guid { get; }
    string Name { get; }
}