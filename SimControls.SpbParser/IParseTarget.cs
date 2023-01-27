using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimControls.SpbParser.PropertyAndSetDeclarations;

namespace SimControls.SpbParser
{
    internal interface IParseTarget
    {
        IParseTarget BeginSet (SetDecl set);
        void EndSet (SetDecl set);
        void PushProperty(PropertyDecl Property);
    }
}
