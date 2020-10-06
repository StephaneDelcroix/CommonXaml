// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace CommonXaml
{
	public static class IXamlNodeExtensions
	{
		internal static void SetParent(this IXamlNode node, XamlElement parent)
		{
			if (node is XamlElement element) element.Parent = parent;
			else if (node is XamlLiteral literal) literal.Parent = parent;
			else throw new NotImplementedException();
		}

		public static (bool success, IXamlNode root) Transform(this (bool success, IXamlNode root) continuation, IXamlTransform transform)
		{
			continuation.root.Accept(transform);
			return (continuation.success && transform.Errors == null, continuation.root);
		}

		public static (bool success, IXamlNode root) Validate(this (bool success, IXamlNode root) continuation, IXamlValidator validator)
			=> continuation.Visit(validator);		

		public static (bool success, IXamlNode root) Visit(this (bool success, IXamlNode root) continuation, IXamlNodeVisitor visitor)
		{
			continuation.root.Accept(visitor);
			return (continuation.success && visitor.Errors == null, continuation.root);
		}
	}
}