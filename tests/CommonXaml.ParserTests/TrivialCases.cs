// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using CommonXaml.Parser;
using CommonXaml.Validators;
using NUnit.Framework;

namespace CommonXaml.ParserTests;

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
		var (success, root) = parser!.Parse(
@"<Control Title=""Foo"" />");
		Assert.That(success, Is.True);

		var rootElement = (XamlElement)root;
		Assert.That(rootElement.XamlType, Is.EqualTo(new XamlType("", "Control", null)));
		Assert.That(rootElement.Properties.Count, Is.EqualTo(1));

		var key = ((IXamlElement)root).Properties.Keys.First();
		Assert.True((key.NamespaceUri, key.LocalName) == ("", "Title"));
		Assert.True(((XamlLiteral)rootElement.Properties[key].Single()).Literal == "Foo");
	}

	[Test]
	public void RootWithMultipleAttributeProperties()
	{
		var (success, root) = parser!.Parse(
@"<Control Title=""Foo"" Name=""Bar""/>");

		Assert.That(success, Is.True);
		var rootElement = (XamlElement)root;

		Assert.That(rootElement.XamlType, Is.EqualTo(new XamlType("", "Control")));
		Assert.That(rootElement.Properties.Count, Is.EqualTo(2));

		var key = rootElement.Properties.Keys.First();
		Assert.True((key.NamespaceUri, key.LocalName) == ("", "Title"));
		Assert.True(((XamlLiteral)rootElement.Properties[key].Single()).Literal == "Foo");

		key = ((IXamlElement)root).Properties.Keys.Skip(1).First();
		Assert.True((key.NamespaceUri, key.LocalName) == ("", "Name"));
		Assert.True(((XamlLiteral)rootElement.Properties[key].Single()).Literal == "Bar");
	}

	[Test]
	public void RootWithElementProperty()
	{
		var (success, root) = parser!.Parse(
@"<Control>
	<Control.Content>
		<View />
	</Control.Content>
</Control>");

		Assert.That(success, Is.True);
		var rootElement = (XamlElement)root;

		Assert.That(rootElement!.XamlType, Is.EqualTo(new XamlType("", "Control")));
		Assert.That(rootElement.Properties.Count, Is.EqualTo(1));

		var key = rootElement.Properties.Keys.First();
		Assert.True((key.NamespaceUri, key.LocalName) == ("", "Control.Content"));
		Assert.True(((XamlElement)rootElement.Properties[key].Single()).XamlType == new XamlType("", "View"));
	}

	[Test]
	public void RootWithMultipleElementProperties()
	{
		var (success, root) = parser!.Parse(
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

		Assert.That(success, Is.True);
		var rootElement = (XamlElement)root;

		Assert.That(rootElement.XamlType, Is.EqualTo(new XamlType("", "Control")));
		Assert.That(rootElement.XamlType, Is.EqualTo(new XamlType("", "Control")));
		Assert.That(rootElement.Properties.Count, Is.EqualTo(3));

		var key = rootElement.Properties.Keys.First();
		Assert.True((key.NamespaceUri, key.LocalName) == ("", "Control.Header"));
		Assert.True(((XamlElement)rootElement.Properties[key].Single()).XamlType == new XamlType("", "View"));

		key = ((IXamlElement)root).Properties.Keys.Skip(1).First();
		Assert.True((key.NamespaceUri, key.LocalName) == ("", "Control.Content"));
		Assert.True(((XamlElement)rootElement.Properties[key].Single()).XamlType == new XamlType("", "View"));

		key = ((IXamlElement)root).Properties.Keys.Skip(2).First();
		Assert.True((key.NamespaceUri, key.LocalName) == ("", "Control.Footer"));
		Assert.True(((XamlElement)rootElement.Properties[key].Single()).XamlType == new XamlType("", "FooterView"));
	}

	[Test]
	public void RootWithImplicitProperty()
	{
		var (success, root) = parser!.Parse(
@"<Control>
	<View />
</Control>");

		Assert.That(success, Is.True);
		var rootElement = (XamlElement)root;

		Assert.That(rootElement.XamlType, Is.EqualTo(new XamlType("", "Control", null)));
		Assert.That(rootElement.Properties.Count, Is.EqualTo(1));

		var key = (XamlPropertyIdentifier)rootElement.Properties.Keys.First();
		Assert.True(key.IsImplicitIdentifier);
		Assert.True(((XamlElement)rootElement.Properties[key].Single()).XamlType == new XamlType("", "View"));
	}

	[Test]
	public void MultiValuedProperty()
	{
		var (success, root) = parser!.Parse(
@"<Control>
	<Control.Content>
		<View />
		<OtherView />
		<View>
		</View>
	</Control.Content>
</Control>");

		Assert.That(success, Is.True);
		Assert.That(((IXamlElement)root)!.XamlType, Is.EqualTo(new XamlType("", "Control", null)));
		Assert.That(((IXamlElement)root).Properties.Count, Is.EqualTo(1));

		var key = ((IXamlElement)root).Properties.Keys.First();
		Assert.True((key.NamespaceUri, key.LocalName) == ("", "Control.Content"));
		Assert.True(((IXamlElement)root).Properties[key].Count == 3);
		Assert.True(((XamlElement)((IXamlElement)root).Properties[key].Skip(0).First()).XamlType == new XamlType("", "View"));
		Assert.True(((XamlElement)((IXamlElement)root).Properties[key].Skip(1).First()).XamlType == new XamlType("", "OtherView"));
		Assert.True(((XamlElement)((IXamlElement)root).Properties[key].Skip(2).First()).XamlType == new XamlType("", "View"));
	}

	[Test]
	public void MultiValuedImplicitProperty()
	{
		var (success, root) = parser!.Parse(
@"<Control>
	<View></View>
	<OtherView />
	<View />
</Control>");

		Assert.That(success, Is.True);

		var rootNode = root as XamlElement;
		Assert.That(rootNode!.XamlType, Is.EqualTo(new XamlType("", "Control", null)));
		Assert.That(rootNode.Properties.Count, Is.EqualTo(1));

		var key = (XamlPropertyIdentifier)rootNode.Properties.Keys.First();
		Assert.True(key.IsImplicitIdentifier);
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
		var (success, root) = parser!.Parse(validXaml09);
		Assert.That(success, Is.True);
	}

	[Test]
	public void RootNodeIsCorrectlyParsed()
	{
		var (success, root) = parser!.Parse(validXaml09);
		Assert.That(success, Is.True);

		var rootNode = root as XamlElement;
        Assert.That(rootNode!.XamlType, Is.EqualTo(new XamlType("http://commonxaml/controls", "Control", null)));
        Assert.That(rootNode.Properties.Count, Is.EqualTo(5));

        var key = rootNode.Properties.Keys.First();
        Assert.True((key.NamespaceUri, key.LocalName) == ("http://www.w3.org/2000/xmlns/", "xmlns"));

        Assert.True(((XamlLiteral)rootNode.Properties[key].Single()).Literal == "http://commonxaml/controls");

        key = rootNode.Properties.Keys.Skip(1).First();
        Assert.True((key.NamespaceUri, key.LocalName) == ("http://www.w3.org/2000/xmlns/", "x"));
        Assert.True(((XamlLiteral)rootNode.Properties[key].Single()).Literal == "http://schemas.microsoft.com/winfx/2009/xaml");

        key = rootNode.Properties.Keys.Skip(2).First();
        Assert.True((key.NamespaceUri, key.LocalName) == ("http://schemas.microsoft.com/winfx/2009/xaml", "Class"));

        Assert.True(((XamlLiteral)rootNode.Properties[key].Single()).Literal == "FooBar");

        key = rootNode.Properties.Keys.Skip(3).First();
        Assert.True((key.NamespaceUri, key.LocalName) == ("", "Title"));
        Assert.True(((XamlLiteral)rootNode.Properties[key].Single()).Literal == "ControlTitle");

        key = rootNode.Properties.Keys.Skip(4).First();
        Assert.True((key.NamespaceUri, key.LocalName) == ("http://commonxaml/controls", "Control.Content"));
        Assert.True(((XamlElement)rootNode.Properties[key].Single()).XamlType == new XamlType("http://commonxaml/controls", "View"));
	}
}