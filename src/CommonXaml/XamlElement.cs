// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CommonXaml
{
	[DebuggerDisplay("{XamlType.NamespaceUri}:{XamlType.Name}")]
	public class XamlElement : IXamlNode
	{
		public XamlElement(XamlType xamlType, IXamlNamespaceResolver namespaceResolver = null, Uri sourceUri = null, int lineNumber = -1, int linePosition = -1)
		{
			XamlType = xamlType;
			NamespaceResolver = namespaceResolver;
			SourceUri = sourceUri;
			LineNumber = lineNumber;
			LinePosition = linePosition;
		}

		public XamlType XamlType { get; internal set; }

		internal readonly Dictionary<IXamlPropertyName, IList<IXamlNode>> properties = new Dictionary<IXamlPropertyName, IList<IXamlNode>>();
		public IReadOnlyDictionary<IXamlPropertyName, IList<IXamlNode>> Properties => properties;

		public XamlElement Parent { get; internal set; }

		public IXamlNamespaceResolver NamespaceResolver { get; }

		public int LineNumber { get; }
		public int LinePosition { get; }
		public Uri SourceUri { get; }

		public bool HasSourceInfo() => LineNumber >= 0 && LinePosition >= 0 && SourceUri != null;

		internal bool TryAdd(IXamlPropertyName propertyName, IList<IXamlNode> propertyValues)
		{
			foreach (var node in propertyValues)
				node.SetParent(this);

			if (properties.ContainsKey(propertyName))
				return false;
			properties.Add(propertyName, propertyValues);
			return true;
		}

		internal void AddOrAppend(IXamlPropertyName propertyName, IXamlNode propertyValue)
		{
			if (Properties.TryGetValue(propertyName, out var values) && values.Count == 1 && values[0] is XamlLiteral literal && propertyValue is XamlLiteral literalValue)
				literal.Literal += literalValue;
			else if (Properties.TryGetValue(XamlPropertyName.ImplicitProperty, out values))
				values.Add(propertyValue);
			else
				TryAdd(propertyName, new List<IXamlNode> { propertyValue });
		}

		internal void ReplaceNode(XamlLiteral original, IXamlNode replacement)
		{
			foreach (var prop in properties) {
				if (prop.Value.Contains(original)) {
					prop.Value.Remove(original);
					replacement.SetParent(this);
					prop.Value.Add(replacement);
				}
			}
		}

	}
}