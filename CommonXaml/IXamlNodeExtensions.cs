// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace CommonXaml
{
	static class IXamlNodeExtensions
	{
		public static void SetParent(this IXamlNode node, IXamlNode parent)
		{
			if (node is XamlElement element) element.Parent = parent;
			else if (node is XamlLiteral literal) literal.Parent = parent;
			else throw new NotImplementedException();
		}
	}
}