// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace CommonXaml;

public interface IXamlNode : IXamlSourceInfo
	{
		IXamlElement? Parent { get; }
		IXamlNamespaceResolver NamespaceResolver { get; }
	}

class EmptyNode : IXamlNode
{
    public IXamlElement? Parent => null;
    public IXamlNamespaceResolver NamespaceResolver => throw new NotImplementedException();
    public int LineNumber => -1;
    public int LinePosition => -1;
    public Uri? SourceUri => null;
}

public static class IXamlNodeExtensions
{
    internal static void SetParent(this IXamlNode node, XamlElement parent)
    {
        if (node is XamlElement element)
            element.Parent = parent;
        else if (node is XamlLiteral literal)
            literal.Parent = parent;
        else throw new InvalidOperationException($"Can't set Parent on {node}");
    }
}