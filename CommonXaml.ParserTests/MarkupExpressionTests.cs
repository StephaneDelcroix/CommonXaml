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

namespace CommonXaml.ParserTests
{
	[TestFixture]
	public class MarkupExpressionTests
	{
		XamlParser parser;
		XamlParserConfiguration config;

		[SetUp]
		public void Setup()
		{
			config = new XamlParserConfiguration {
				SourceUri = new Uri("test.xaml", UriKind.RelativeOrAbsolute),
				MinSupportedXamlVersion = XamlVersion.Xaml2009,
			};
			parser = new XamlParser(config);
		}

		[TearDown]
		public void TearDown()
		{
			parser = null;
		}

		const string xaml = "<Control xmlns=\"http://commonxaml/controls\" xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\" Text=\"{0}\" />";

		[Test]
		public void EmptyMarkup()
		{
			using var textreader = new StringReader(string.Format(xaml, "{Binding}"));
			using var xmlreader = XmlReader.Create(textreader);
			Assert.That(parser.TryProcess(xmlreader, out var root, out _), Is.True);
			(true, root).Transform(new ExpandMarkupExtensionsTransform())
						.Transform(new ApplyTypeArgumentsTransform())
						.Validate(new XamlVersionValidator(config));
			var binding = root.Properties[new XamlPropertyName("", "Text")].First() as XamlElement;
			Assert.That(binding.XamlType, Is.EqualTo(new XamlType("http://commonxaml/controls", "Binding", null)));
		}

		[Test]
		public void MarkupWithExplicitProperty()
		{
			using var textreader = new StringReader(string.Format(xaml, "{Binding Path=Foo}"));
			using var xmlreader = XmlReader.Create(textreader);
			Assert.That(parser.TryProcess(xmlreader, out var root, out _), Is.True);
			(true, root).Transform(new ExpandMarkupExtensionsTransform())
						.Transform(new ApplyTypeArgumentsTransform())
						.Validate(new XamlVersionValidator(config));
			var binding = root.Properties[new XamlPropertyName("", "Text")].First() as XamlElement;
			Assert.That(binding.XamlType, Is.EqualTo(new XamlType("http://commonxaml/controls", "Binding", null)));
			Assert.That(binding.Properties.Count, Is.EqualTo(1));
			Assert.That(binding.Properties.First().Key, Is.EqualTo(new XamlPropertyName("","Path")));
			Assert.That((binding.Properties.First().Value.First() as XamlLiteral).Literal, Is.EqualTo("Foo"));
		}
	}
}
