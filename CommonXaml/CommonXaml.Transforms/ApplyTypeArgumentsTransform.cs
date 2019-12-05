﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace CommonXaml.Transforms
{
	public class ApplyTypeArgumentsTransform : IXamlTransform
	{
		public XamlParserConfiguration Config { get; }
		public IXamlTransform.TreeVisitingMode VisitingMode => IXamlTransform.TreeVisitingMode.TopDown;
		public IList<Exception> TransformExceptions { get; private set; }
		public bool ShouldSkipChildren(IXamlNode node) => false;
		public ApplyTypeArgumentsTransform(XamlParserConfiguration config) => Config = config;

		public void Transform(XamlLiteral node)
		{
		}

		public void Transform(XamlElement node)
		{
			if (!node.Properties.TryGetValue(new XamlPropertyName(XamlPropertyName.Xaml2009Uri, "TypeArguments"), out var nodes))
				return;

			if (nodes.Count != 1 || !(nodes[0] is XamlLiteral literal))
				return;

			if (!TypeArgumentsParser.TryParseTypeArguments(literal.Literal, literal.NamespaceResolver, (IXamlSourceInfo)literal, out var typeArguments, out var exceptions)) {
				((List<Exception>)(TransformExceptions ??= new List<Exception>())).AddRange(exceptions);
				return;
			}

			node.XamlType = new XamlType(node.XamlType.NamespaceUri, node.XamlType.Name, (List<XamlType>)typeArguments);
		}
	}
}
