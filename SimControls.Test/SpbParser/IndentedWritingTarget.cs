using Melville.Hacks;
using SimControls.SpbParser;
using SimControls.SpbParser.PropertyAndSetDeclarations;

namespace SimControls.Test.SpbParser;

public class IndentedWritingTarget : IParseTarget
{
    private IndentingStringBuilder target = new();
    public IParseTarget BeginSet(SetDecl set)
    {
        target.AppendLine($"{set.Name} ({set.Description})");
        target.BeginIndent();
        return this;
    }

    public void EndSet(SetDecl set) => target.EndIndent();

    public void PushProperty(ref PropertyAccessor accessor)
    {
        if (accessor.PropertyDisplayString(out var pds))
            target.AppendLine(pds);
    }

    public override string ToString() => target.ToString();
}