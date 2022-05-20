// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Xml;
using CommonXaml.Parser;
using CommonXaml.Transforms;
using CommonXaml.Validators;
using NUnit.Framework;

namespace CommonXaml.ParserTests;

[TestFixture]
public class MarkupExpressionTests
{
	XamlParser? parser;
	XamlParserConfiguration config = new(new Uri("test.xaml", UriKind.RelativeOrAbsolute), XamlVersion.Xaml2009);

	[SetUp]
	public void Setup() => parser = new XamlParser(config);

	[TearDown]
	public void TearDown() => parser = null;

	const string xaml = "<Control xmlns=\"http://commonxaml/controls\" xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\" Text=\"{0}\" />";

	[Test]
	public void EmptyMarkup()
	{
		var (success, root) = parser!.Parse(string.Format(xaml, "{Binding}"))
			.Transform(new ExpandMarkupExtensionsTransform(config))
			.Transform(new ApplyTypeArgumentsTransform(config))
			.Validate(new XamlVersionValidator(config));

		Assert.That(success, Is.True);
		var rootElement = root as XamlElement;

		Assert.True(rootElement!.TryGetProperty(("", "Text"), out var values));
		var binding = values.First() as XamlElement;
		Assert.That(binding!.XamlType, Is.EqualTo(new XamlType("http://commonxaml/controls", "Binding", null)));
	}

	[Test]
	public void MarkupWithExplicitProperty()
	{
		var (success, root) = parser!.Parse(string.Format(xaml, "{Binding Path=Foo}"))
			.Transform(new ExpandMarkupExtensionsTransform(config))
			.Transform(new ApplyTypeArgumentsTransform(config))
			.Validate(new XamlVersionValidator(config));

		Assert.That(success, Is.True);
		var rootElement = root as XamlElement;

		Assert.True(rootElement!.TryGetProperty(("", "Text"), out var values));
		var binding = values.First() as XamlElement;
		Assert.That(binding!.XamlType, Is.EqualTo(new XamlType("http://commonxaml/controls", "Binding", null)));
		Assert.That(binding.Properties.Count, Is.EqualTo(1));
		var propName = (binding.Properties.First().Key.NamespaceUri, binding.Properties.First().Key.LocalName);
		Assert.That(propName, Is.EqualTo(("", "Path")));
		Assert.That((binding.Properties.First().Value.First() as XamlLiteral)!.Literal, Is.EqualTo("Foo"));
	}
}
