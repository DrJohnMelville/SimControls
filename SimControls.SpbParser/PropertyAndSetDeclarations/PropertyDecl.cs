using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melville.INPC;
using SimControls.SpbParser.ValueReaders;

namespace SimControls.SpbParser.PropertyAndSetDeclarations
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
