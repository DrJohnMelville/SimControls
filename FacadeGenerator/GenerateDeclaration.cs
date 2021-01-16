using System;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace FacadeGenerator
{
    public struct VariableDeclaration
    {
        public string VarName { get; }
        public string UnitName { get; }
        public bool CanWrite { get; }
        private static Regex lowerCase = new Regex(@"\b(\w)(\w*)\s*");
        public string CSharpName => 
            lowerCase.Replace(VarName, i => i.Groups[1].Value + i.Groups[2].Value.ToLower());

        public VariableDeclaration(string varName, string unitName, bool canWrite)
        {
            VarName = TrimIndex(varName);
            UnitName = unitName;
            CanWrite = canWrite;
        }

        private static string TrimIndex(string varName)
        {
            return varName.EndsWith(":index")?varName.Substring(0, varName.Length-6):varName;
        }

        public string OutputType => (CanWrite, UnitName) switch
        {
            (true, "Bool") =>"ReadOnlyBoolItem",
            (false, "Bool") =>"BoolItem",
            (true, _) =>$"DataItem<{CSharpType}>",
            (false, _) => $"ReadOnlyDataItem<{CSharpType}>"
        }; 

        public string CSharpType =>
            UnitName switch
            {
                "Feet per second" => "double",
                "Percent Over 100" => "double",
                "Percent over 100" => "double",
                "Number" => "double",
                "Gallons" => "double",
                "Radians" => "double",
                "Degrees" => "double",
                "Feet" => "double",
                "Feet/minute" => "double",
                "Knots" => "double",
                "Seconds" => "double",
                "Position" => "double",
                "Bool" => "int",
                _ => string.Empty
            };

        public string SimulatorType =>
            UnitName switch
            {
                "Bool" => "INT32",
                _ => "FLOAT64"
            };

        public string Documentation()
        {
            return "/// <summary>\r\n" +
                   $"/// {VarName} ({UnitName}) {(CanWrite?"Writeable":"Read Only")}\r\n" +
                   "/// </summary>\r\n";
        }
    }

    public static class GenerateDeclaration
    {
        public static string DeclareVariable(string name, string unit, string writable)
        {
            return DeclareVariable(new VariableDeclaration(name, unit, writable == "Y"));
        }

        private static string DeclareVariable(VariableDeclaration variable) =>
            variable.CSharpType switch
            {
                
                "" => $"        //{variable.VarName}/{variable.UnitName}/{variable.CanWrite}/{variable.CSharpName}",
                _=> RenderDeclaration(variable)
            };

        private static string RenderDeclaration(in VariableDeclaration variable) => 
            variable.Documentation()+
            $"        public static {variable.OutputType} {variable.CSharpName}(this IVariableCache innerStore) " +
            $"=> innerStore.GetVariable<{variable.CSharpType}, {variable.OutputType}>(\"{variable.VarName}\", " +
            $"\"{variable.UnitName}\", \"{variable.SimulatorType}\");";

        public static string DeclareEvent(string component) => 
            $"        public static SimEventTrigger {NameEvent(component)}(this IVariableCache cache) => cache.GetEvent(\"{component}\");";

        private static string NameEvent(string dataName) => csEventNamer.Replace(dataName,
            i => i.Groups[1].Value + i.Groups[2].Value.ToLower())+"Event";
        private static Regex csEventNamer = new Regex(@"([a-zA-Z0-9])([a-zA-Z0-9]*)[\s_]*");

    }
}