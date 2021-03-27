using System.Text.RegularExpressions;

namespace FacadeGenerator
{
    public class VariableDeclaration
    {
        public string VarName { get; }
        public string UnitName { get; }
        public bool CanWrite { get; }
        public ushort Index { get; }
        private static Regex lowerCase = new Regex(@"\b(\w)(\w*)\s*");
        public string CSharpName => 
            lowerCase.Replace(VarName, i => i.Groups[1].Value + i.Groups[2].Value.ToLower());

        public VariableDeclaration(string varName, string unitName, bool canWrite, ushort index)
        {
            VarName = TrimIndex(varName);
            UnitName = unitName;
            CanWrite = canWrite;
            Index = index;
        }

        private static string TrimIndex(string varName)
        {
            return varName.EndsWith(":index")?varName.Substring(0, varName.Length-6):varName;
        }

        public string OutputType => (CanWrite, UnitName) switch
        {
            (false, "Bool") =>"ReadOnlyBoolItem",
            (true, "Bool") =>"BoolItem",
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
            return  "        /// <summary>\r\n" +
                   $"        /// {VarName} ({UnitName}) {(CanWrite?"Writeable":"Read Only")}\r\n" +
                    "        /// </summary>\r\n";
        }

        public string DeclareCreatorFunction() =>
            !string.IsNullOrWhiteSpace(UnitName)
                ? VarDeclaration()
                : $"// Not Enough Elements: {VarName} index: {Index}";

        private string VarDeclaration()
        {
            return CSharpType switch
            {
                "" => $"        //{VarName}/{UnitName}/{CanWrite}/{CSharpName}",
                _=> DeclarationSyntax()
            };
        }

        private string DeclarationSyntax() =>
            Documentation() + 
            $"        public static {OutputType} {CSharpName}(this IVariableCache innerStore) " 
            + Implementation() + ";";
 
        private string Implementation() =>
            $"=> innerStore.GetVariable<{CSharpType}, {OutputType}>(\"{VarName}\", " +
            $"\"{UnitName}\", \"{SimulatorType}\", {Index})";

        public string SwitchBranch() => $"            {Index} {Implementation()},";
    }
}