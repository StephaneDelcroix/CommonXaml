// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;

namespace CommonXaml;

[DebuggerDisplay("{Literal}")]
public class XamlLiteral : IXamlLiteral
{
	public XamlLiteral(string literal, IXamlNamespaceResolver namespaceResolver, Uri? sourceUri, int lineNumber = -1, int linePosition = -1)
	{
		Literal = literal;
		NamespaceResolver = namespaceResolver;
		SourceUri = sourceUri;
		LineNumber = lineNumber;
		LinePosition = linePosition;
	}

	public string Literal { get; set; }

	public IXamlElement? Parent { get; set; }
	public IXamlNamespaceResolver NamespaceResolver { get; }

	public int LineNumber { get; }
	public int LinePosition { get; }
	public Uri? SourceUri { get; }

	/// <summary>
	/// Returns a read-only ReadOnlyXamlLiteral wrapper for the current XamlLiteral
	/// </summary>
	/// <returns></returns>
	public ReadOnlyXamlLiteral AsReadOnly() => new(this);
}