// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;

namespace CommonXaml;

[DebuggerDisplay("{Literal}")]
public class ReadOnlyXamlLiteral : IXamlLiteral
{
    internal ReadOnlyXamlLiteral(XamlLiteral literal) => _literal = literal;

    XamlLiteral _literal;

    public string Literal => _literal.Literal;

    public IXamlElement? Parent => _literal.Parent switch {
        XamlElement element => element.AsReadOnly(),
        _ => _literal.Parent,
    };
    public IXamlNamespaceResolver NamespaceResolver => _literal.NamespaceResolver;

    public int LineNumber => _literal.LineNumber;
    public int LinePosition => _literal.LinePosition;
    public Uri? SourceUri => _literal.SourceUri;
}