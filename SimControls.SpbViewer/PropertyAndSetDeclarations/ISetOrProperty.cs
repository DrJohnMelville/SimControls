using System;

namespace SimControls.SbpViewer.PropertyAndSetDeclarations;

public interface ISetOrProperty
{
    Guid Guid { get; }
    string Name { get; }
}