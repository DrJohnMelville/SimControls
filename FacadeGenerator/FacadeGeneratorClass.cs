using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace FacadeGenerator
{
    [Generator]
    public class FacadeGeneratorClass : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            context.AddSource("StrongTypedPlaneDataStore.Generated.cs",
                CreateText(GetVariableLines(context),
                    SplitIntoLines(VarData.Events)
                ));
        }

        private static string[] GetVariableLines(GeneratorExecutionContext context)
        {
#if true
            return SplitIntoLines(VarData.Variables);
#else
            return context.AdditionalFiles
                .Where(i => i.Path.EndsWith("Variables.csv", StringComparison.Ordinal))
                .SelectMany(i => i.GetText()?.Lines.Select(j => j.ToString()) ?? Array.Empty<string>())
                .ToArray();
#endif
        }

        private static string[] SplitIntoLines(string text)
        {
            return text.Split(new []{'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
        }

        private string CreateText(string[] variableLines, string[] eventLines)
        {
            var variableDeclarations = variableLines.Skip(1).Select(SingleVariableFromString).ToList();
            return Prefix()
                   + SimVariableDeclarations(variableDeclarations) 
                   + DeclareVariableByNumber(variableDeclarations)
                   + SimEventDeclarations(eventLines)
                   + Suffix();
        }

        private string DeclareVariableByNumber(List<VariableDeclaration> variableDeclarations)
        {
            var ret = new StringBuilder();
            ret.AppendLine("        public static DataItem VariableByNumber(this IVariableCache innerStore, ushort index) => index switch");
            ret.AppendLine("        {");
            foreach (var declaration in variableDeclarations.Where(i=>i.CSharpType.Length > 0))
            {
                
                ret.AppendLine(declaration.SwitchBranch());
            }
            
            ret.AppendLine("            _ => throw new System.IO.InvalidDataException(\"No such variable: \"+index)");
            ret.AppendLine("        };");
            return ret.ToString();
        }


        private string SimVariableDeclarations(IEnumerable<VariableDeclaration> variableDeclarations)
        {
            return string.Join("\r\n", variableDeclarations
                       .Select(i=>i.DeclareCreatorFunction()))
                +Environment.NewLine;
        }

        private VariableDeclaration SingleVariableFromString(string varDecl, int index)
        {
            var components = varDecl.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            return 
                components.Length > 2?
                    new VariableDeclaration(components[0], components[1], components[2] == "Y", (ushort)index):
                    new VariableDeclaration(varDecl, "", false, (ushort)index);
        }

        private string SimEventDeclarations(string[] eventLines) => 
            string.Join(Environment.NewLine, eventLines.Select(SingleEventFromString));

        private string SingleEventFromString(string eventDecl)
        {
            var components = eventDecl.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            return 
                components.Length > 1?
                    GenerateDeclaration.DeclareEvent(components[1]):
                    "// Not enough Elements: "+ eventDecl;
        }


        private static string Suffix()
        {
            return "\r\n    }\r\n}";
        }

        private static string Prefix() =>
            @"namespace SimControls.Model
{
    public static partial class StrongTypedPlaneDataStore
    {
";
    }
}