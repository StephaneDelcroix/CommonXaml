// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CommonXaml.Parser;
using CommonXaml.Transforms;
using CommonXaml.Validators;
using NUnit.Framework;

namespace CommonXaml.ParserTests
{
	[TestFixture]
	public class GenericsTests
	{
		XamlParser parser;

		[SetUp]
		public void Setup()
		{
			var config = new XamlParserConfiguration {
				SourceUri = new Uri("test.xaml", UriKind.RelativeOrAbsolute),
				MinSupportedXamlVersion = XamlVersion.Xaml2009,
			};
			config.Transforms = new List<IXamlTransform> {
				new ExpandMarkupExtensionsTransform(config),
				new ApplyTypeArgumentsTransform(config),
			};
			config.Validators = new List<IXamlValidator> { new XamlVersionValidator(config) };

			parser = new XamlParser(config);
		}

		[TearDown]
		public void TearDown()
		{
			parser = null;
		}

		const string genericXaml =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Control
		xmlns=""http://commonxaml/controls""
		xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
		xmlns:scg=""clr-namespace:System.Collections.Generic;assembly=mscorlib"">
	<Control.Content>
		<scg:List x:TypeArguments=""scg:KeyValuePair(x:String, x:String)"" />
	</Control.Content>
</Control>
";

		[Test]
		public void TypeArgumentsAreApplied()
		{
			using var textreader = new StringReader(genericXaml);
			using var xmlreader = XmlReader.Create(textreader);
			Assert.That(parser.TryProcess(xmlreader, out var root, out _), Is.True);
			var content = root.Properties[new XamlPropertyIdentifier("http://commonxaml/controls", "Control.Content")][0] as XamlElement;
			Assert.True(content.XamlType ==
				new XamlType("clr-namespace:System.Collections.Generic;assembly=mscorlib", "List", new List<XamlType>{
					new XamlType("clr-namespace:System.Collections.Generic;assembly=mscorlib", "KeyValuePair", new List<XamlType> {
						new XamlType("http://schemas.microsoft.com/winfx/2009/xaml", "String"),
						new XamlType("http://schemas.microsoft.com/winfx/2009/xaml", "String"),
					})
				}));
		}
	}
}
