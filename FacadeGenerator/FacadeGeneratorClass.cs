using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

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
            return Prefix()
                   + SimVariableDeclarations(variableLines)
                   + SimEventDeclarations(eventLines)
                   + Suffix();
        }


        private string SimVariableDeclarations(string[] variables)
        {
            return string.Join("\r\n", variables.Skip(1).Select(SingleVariableFromString))
                +Environment.NewLine;
        }

        private string SingleVariableFromString(string varDecl)
        {
            var components = varDecl.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            return 
                components.Length > 2?
                    GenerateDeclaration.DeclareVariable(components[0], components[1], components[2]):
                    "// Not enough Elements: "+ varDecl;
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