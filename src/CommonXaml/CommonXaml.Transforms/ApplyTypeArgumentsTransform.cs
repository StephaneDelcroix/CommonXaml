// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace CommonXaml.Transforms;

public class ApplyTypeArgumentsTransform : IXamlTransform<IXamlTransformConfiguration>
{
	public ApplyTypeArgumentsTransform(IXamlTransformConfiguration config)
		=> Config = config;

	public TreeVisitingMode VisitingMode => TreeVisitingMode.TopDown;
	public IXamlTransformConfiguration Config { get; }

	public bool ShouldSkipChildren(IXamlNode node) => false;

	public bool Transform(XamlLiteral node) => true;

	public bool Transform(XamlElement node)
	{
		if (!node.Properties.TryGetValue(new XamlPropertyIdentifier(XamlPropertyIdentifier.Xaml2009Uri, "TypeArguments"), out var nodes))
			return true;

		if (nodes.Count != 1 || nodes[0] is not XamlLiteral literal)
			return true;

		if (!TypeArgumentsParser.TryParseTypeArguments(literal.Literal, literal.NamespaceResolver, (IXamlSourceInfo)literal, out var typeArguments, Config.Logger)) {
			return false;
		}

		node.XamlType = new XamlType(node.XamlType.NamespaceUri, node.XamlType.Name, (List<XamlType>)typeArguments);
		return true;
	}
}