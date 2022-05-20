// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;

namespace CommonXaml;

public interface IXamlTransform<TConfig> where TConfig : IXamlTransformConfiguration
{
	TreeVisitingMode VisitingMode { get; }
	public bool ShouldSkipChildren(IXamlNode node);

	bool Transform(XamlLiteral node);
	bool Transform(XamlElement node);

	TConfig Config { get; }
}

public static class TransformExtensions
{
	public static bool Accept<TConfig>(this IXamlNode self, IXamlTransform<TConfig> transform) where TConfig : IXamlTransformConfiguration
	{
		if (self is XamlLiteral literal)
			return literal.Accept(transform);
		else if (self is XamlElement element)
			return element.Accept(transform);
		else
			throw new NotImplementedException();
	}

	static bool Accept<TConfig>(this XamlLiteral self, IXamlTransform<TConfig> transform) where TConfig : IXamlTransformConfiguration
		=> transform.Transform(self);

	static bool Accept<TConfig>(this XamlElement self, IXamlTransform<TConfig> transform) where TConfig : IXamlTransformConfiguration
	{
		var success = true;
		if (transform.VisitingMode == TreeVisitingMode.TopDown && (success || transform.Config.ContinueOnError))
			success &= transform.Transform(self);

		if (!transform.ShouldSkipChildren(self)) {
			foreach (var nodelist in self.Properties.Values)
				foreach (var node in nodelist.ToList())
					if (success || transform.Config.ContinueOnError)
						success &= node.Accept(transform);
		}

		if (transform.VisitingMode == TreeVisitingMode.BottomUp && (success || transform.Config.ContinueOnError))
			success &= transform.Transform(self);

		return success;
	}
}