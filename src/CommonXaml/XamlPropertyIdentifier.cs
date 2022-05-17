// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;

namespace CommonXaml
{
	[DebuggerDisplay("{NamespaceURI}:{LocalName}")]	
	public readonly struct XamlPropertyIdentifier : IXamlPropertyIdentifier, IXamlSourceInfo
	{
		public const string Xaml2006Uri = "http://schemas.microsoft.com/winfx/2006/xaml";
		public const string Xaml2009Uri = "http://schemas.microsoft.com/winfx/2009/xaml";
		public static IXamlPropertyIdentifier ImplicitProperty = new XamlPropertyIdentifier("http://commonxaml/internals", "ImplicitContent");

		public string NamespaceUri { get; }
		public string LocalName { get; }

		public int LineNumber { get; }
		public int LinePosition { get; }
		public Uri? SourceUri { get; }
#if !NETSTANDARD2_1_OR_GREATER
        public bool HasSourceInfo() => LineNumber >= 0 && LinePosition >= 0 && SourceUri != null;
#endif
        public XamlPropertyIdentifier(string namespaceUri, string localName, Uri? sourceUri = null, int lineNumber = -1, int linePosition = -1)
		{
			NamespaceUri = namespaceUri;
			LocalName = localName;
			SourceUri = sourceUri;
			LineNumber = lineNumber;
			LinePosition = linePosition;
		}

		public static XamlPropertyIdentifier CreateImplicitIdentifier(Uri? sourceUri = null, int lineNumber = -1, int linePosition = -1)
			 => new XamlPropertyIdentifier("http://commonxaml/internals", "ImplicitContent", sourceUri, lineNumber, linePosition);

		public bool IsImplicitIdentifier => ("http://commonxaml/internals", "ImplicitContent") == (NamespaceUri, LocalName);

		public override bool Equals(object obj) => obj switch {
			IXamlPropertyIdentifier other => NamespaceUri == other.NamespaceUri && LocalName == other.LocalName,
			_ => false,
		};

		public override int GetHashCode() => (NamespaceUri, LocalName).GetHashCode();

		public static bool operator ==(XamlPropertyIdentifier x1, IXamlPropertyIdentifier x2)
			=> x1.NamespaceUri == x2.NamespaceUri && x1.LocalName == x2.LocalName;
		public static bool operator ==(IXamlPropertyIdentifier x1, XamlPropertyIdentifier x2)
			=> x1.NamespaceUri == x2.NamespaceUri && x1.LocalName == x2.LocalName;
		public static bool operator !=(XamlPropertyIdentifier x1, IXamlPropertyIdentifier x2)
			=> !(x1 == x2);
		public static bool operator !=(IXamlPropertyIdentifier x1, XamlPropertyIdentifier x2)
			=> !(x1 == x2);
	}
}