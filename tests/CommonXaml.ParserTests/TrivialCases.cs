// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
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
	public class TrivialCases
	{
		XamlParser? parser;

		[SetUp]
		public void Setup()
		{
			var config = new XamlParserConfiguration(new Uri("test.xaml", UriKind.RelativeOrAbsolute), XamlVersion.Xaml2009);
			parser = new XamlParser(config);
		}

        [TearDown]
        public void TearDown() => parser = null;

        [Test]
		public void RootWithAttributeProperty()
		{
			using var textreader = new StringReader(
@"<Control Title=""Foo"" />");
			using var xmlreader = XmlReader.Create(textreader);
			Assert.That(parser!.TryProcess(xmlreader, out var rootNode, out _), Is.True);
			Assert.That(rootNode!.XamlType, Is.EqualTo(new XamlType("", "Control", null)));
			Assert.That(rootNode!.Properties.Count, Is.EqualTo(1));

			var key = rootNode.Properties.Keys.First();
			Assert.True(key == new XamlPropertyName("", "Title"));
			Assert.True(((XamlLiteral)rootNode.Properties[key].Single()).Literal == "Foo");
		}

		[Test]
		public void RootWithMultipleAttributeProperties()
		{
			using var textreader = new StringReader(
@"<Control Title=""Foo"" Name=""Bar""/>");
			using var xmlreader = XmlReader.Create(textreader);
			Assert.That(parser!.TryProcess(xmlreader, out var rootNode, out _), Is.True);
			Assert.That(rootNode!.XamlType, Is.EqualTo(new XamlType("", "Control")));
			Assert.That(rootNode.Properties.Count, Is.EqualTo(2));

			var key = rootNode.Properties.Keys.First();
			Assert.True(key == new XamlPropertyName("", "Title"));
			Assert.True(((XamlLiteral)rootNode.Properties[key].Single()).Literal == "Foo");

			key = rootNode.Properties.Keys.Skip(1).First();
			Assert.True(key == new XamlPropertyName("", "Name"));
			Assert.True(((XamlLiteral)rootNode.Properties[key].Single()).Literal == "Bar");
		}

		[Test]
		public void RootWithElementProperty()
		{
			using var textreader = new StringReader(
@"<Control>
	<Control.Content>
		<View />
	</Control.Content>
</Control>");
			using var xmlreader = XmlReader.Create(textreader);
			Assert.That(parser!.TryProcess(xmlreader, out var rootNode, out _), Is.True);
			Assert.That(rootNode!.XamlType, Is.EqualTo(new XamlType("", "Control")));
			Assert.That(rootNode.Properties.Count, Is.EqualTo(1));

			var key = rootNode.Properties.Keys.First();
			Assert.True(key == new XamlPropertyName("", "Control.Content"));
			Assert.True(((XamlElement)rootNode.Properties[key].Single()).XamlType == new XamlType("", "View"));
		}

		[Test]
		public void RootWithMultipleElementProperties()
		{
			using var textreader = new StringReader(
@"<Control>
	<Control.Header>
		<View />
	</Control.Header>
	<Control.Content>
		<View />
	</Control.Content>
	<Control.Footer>
		<FooterView />
	</Control.Footer>
</Control>");
			using var xmlreader = XmlReader.Create(textreader);
			Assert.That(parser!.TryProcess(xmlreader, out var rootNode, out _), Is.True);
			Assert.That(rootNode!.XamlType, Is.EqualTo(new XamlType("", "Control")));
			Assert.That(rootNode.Properties.Count, Is.EqualTo(3));

			var key = rootNode.Properties.Keys.First();
			Assert.True(key == new XamlPropertyName("", "Control.Header"));
			Assert.True(((XamlElement)rootNode.Properties[key].Single()).XamlType == new XamlType("", "View"));

			key = rootNode.Properties.Keys.Skip(1).First();
			Assert.True(key == new XamlPropertyName("", "Control.Content"));
			Assert.True(((XamlElement)rootNode.Properties[key].Single()).XamlType == new XamlType("", "View"));

			key = rootNode.Properties.Keys.Skip(2).First();
			Assert.True(key == new XamlPropertyName("", "Control.Footer"));
			Assert.True(((XamlElement)rootNode.Properties[key].Single()).XamlType == new XamlType("", "FooterView"));
		}

		[Test]
		public void RootWithImplicitProperty()
		{
			using var textreader = new StringReader(
@"<Control>
	<View />
</Control>");
			using var xmlreader = XmlReader.Create(textreader);
			Assert.That(parser!.TryProcess(xmlreader, out var rootNode, out _), Is.True);
			Assert.That(rootNode!.XamlType, Is.EqualTo(new XamlType("", "Control", null)));
			Assert.That(rootNode.Properties.Count, Is.EqualTo(1));

			var key = rootNode.Properties.Keys.First();
			Assert.True(key == XamlPropertyName.ImplicitProperty);
			Assert.True(((XamlElement)rootNode.Properties[key].Single()).XamlType == new XamlType("", "View"));
		}

		[Test]
		public void MultiValuedProperty()
		{
			using var textreader = new StringReader(
@"<Control>
	<Control.Content>
		<View />
		<OtherView />
		<View>
		</View>
	</Control.Content>
</Control>");
			using var xmlreader = XmlReader.Create(textreader);
			Assert.That(parser!.TryProcess(xmlreader, out var rootNode, out _), Is.True);
			Assert.That(rootNode!.XamlType, Is.EqualTo(new XamlType("", "Control", null)));
			Assert.That(rootNode.Properties.Count, Is.EqualTo(1));

			var key = rootNode.Properties.Keys.First();
			Assert.True(key == new XamlPropertyName("", "Control.Content"));
			Assert.True(rootNode.Properties[key].Count == 3);
			Assert.True(((XamlElement)rootNode.Properties[key].Skip(0).First()).XamlType == new XamlType("", "View"));
			Assert.True(((XamlElement)rootNode.Properties[key].Skip(1).First()).XamlType == new XamlType("", "OtherView"));
			Assert.True(((XamlElement)rootNode.Properties[key].Skip(2).First()).XamlType == new XamlType("", "View"));
		}

		[Test]
		public void MultiValuedImplicitProperty()
		{
			using var textreader = new StringReader(
@"<Control>
	<View></View>
	<OtherView />
	<View />
</Control>");
			using var xmlreader = XmlReader.Create(textreader);
			Assert.That(parser!.TryProcess(xmlreader, out var rootNode, out _), Is.True);
			Assert.That(rootNode!.XamlType, Is.EqualTo(new XamlType("", "Control", null)));
			Assert.That(rootNode.Properties.Count, Is.EqualTo(1));

			var key = rootNode.Properties.Keys.First();
			Assert.True(key == XamlPropertyName.ImplicitProperty);
			Assert.True(rootNode.Properties[key].Count == 3);
			Assert.True(((XamlElement)rootNode.Properties[key].Skip(0).First()).XamlType == new XamlType("", "View"));
			Assert.True(((XamlElement)rootNode.Properties[key].Skip(1).First()).XamlType == new XamlType("", "OtherView"));
			Assert.True(((XamlElement)rootNode.Properties[key].Skip(2).First()).XamlType == new XamlType("", "View"));
		}

		const string validXaml09 =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Control
		xmlns=""http://commonxaml/controls""
		xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
		x:Class=""FooBar""
		Title=""ControlTitle"">
	<Control.Content>
		<View x:Name=""aView"" Text=""Hello, World!"">
			<ImplicitContentView />
			<ImplicitContentView />
			<ImplicitContentView />
		</View>
	</Control.Content>
</Control>
";

		[Test]
		public void XamlParsedSuccessfully()
		{
			using var textreader = new StringReader(validXaml09);
			using var xmlreader = XmlReader.Create(textreader);
			Assert.That(parser!.TryProcess(xmlreader, out _, out _), Is.True);
		}

		[Test]
		public void RootNodeIsCorrectlyParsed()
		{
			using var textreader = new StringReader(validXaml09);
			using var xmlreader = XmlReader.Create(textreader);
			Assert.That(parser!.TryProcess(xmlreader, out var rootNode, out _), Is.True);
			Assert.That(rootNode!.XamlType, Is.EqualTo(new XamlType("http://commonxaml/controls", "Control", null)));
			Assert.That(rootNode.Properties.Count, Is.EqualTo(5));


			var key = rootNode.Properties.Keys.First();
			Assert.True(key == new XamlPropertyName("http://www.w3.org/2000/xmlns/", "xmlns"));
			Assert.True(((XamlLiteral)rootNode.Properties[key].Single()).Literal == "http://commonxaml/controls");

			key = rootNode.Properties.Keys.Skip(1).First();
			Assert.True(key == new XamlPropertyName("http://www.w3.org/2000/xmlns/", "x"));
			Assert.True(((XamlLiteral)rootNode.Properties[key].Single()).Literal == "http://schemas.microsoft.com/winfx/2009/xaml");

			key = rootNode.Properties.Keys.Skip(2).First();
			Assert.True(key == new XamlPropertyName("http://schemas.microsoft.com/winfx/2009/xaml", "Class"));
			Assert.True(((XamlLiteral)rootNode.Properties[key].Single()).Literal == "FooBar");

			key = rootNode.Properties.Keys.Skip(3).First();
			Assert.True(key == new XamlPropertyName("", "Title"));
			Assert.True(((XamlLiteral)rootNode.Properties[key].Single()).Literal == "ControlTitle");

			key = rootNode.Properties.Keys.Skip(4).First();
			Assert.True(key == new XamlPropertyName("http://commonxaml/controls", "Control.Content"));
			Assert.True(((XamlElement)rootNode.Properties[key].Single()).XamlType == new XamlType("http://commonxaml/controls", "View"));
		}
	}
}
