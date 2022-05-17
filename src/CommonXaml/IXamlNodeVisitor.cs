// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace CommonXaml
{
	public interface IXamlNodeVisitor
	{
		TreeVisitingMode VisitingMode  { get; }
		public bool ShouldSkipChildren(IXamlNode node);
		IList<Exception>? Errors { get; }

		void Visit(XamlLiteral node);
		void Visit(XamlElement node);

		public enum TreeVisitingMode
		{
			TopDown,
			BottomUp,
		}
    }

	public static class VisitorExtensions
	{
		public static void Accept(this IXamlNode self, IXamlNodeVisitor visitor)
		{
			if (self is XamlLiteral literal)
				literal.Accept(visitor);
			else if (self is XamlElement element)
				element.Accept(visitor);
			else
				throw new NotImplementedException();
		}

		public static void Accept(this XamlLiteral self, IXamlNodeVisitor visitor) => visitor.Visit(self);

		public static void Accept(this XamlElement self, IXamlNodeVisitor visitor)
		{
			if (visitor.VisitingMode == IXamlNodeVisitor.TreeVisitingMode.TopDown)
				visitor.Visit(self);

			if (!visitor.ShouldSkipChildren(self)) {
				foreach (var nodelist in self.Properties.Values)
					foreach (var node in nodelist)
						node.Accept(visitor);
			}

			if (visitor.VisitingMode == IXamlNodeVisitor.TreeVisitingMode.BottomUp)
				visitor.Visit(self);
		}
	}
}