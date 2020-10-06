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

namespace CommonXaml.RuntimeInflatorTests
{
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
		XamlParserConfiguration config = new XamlParserConfiguration(new Uri("test.xaml", UriKind.RelativeOrAbsolute), XamlVersion.Xaml2009);

        [SetUp]
        public void Setup() => parser = new XamlParser(config);

        [TearDown]
        public void TearDown() => parser = null;
        [Test()]
        public void TestCase()
        {
            using var textreader = new StringReader(xaml);
            using var xmlreader = XmlReader.Create(textreader);
            Assert.That(parser!.TryProcess(xmlreader, out var root, out _), Is.True);
            (true, root!).Transform(new ExpandMarkupExtensionsTransform())
                         .Transform(new ApplyTypeArgumentsTransform())
                         .Validate(new XamlVersionValidator(config))
						 .Visit(new ActivatorVisitor(config));
        }
    }
}
