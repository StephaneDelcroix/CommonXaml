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

namespace CommonXaml.ParserTests;

[TestFixture]
public class GenericsTests
{
	XamlParser? parser;
	XamlParserConfiguration config = new XamlParserConfiguration(new Uri("test.xaml", UriKind.RelativeOrAbsolute), XamlVersion.Xaml2009);

	[SetUp]
	public void Setup()
	{
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
		var (success, root) = parser!.Parse(genericXaml)
			.Transform(new ExpandMarkupExtensionsTransform(config))
			.Transform(new ApplyTypeArgumentsTransform(config))
			.Validate(new XamlVersionValidator(config));

		Assert.That(success, Is.True);
		var rootElement = root as XamlElement;

		var content = rootElement!.Properties[new XamlPropertyIdentifier("http://commonxaml/controls", "Control.Content")][0] as XamlElement;
		Assert.True(content!.XamlType ==
			new XamlType("clr-namespace:System.Collections.Generic;assembly=mscorlib", "List", new List<XamlType>{
					new XamlType("clr-namespace:System.Collections.Generic;assembly=mscorlib", "KeyValuePair", new List<XamlType> {
						new XamlType("http://schemas.microsoft.com/winfx/2009/xaml", "String"),
						new XamlType("http://schemas.microsoft.com/winfx/2009/xaml", "String"),
					})
			}));
	}
}
