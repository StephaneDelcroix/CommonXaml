// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CommonXaml;

[DebuggerDisplay("{XamlType.NamespaceUri}:{XamlType.Name}")]
public class ReadOnlyXamlElement : IXamlElement
{
	readonly XamlElement element;

	internal ReadOnlyXamlElement(XamlElement element)
			=> this.element = element;

	public XamlType XamlType => element.XamlType;
	public IReadOnlyDictionary<IXamlPropertyIdentifier, IList<IXamlNode>> Properties => element.Properties;
	public IXamlElement? Parent => element.Parent;
	public IXamlNamespaceResolver NamespaceResolver => element.NamespaceResolver;
	public int LineNumber => element.LineNumber;
	public int LinePosition => element.LinePosition;
	public Uri? SourceUri => element.SourceUri;
}