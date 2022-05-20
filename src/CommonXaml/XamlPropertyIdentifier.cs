// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;

namespace CommonXaml;

[DebuggerDisplay("{NamespaceURI}:{LocalName}")]
public readonly struct XamlPropertyIdentifier : IXamlPropertyIdentifier, IXamlSourceInfo
{
	public const string Xaml2006Uri = "http://schemas.microsoft.com/winfx/2006/xaml";
	public const string Xaml2009Uri = "http://schemas.microsoft.com/winfx/2009/xaml";
	public const string Xaml2022Uri = "http://schemas.microsoft.com/winfx/2022/xaml";

	public static IXamlPropertyIdentifier ImplicitProperty = new XamlPropertyIdentifier("http://commonxaml/internals", "ImplicitContent");

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

	public static XamlPropertyIdentifier CreateImplicitIdentifier(Uri? sourceUri = null, int lineNumber = -1, int linePosition = -1)
		 => new(ImplicitProperty.NamespaceUri, ImplicitProperty.LocalName, sourceUri, lineNumber, linePosition);

	public bool IsImplicitIdentifier => (ImplicitProperty.NamespaceUri, ImplicitProperty.LocalName) == (NamespaceUri, LocalName);

	public override bool Equals(object obj) => obj switch {
		IXamlPropertyIdentifier other => NamespaceUri == other.NamespaceUri && LocalName == other.LocalName,
		_ => false,
	};

	public override int GetHashCode() => (NamespaceUri, LocalName).GetHashCode();

	public static bool operator ==(XamlPropertyIdentifier x1, IXamlPropertyIdentifier x2)
		=> x1.Equals(x2);
	public static bool operator ==(IXamlPropertyIdentifier x1, XamlPropertyIdentifier x2)
		=> x2.Equals(x1);
	public static bool operator !=(XamlPropertyIdentifier x1, IXamlPropertyIdentifier x2)
		=> !(x1 == x2);
	public static bool operator !=(IXamlPropertyIdentifier x1, XamlPropertyIdentifier x2)
		=> !(x1 == x2);
}