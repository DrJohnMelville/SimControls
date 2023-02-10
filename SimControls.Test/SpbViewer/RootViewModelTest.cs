using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Melville.FileSystem;
using Melville.MVVM.Wpf.MvvmDialogs;
using Melville.MVVM.Wpf.RootWindows;
using Moq;
using SimControls.SpbViewer.Views;
using Xunit;

namespace SimControls.Test.SpbViewer;

public class RootViewModelTest
{
    private readonly Mock<IOpenSaveFile> openSaveFile = new();
    private readonly Mock<INavigationWindow> navWindow= new();
    private readonly Mock<IFile> file= new();
    private readonly Mock<ISpbTreeFactory> treeFactory= new();
    private readonly RootViewModel sut = new();

    [Fact]
    public async Task NullFileDoesNothingAsync()
    {
        await sut.LoadFile(openSaveFile.Object, navWindow.Object, treeFactory.Object, i=>null!);
        treeFactory.VerifyNoOtherCalls();
        navWindow.VerifyNoOtherCalls();
    }
    [Fact]
    public async Task LoadedFileCallsParserAndNavigatesAsync()
    {
        openSaveFile.Setup(i => i.GetLoadFile(null, "Spb", "SPB File|*.spb", "Select File"))
            .Returns(file.Object);
        var stream = new MemoryStream();
        file.Setup(i => i.OpenRead()).ReturnsAsync(stream);
        var node = Mock.Of<IList<SpbNode>>();
        treeFactory.Setup(i => i.Load(stream)).ReturnsAsync(new object[]{node});
        await sut.LoadFile(openSaveFile.Object, navWindow.Object, treeFactory.Object,
            i=>new SpbViewModel(i));

        treeFactory.Verify(i => i.Load(stream), Times.Once);
        treeFactory.VerifyNoOtherCalls();

        navWindow.Verify(i => i.NavigateTo(It.IsAny<SpbViewModel>()));
        navWindow.VerifyNoOtherCalls();
    }

}