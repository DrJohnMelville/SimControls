using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using SimControls.SpbParser;
using SimControls.SpbViewer.PropertyAndSetDeclarations;

namespace SimControls.SpbViewer.Views;

public interface ISpbTreeFactory
{
    Task<IList<object>> Load(Stream source);
}

public partial class SpbTreeFactory: ISpbTreeFactory
{
    [FromConstructor] private readonly Func<Task<IPropertyRegistry>> registry;
    public async Task<IList<object>> Load(Stream source)
    {
        var root = new SpbNode(null!);
        var ret = new SpbNodeParser(await registry(), root);
        await new SbpReader(PipeReader.Create(source), ret).ReadAsync();
        return root.Children;
    }
}