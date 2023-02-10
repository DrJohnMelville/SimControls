using System;
using Melville.Hacks;
using Microsoft.CodeAnalysis.Diagnostics;
using SimControls.SpbViewer.PropertyAndSetDeclarations;
using SimControls.SpbParser;

namespace SimControls.SpbViewer.ParseTargets;

public partial class StringParseTarget: IParseTarget
{
    [FromConstructor] private readonly IPropertyRegistry properties;
    private readonly IndentingStringBuilder target = new();
    public ValueTask ParseItem(ISingleField field) =>
        properties.Get(field.Guid) switch
        {
            SetDecl sd => HandleSetDecl(field, sd),
            PropertyDecl pd => HandlePropertyDecl(field, pd),
            _ => throw new ArgumentOutOfRangeException("Unknown item type")
        };

    private async ValueTask HandlePropertyDecl(ISingleField field, PropertyDecl pd)
    {
        target.AppendLine($"{pd.Name}: {await pd.Parser.Parse(field)}");
    }

    private ValueTask HandleSetDecl(ISingleField field, SetDecl sd)
    {
        target.AppendLine($"{sd.Name} ({sd.Description})");
        field.PushSetDeclaration(this);
        target.BeginIndent();
        return ValueTask.CompletedTask;
    }

    public void EndScope() => target.EndIndent();

    public override string ToString() => target.ToString();
}