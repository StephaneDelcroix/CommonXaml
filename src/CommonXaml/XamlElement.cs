// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CommonXaml;

[DebuggerDisplay("{XamlType.NamespaceUri}:{XamlType.Name}")]
public class XamlElement : IXamlElement
{
	public XamlElement(XamlType xamlType, IXamlNamespaceResolver namespaceResolver, Uri? sourceUri = null, int lineNumber = -1, int linePosition = -1)
	{
		XamlType = xamlType;
		NamespaceResolver = namespaceResolver;
		SourceUri = sourceUri;
		LineNumber = lineNumber;
		LinePosition = linePosition;
	}

	public XamlType XamlType { get; set; }

	internal readonly Dictionary<IXamlPropertyIdentifier, IList<IXamlNode>> properties = new();
	public IReadOnlyDictionary<IXamlPropertyIdentifier, IList<IXamlNode>> Properties => properties;

	public IXamlElement? Parent { get; set; }

	public IXamlNamespaceResolver NamespaceResolver { get; }

	public int LineNumber { get; }
	public int LinePosition { get; }
	public Uri? SourceUri { get; }

	public bool TryAdd(IXamlPropertyIdentifier propertyName, IList<IXamlNode> propertyValues)
	{
		foreach (var node in propertyValues)
			node.SetParent(this);

		if (properties.ContainsKey(propertyName))
			return false;
		properties.Add(propertyName, propertyValues);
		return true;
	}

	public void AddOrAppend(IXamlPropertyIdentifier propertyName, IXamlNode propertyValue)
	{
		if (Properties.TryGetValue(propertyName, out var values) && values.Count == 1 && values[0] is XamlLiteral literal && propertyValue is XamlLiteral literalValue)
			literal.Literal += literalValue;
		else if (this.TryGetImplicitProperty(out values))
			values.Add(propertyValue);
		else
			TryAdd(propertyName, new List<IXamlNode> { propertyValue });
	}

	public void ReplaceNode(IXamlPropertyIdentifier? propertyName, IXamlNode original, IXamlNode replacement)
	{
		if (propertyName is null)
			return;
		if (!properties.TryGetValue(propertyName, out var props))
			return;
		var idx = props.IndexOf(original);
		if (idx < 0)
			return;
		replacement.SetParent(this);
		props[idx] = replacement;
	}

	public ReadOnlyXamlElement AsReadOnly() => new(this);
}