using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melville.INPC;
using SimControls.SpbParser.ValueReaders;

namespace SimControls.SpbParser.PropertyAndSetDeclarations
{
    public interface ISetOrProperty
    {
        Guid Guid { get; }
        string Name { get; }
    }
    public abstract partial class PropertyDecl: ISetOrProperty
    {
        [FromConstructor] public Guid Guid { get; }
        [FromConstructor]public string Name { get;}
        [FromConstructor]private IValueParser Parser { get; }
    }

    public partial class SetDecl: ISetOrProperty
    {
        [FromConstructor] public Guid Guid { get; }
        [FromConstructor] public string Name { get; }
        [FromConstructor] public string Description { get; }

    }
}
