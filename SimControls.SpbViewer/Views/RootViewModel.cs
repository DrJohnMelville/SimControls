using System;
using System.Collections.Generic;
using System.IO;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.MvvmDialogs;
using Melville.MVVM.Wpf.RootWindows;
using Melville.MVVM.Wpf.ViewFrames;

namespace SimControls.SpbViewer.Views;

[OnDisplayed(nameof(LoadFile))]
public class RootViewModel
{

    public async Task LoadFile([FromServices] IOpenSaveFile fileDlg,
        INavigationWindow navWin, 
        [FromServices] ISpbTreeFactory treeFactory,
        [FromServices] Func<IList<object>, SpbViewModel> windowFactory)
    {
        var file = fileDlg.GetLoadFile(null, "Spb", "SPB File|*.spb", "Select File");
        if (file == null) return;
        var node = await treeFactory.Load(await file.OpenRead());
        navWin.NavigateTo(windowFactory(node));

    }
}