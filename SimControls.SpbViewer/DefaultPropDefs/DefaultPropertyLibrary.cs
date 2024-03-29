﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimControls.SpbViewer.PropertyAndSetDeclarations;


namespace SimControls.SpbViewer.DefaultPropDefs;

public static class DefaultPropertyLibrary
{
    public static Stream? GetDefaultStream(string fileName) =>
        typeof(DefaultPropertyLibrary).Assembly.GetManifestResourceStream(
            typeof(DefaultPropertyLibrary), fileName);

    public static IEnumerable<Stream> GetAllStreams()
    {
        var type = typeof(DefaultPropertyLibrary);
        var prefix = type.Namespace??"";
        var assembly = type.Assembly;
        return assembly.GetManifestResourceNames()
            .Where(i=> MyCompare(prefix, i))
            .Select(assembly.GetManifestResourceStream)
            .OfType<Stream>();
    }

    private static bool MyCompare(string prefix, string s)
    {
        var startsWith = s.StartsWith(prefix);
        return startsWith;
    }


    public static Task<IPropertyRegistry> GlobalRegistryAsync() =>
        PropertyRegistryParser.ParseAsync(GetAllStreams());
}