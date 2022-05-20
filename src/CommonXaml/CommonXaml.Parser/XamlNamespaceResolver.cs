// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Xml;

namespace CommonXaml.Parser;

class XamlNamespaceResolver : IXamlNamespaceResolver
{
	readonly IDictionary<string, string> NamespacesInScope;

	public XamlNamespaceResolver(IXmlNamespaceResolver resolver)
		=> NamespacesInScope = resolver.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml);

	public string LookupNamespace(string prefix)
		=> NamespacesInScope.TryGetValue(prefix, out var xmlns) ? xmlns : string.Empty;

	public string LookupPrefix(string namespaceName)
	{
		foreach (var kvp in NamespacesInScope) {
			if (kvp.Value == namespaceName)
				return kvp.Key;
		}
		return string.Empty;
	}
}