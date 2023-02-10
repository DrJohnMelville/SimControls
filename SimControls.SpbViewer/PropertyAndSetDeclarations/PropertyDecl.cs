using System;
using SimControls.SpbViewer.ValueReaders;

namespace SimControls.SpbViewer.PropertyAndSetDeclarations
{
    public partial class PropertyDecl: ISetOrProperty
    {
        [FromConstructor] public Guid Guid { get; }
        [FromConstructor]public string Name { get;}
        [FromConstructor]public string Description { get;}
        [FromConstructor]public string Default { get;}
        [FromConstructor]internal ValueParser Parser { get; }

        public override string ToString() => 
            $"{Guid}, {Name}, {Description}, {Default}, {Parser.TypeString}";
    }
}
