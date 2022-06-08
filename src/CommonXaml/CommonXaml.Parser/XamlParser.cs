// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

using static CommonXaml.XamlExceptionCode;

namespace CommonXaml.Parser;

public class XamlParser
{
	public IXamlParserConfiguration Config { get; }

	public XamlParser(IXamlParserConfiguration config) => Config = config;

    public (bool success, IXamlNode root) Parse(XmlReader reader)
		=> (TryParse(reader, out XamlElement? rootNode), rootNode as IXamlNode ?? new EmptyNode());

    bool TryParse(XmlReader reader, out XamlElement? rootnode)
	{
		try {
			reader.MoveToContent();
			var success = TryParseElements(reader, out var roots);

			if (roots is not null && roots.Count > 0)
				rootnode = roots[0] as XamlElement;
			else
				rootnode = null;

			return success;
		} catch (Exception e) {
			Config.Logger.LogException(e);

			rootnode = null;
			return false;
		}
	}

	bool TryParseElements(XmlReader reader, out IList<IXamlNode> nodes)
	{
		var success = true;

		Debug.Assert(reader.NodeType == XmlNodeType.Element);

		nodes = new List<IXamlNode>();

		do {
			switch (reader.NodeType) {
			case XmlNodeType.EndElement:
				return success;
			case XmlNodeType.Element:
				success &= TryParseElement(reader, out var xamlElement);
				if (!success && !Config.ContinueOnError)
					return false;
				nodes.Add(xamlElement);
				break;
			case XmlNodeType.Whitespace:
				break;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				if (nodes.Count == 1 && nodes[0] is XamlLiteral literal)
					literal.Literal += reader.Value.Trim();
				else
					nodes.Add(new XamlLiteral(reader.Value.Trim(), new XamlNamespaceResolver((IXmlNamespaceResolver)reader), Config.SourceUri, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition));
				break;
			}
		} while (reader.Read());
		return success;
	}

	bool TryParseElement(XmlReader reader, out XamlElement node)
	{
		var success = true;

		Debug.Assert(reader.NodeType == XmlNodeType.Element);

		var type = new XamlType(reader.NamespaceURI, reader.LocalName, null);
		var resolver = new XamlNamespaceResolver(new XmlNamespaceManager(reader.NameTable));
		node = new XamlElement(type, resolver, Config.SourceUri, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition);

		success &= TryParseAttributeProperties(node, reader);
		if (!success && !Config.ContinueOnError)
			return false;

		success &= TryParseElementProperties(node, reader);
        if (!success && !Config.ContinueOnError)
            return false;

        return success;
	}

	bool TryParseAttributeProperties(XamlElement element, XmlReader reader)
	{
		Debug.Assert(reader.NodeType == XmlNodeType.Element);
		var success = true;

		for (var i = 0; i < reader.AttributeCount; i++) {
			if (!success && !Config.ContinueOnError)
				break;

			reader.MoveToAttribute(i);

			var propertyName = new XamlPropertyIdentifier(reader.NamespaceURI, reader.LocalName, Config.SourceUri, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition);
			var literal = new XamlLiteral(reader.Value.Trim(), new XamlNamespaceResolver((IXmlNamespaceResolver)reader), Config.SourceUri, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition);
			if (!element.TryAdd(propertyName, new List<IXamlNode> { literal })) {
				Config.Logger.LogXamlParseException(CXAML1010, new[] { propertyName.LocalName }, propertyName);
				success = false;
            }	
		}

		reader.MoveToElement();
		return success;
	}

	bool TryParseElementProperties(XamlElement element, XmlReader reader)
	{
		Debug.Assert(reader.NodeType == XmlNodeType.Element);
		var success = true;
		var elementNsUri = reader.NamespaceURI;

		if (reader.IsEmptyElement) {
			//reader.Read();
			return true;
		}

		while (reader.Read()) {
			switch (reader.NodeType) {
			case XmlNodeType.EndElement:
				Debug.Assert(reader.LocalName == element.XamlType.Name);
				return success;
			case XmlNodeType.Whitespace:
				break;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				element.AddOrAppend(XamlPropertyIdentifier.CreateImplicitIdentifier(Config.SourceUri, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition),
									new XamlLiteral(reader.Value.Trim(), new XamlNamespaceResolver((IXmlNamespaceResolver)reader), Config.SourceUri, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition));
				break;
			case XmlNodeType.Element:
				IXamlPropertyIdentifier propertyName;
				if (   reader.Name.Contains(".")
					|| (reader.NamespaceURI == XamlPropertyIdentifier.Xaml2009Uri && reader.LocalName == "Arguments")) {
					propertyName = new XamlPropertyIdentifier(!string.IsNullOrEmpty(reader.NamespaceURI) ? reader.NamespaceURI : elementNsUri, reader.LocalName, Config.SourceUri, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition);

					if (reader.IsEmptyElement) {
						Config.Logger.LogXamlParseException(CXAML1011, new[] { reader.Name }, (IXamlSourceInfo)propertyName);
						return false;
					}

					reader.Read(); //Consume the property opening tag. The closing tag will be used as a return indicator in TryParseElements
				} else
					propertyName = XamlPropertyIdentifier.CreateImplicitIdentifier(Config.SourceUri, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition);

				success &= TryParseElements(reader, out var valueNodes);
				if (!success && !Config.ContinueOnError)
					return false;

				if (!element.TryAdd(propertyName, valueNodes)) {
					success = false;
					Config.Logger.LogXamlParseException(CXAML1010, new[] { reader.Name }, propertyName as IXamlSourceInfo ?? element);
				}
				break;
			}
		}

		return success;
	}
}

public static class ParserExtensions
{
    public static (bool success, IXamlNode root) Parse(this XamlParser parser, string xaml)
    {
        using var textreader = new System.IO.StringReader(xaml);
        using var xmlreader = XmlReader.Create(textreader);
        return parser.Parse(xmlreader);
    }
}
