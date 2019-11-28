// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace CommonXaml.Transforms
{
	public class ExpandMarkupExtensionsTransform : IXamlTransform
	{
		public XamlParserConfiguration Config { get; }
		public IXamlTransform.TreeVisitingMode VisitingMode => IXamlTransform.TreeVisitingMode.TopDown;
		public IList<Exception> TransformExceptions { get; private set; }
		public bool ShouldSkipChildren(IXamlNode node) => false;
		public ExpandMarkupExtensionsTransform(XamlParserConfiguration config) => Config = config;

		public void Transform(XamlLiteral node)
		{
		}

		public void Transform(XamlElement node)
		{
		}
	}
}
