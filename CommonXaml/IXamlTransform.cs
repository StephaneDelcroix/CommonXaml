// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace CommonXaml
{
	public interface IXamlTransform
	{
		XamlParserConfiguration Config { get; }
		TreeVisitingMode VisitingMode { get; }
		IList<Exception> TransformExceptions { get; }

		public bool ShouldSkipChildren(IXamlNode node);
		void Transform(XamlLiteral node);
		void Transform(XamlElement node);

		public enum TreeVisitingMode
		{
			TopDown,
			BottomUp,
		}
	}

	public static class TransformExtensions
	{
		public static void Accept(this IXamlNode self, IXamlTransform visitor)
		{
			if (self is XamlLiteral literal)
				literal.Accept(visitor);
			else if (self is XamlElement element)
				element.Accept(visitor);
			else
				throw new NotImplementedException();
		}

		static void Accept(this XamlLiteral self, IXamlTransform transform) => transform.Transform(self);

		static void Accept(this XamlElement self, IXamlTransform transform)
		{
			if (transform.VisitingMode == IXamlTransform.TreeVisitingMode.TopDown)
				transform.Transform(self);

			if (!transform.ShouldSkipChildren(self)) {
				foreach (var nodelist in self.Properties.Values)
					foreach (var node in nodelist)
						node.Accept(transform);
			}

			if (transform.VisitingMode == IXamlTransform.TreeVisitingMode.BottomUp)
				transform.Transform(self);
		}
	}
}