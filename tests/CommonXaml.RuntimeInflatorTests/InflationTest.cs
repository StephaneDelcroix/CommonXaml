// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.using CommonXaml.Transforms;

using NUnit.Framework;
using System;
using System.Xml;
using System.IO;

using CommonXaml.Parser;
using CommonXaml.Validators;
using CommonXaml.RuntimeInflator;
using CommonXaml.Transforms;

namespace CommonXaml.RuntimeInflatorTests;

[TestFixture()]
public class InflationTest
{
    readonly string xaml =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Control
		xmlns=""http://commonxaml/controls""
		xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
        Title=""foo"">
	<Control.Content>
		<View Text=""bar""/>
	</Control.Content>
</Control>";

		XamlParser? parser;
    readonly XamlParserConfiguration config = new(new Uri("test.xaml", UriKind.RelativeOrAbsolute), XamlVersion.Xaml2009);

    [SetUp]
    public void Setup() => parser = new XamlParser(config);

    [TearDown]
    public void TearDown() => parser = null;

    [Test]
    public void SimpleCase()
    {
        var context = new RuntimeContext();
        var (success, root) = parser!.Parse(xaml)
            .Transform(new ExpandMarkupExtensionsTransform(config))
            .Transform(new ApplyTypeArgumentsTransform(config))
            .Validate(new XamlVersionValidator(config))
            .Visit(new ActivatorVisitor(config, context));
        Assert.That(success, Is.True);
        Assert.That(context.Values[root], Is.TypeOf<Control>());
    }
}