// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace CommonXaml;

public interface IXamlNodeVisitor<TConfig> where TConfig : IXamlNodeVisitorConfiguration
{
		TreeVisitingMode VisitingMode  { get; }
		public bool ShouldSkipChildren(IXamlNode node);		

		bool Visit(IXamlLiteral node);
		bool Visit(IXamlElement node);

    TConfig Config { get; }
}

public static class VisitorExtensions
{
	public static bool Accept<TConfig>(this IXamlNode self, IXamlNodeVisitor<TConfig> visitor) where TConfig : IXamlNodeVisitorConfiguration
	{
		if (self is IXamlLiteral literal)
			return literal.Accept(visitor);
		else if (self is IXamlElement element)
			return element.Accept(visitor);
		else
			throw new NotImplementedException();
	}

	public static bool Accept<TConfig>(this IXamlLiteral self, IXamlNodeVisitor<TConfig> visitor) where TConfig : IXamlNodeVisitorConfiguration
		=> visitor.Visit(self);

	public static bool Accept<TConfig>(this IXamlElement self, IXamlNodeVisitor<TConfig> visitor) where TConfig : IXamlNodeVisitorConfiguration
	{
		var success = true;
		if (visitor.VisitingMode == TreeVisitingMode.TopDown && (success || visitor.Config.ContinueOnError))
			success &= visitor.Visit(self);

		if (!visitor.ShouldSkipChildren(self)) {
			foreach (var nodelist in self.Properties.Values)
				foreach (var node in nodelist)
					if (success || visitor.Config.ContinueOnError)
						success &= node.Accept(visitor);
		}

		if (visitor.VisitingMode == TreeVisitingMode.BottomUp && (success || visitor.Config.ContinueOnError))
			success &= visitor.Visit(self);

		return success;
	}
}