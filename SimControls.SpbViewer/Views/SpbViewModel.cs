using System.Collections.Generic;
using Melville.MVVM.Wpf.Clipboards;
using Melville.MVVM.Wpf.DiParameterSources;
using SimControls.SpbViewer.PropertyAndSetDeclarations;

namespace SimControls.SpbViewer.Views;

public partial class SpbViewModel
{
    [FromConstructor]public IList<object> Root { get; }


    public void CopyGuid(ISpbNode item, [FromServices]IClipboardWriter writer)
    {
        writer.WriteTextString(item.Declaration.Guid.ToString());
    }
}

