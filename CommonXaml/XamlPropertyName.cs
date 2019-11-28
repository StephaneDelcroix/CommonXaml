// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;

namespace CommonXaml
{
	[DebuggerDisplay("{NamespaceURI}:{LocalName}")]
	public readonly struct XamlPropertyName : IXamlPropertyName
	{
		public const string Xaml2006Uri = "http://schemas.microsoft.com/winfx/2006/xaml";
		public const string Xaml2009Uri = "http://schemas.microsoft.com/winfx/2009/xaml";
		public static IXamlPropertyName ImplicitProperty = new XamlPropertyName("http://commonxaml/internals", "ImplicitContent");

		public string NamespaceUri { get; }
		public string LocalName { get; }

		public XamlPropertyName(string namespaceUri, string localName)
		{
			NamespaceUri = namespaceUri;
			LocalName = localName;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (!(obj is IXamlPropertyName other))
				return false;
			return NamespaceUri == other.NamespaceUri && LocalName == other.LocalName;
		}

		public override int GetHashCode()
		{
			unchecked {
				int hashCode = 0;
				if (NamespaceUri != null)
					hashCode = NamespaceUri.GetHashCode();
				if (LocalName != null)
					hashCode = (hashCode * 397) ^ LocalName.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(XamlPropertyName x1, IXamlPropertyName x2)
			=> x1.NamespaceUri == x2.NamespaceUri && x1.LocalName == x2.LocalName;
		public static bool operator ==(IXamlPropertyName x1, XamlPropertyName x2)
			=> x1.NamespaceUri == x2.NamespaceUri && x1.LocalName == x2.LocalName;
		public static bool operator !=(XamlPropertyName x1, IXamlPropertyName x2)
			=> !(x1 == x2);
		public static bool operator !=(IXamlPropertyName x1, XamlPropertyName x2)
			=> !(x1 == x2);
	}
}