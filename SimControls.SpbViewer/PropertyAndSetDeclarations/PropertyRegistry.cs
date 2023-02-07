using System;
using System.Collections.Generic;
using System.Linq;

namespace SimControls.SbpViewer.PropertyAndSetDeclarations;

//public interface IPropertyRegistry

public interface IPropertyRegistry
{
    ISetOrProperty Get(Guid key);
    int ItemCount { get; }
}

public static class PropertyRegistryOperations
{
    public static ISetOrProperty Get(this IPropertyRegistry reg, string key) =>
        reg.Get(new Guid(key));
}

internal class PropertyRegistry : IPropertyRegistry
{
    private readonly Dictionary<Guid, ISetOrProperty> items;

    public PropertyRegistry(IEnumerable<ISetOrProperty> items)
    {
        this.items = items.ToDictionary(i => i.Guid);
    }

    public ISetOrProperty Get(Guid key) => items[key];
    public int ItemCount => items.Count;
}
