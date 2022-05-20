// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace CommonXaml;

public interface IXamlElement : IXamlNode
{
    XamlType XamlType { get; }
    IReadOnlyDictionary<IXamlPropertyIdentifier, IList<IXamlNode>> Properties { get; }
}

public static class IXamlElementExtensions
{
    public static bool TryGetProperty(this IXamlElement self, (string namespaceUri, string localName) prop, out IList<IXamlNode> value)
        => self.Properties.TryGetValue(new XamlPropertyIdentifier(prop.namespaceUri, prop.localName), out value);

    public static bool TryGetImplicitProperty(this IXamlElement self, out IList<IXamlNode> value)
        => self.Properties.TryGetValue(XamlPropertyIdentifier.ImplicitProperty, out value);

    public static IXamlPropertyIdentifier? GetIdentifier(this IXamlElement self, IXamlNode node)
    {
        foreach (var prop in self.Properties)
            if (prop.Value.Contains(node))
                return prop.Key;
        return null;
    }
}