using System;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace FacadeGenerator
{
    public static class GenerateDeclaration
    {
        public static string DeclareEvent(string component) => 
            $"        public static SimEventTrigger {NameEvent(component)}(this IVariableCache cache) => cache.GetEvent(\"{component}\");";

        private static string NameEvent(string dataName) => csEventNamer.Replace(dataName,
            i => i.Groups[1].Value + i.Groups[2].Value.ToLower())+"Event";
        private static Regex csEventNamer = new Regex(@"([a-zA-Z0-9])([a-zA-Z0-9]*)[\s_]*");

    }
}