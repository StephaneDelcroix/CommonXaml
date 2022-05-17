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
		public XamlElement(XamlType xamlType, IXamlNamespaceResolver namespaceResolver, Uri? sourceUri = null, int lineNumber = -1, int linePosition = -1)
		{
			XamlType = xamlType;
			NamespaceResolver = namespaceResolver;
			SourceUri = sourceUri;
			LineNumber = lineNumber;
			LinePosition = linePosition;
		}

		public XamlType XamlType { get; internal set; }

		internal readonly Dictionary<IXamlPropertyIdentifier, IList<IXamlNode>> properties = new Dictionary<IXamlPropertyIdentifier, IList<IXamlNode>>();
		public IReadOnlyDictionary<IXamlPropertyIdentifier, IList<IXamlNode>> Properties => properties;

		public XamlElement? Parent { get; internal set; }

		public IXamlNamespaceResolver NamespaceResolver { get; }

		public int LineNumber { get; }
		public int LinePosition { get; }
		public Uri? SourceUri { get; }
#if !NETSTANDARD2_1_OR_GREATER
        public bool HasSourceInfo() => LineNumber >= 0 && LinePosition >= 0 && SourceUri != null;
#endif
        internal bool TryAdd(IXamlPropertyIdentifier propertyName, IList<IXamlNode> propertyValues)
		{
			foreach (var node in propertyValues)
				node.SetParent(this);

			if (properties.ContainsKey(propertyName))
				return false;
			properties.Add(propertyName, propertyValues);
			return true;
		}

		internal void AddOrAppend(IXamlPropertyIdentifier propertyName, IXamlNode propertyValue)
		{
			if (Properties.TryGetValue(propertyName, out var values) && values.Count == 1 && values[0] is XamlLiteral literal && propertyValue is XamlLiteral literalValue)
				literal.Literal += literalValue;
			else if (TryGetImplicitProperty(out values))
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

        class xprop : IXamlPropertyIdentifier
        {
            public xprop((string namespaceUri, string localName)prop)
            {
				NamespaceUri = prop.namespaceUri;
				LocalName = prop.localName;
            }
            public string NamespaceUri { get; }
            public string LocalName { get; }
			public override int GetHashCode() => (NamespaceUri, LocalName).GetHashCode();
		}

		public bool TryGetProperty((string namespaceUri, string localName) prop, out IList<IXamlNode> value) => Properties.TryGetValue(new xprop(prop), out value);
		public bool TryGetImplicitProperty(out IList<IXamlNode> value) => TryGetProperty(("http://commonxaml/internals", "ImplicitContent"),out value);
	}
}