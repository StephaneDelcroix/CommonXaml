// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;

namespace CommonXaml
{
	[DebuggerDisplay("{NamespaceURI}:{LocalName}")]	
	public readonly struct XamlPropertyIdentifier : IXamlPropertyName, IXamlSourceInfo
	{
		public string NamespaceUri { get; }
		public string LocalName { get; }

		public int LineNumber { get; }
		public int LinePosition { get; }
		public Uri? SourceUri { get; }

		public XamlPropertyIdentifier(string namespaceUri, string localName, Uri? sourceUri = null, int lineNumber = -1, int linePosition = -1)
		{
			NamespaceUri = namespaceUri;
			LocalName = localName;
			SourceUri = sourceUri;
			LineNumber = lineNumber;
			LinePosition = linePosition;
		}

		public bool HasSourceInfo() => LineNumber >= 0 && LinePosition >= 0 && SourceUri != null;

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (!(obj is IXamlPropertyName other))
				return false;
			return NamespaceUri == other.NamespaceUri && LocalName == other.LocalName;
		}

		public override int GetHashCode() => (NamespaceUri, LocalName).GetHashCode();

		public static bool operator ==(XamlPropertyIdentifier x1, IXamlPropertyName x2)
			=> x1.NamespaceUri == x2.NamespaceUri && x1.LocalName == x2.LocalName;
		public static bool operator ==(IXamlPropertyName x1, XamlPropertyIdentifier x2)
			=> x1.NamespaceUri == x2.NamespaceUri && x1.LocalName == x2.LocalName;
		public static bool operator !=(XamlPropertyIdentifier x1, IXamlPropertyName x2)
			=> !(x1 == x2);
		public static bool operator !=(IXamlPropertyName x1, XamlPropertyIdentifier x2)
			=> !(x1 == x2);
	}
}