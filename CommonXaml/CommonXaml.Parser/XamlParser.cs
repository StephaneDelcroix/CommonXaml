// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

using static CommonXaml.XamlExceptionCode;

namespace CommonXaml.Parser
{
	public class XamlParser
	{
		public IXamlParserConfiguration Config { get; }

		public XamlParser(IXamlParserConfiguration config) => Config = config;

		public bool TryProcess(XmlReader reader, out XamlElement rootnode, out IList<Exception> exceptions)
		{
			if (!TryParse(reader, out rootnode, out exceptions))
				return false;

			return exceptions == null || Config.ContinueOnError;
		}

		bool TryParse(XmlReader reader, out XamlElement rootnode, out IList<Exception> exceptions)
		{
			exceptions = null;
			rootnode = null;

			reader.MoveToContent();
			if (!TryParseElements(reader, out var roots, out var elementExceptions)) {
				AppendExceptions(ref exceptions, elementExceptions);
				return false;
			}

			rootnode = roots[0] as XamlElement;

			AppendExceptions(ref exceptions, elementExceptions);
			return exceptions == null || Config.ContinueOnError;
		}

		bool TryParseElements(XmlReader reader, out IList<IXamlNode> nodes, out IList<Exception> exceptions)
		{
			exceptions = null;
			Debug.Assert(reader.NodeType == XmlNodeType.Element);

			nodes = new List<IXamlNode>();

			do {
				switch (reader.NodeType) {
				case XmlNodeType.EndElement:
					return exceptions == null || Config.ContinueOnError;
				case XmlNodeType.Element:
					if (!TryParseElement(reader, out var xamlElement, out var elementExceptions)) {
						AppendExceptions(ref exceptions, elementExceptions);
						return false;
					}
					nodes.Add(xamlElement);
					AppendExceptions(ref exceptions, elementExceptions);
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
			return exceptions == null || Config.ContinueOnError;
		}

		bool TryParseElement(XmlReader reader, out XamlElement node, out IList<Exception> exceptions)
		{
			exceptions = null;
			Debug.Assert(reader.NodeType == XmlNodeType.Element);

			var type = new XamlType(reader.NamespaceURI, reader.LocalName, null);
			var resolver = new XamlNamespaceResolver(new XmlNamespaceManager(reader.NameTable));
			node = new XamlElement(type, resolver, Config.SourceUri, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition);

			if (!TryParseAttributeProperties(node, reader, out var attributeExceptions)) {
				AppendExceptions(ref exceptions, attributeExceptions);
				return false;
			}

			if (!TryParseElementProperties(node, reader, out var elementExceptions)) {
				AppendExceptions(ref exceptions, elementExceptions);
				return false;
			}

			AppendExceptions(ref exceptions, attributeExceptions);
			AppendExceptions(ref exceptions, elementExceptions);
			return exceptions == null || Config.ContinueOnError;
		}

		bool TryParseAttributeProperties(XamlElement element, XmlReader reader, out IList<Exception> exceptions)
		{
			exceptions = null;
			Debug.Assert(reader.NodeType == XmlNodeType.Element);

			for (var i = 0; i < reader.AttributeCount; i++) {
				if (exceptions != null && !Config.ContinueOnError) break;

				reader.MoveToAttribute(i);

				var propertyName = new XamlPropertyIdentifier(reader.NamespaceURI, reader.LocalName, Config.SourceUri, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition);
				var literal = new XamlLiteral(reader.Value.Trim(), new XamlNamespaceResolver((IXmlNamespaceResolver)reader), Config.SourceUri, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition);
				if (!element.TryAdd(propertyName, new List<IXamlNode> { literal }))
					(exceptions ??= new List<Exception>()).Add(new XamlParseException(CXAML1010, new[] { propertyName.LocalName}, propertyName));
			}

			reader.MoveToElement();
			return exceptions == null || Config.ContinueOnError;
		}

		bool TryParseElementProperties(XamlElement element, XmlReader reader, out IList<Exception> exceptions)
		{
			var elementNsUri = reader.NamespaceURI;
			exceptions = null;
			Debug.Assert(reader.NodeType == XmlNodeType.Element);

			if (reader.IsEmptyElement) {
				//reader.Read();
				return true;
			}

			while (reader.Read()) {
				switch (reader.NodeType) {
				case XmlNodeType.EndElement:
					Debug.Assert(reader.LocalName == element.XamlType.Name);
					return exceptions == null || Config.ContinueOnError;
				case XmlNodeType.Whitespace:
					break;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
					element.AddOrAppend(XamlPropertyName.ImplicitProperty, new XamlLiteral(reader.Value.Trim(), new XamlNamespaceResolver((IXmlNamespaceResolver)reader), Config.SourceUri, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition));
					break;
				case XmlNodeType.Element:
					IXamlPropertyName propertyName;
					if (   reader.Name.Contains(".")
						|| (reader.NamespaceURI == XamlPropertyName.Xaml2009Uri && reader.LocalName == "Arguments")) {
						propertyName = new XamlPropertyIdentifier(!string.IsNullOrEmpty(reader.NamespaceURI) ? reader.NamespaceURI : elementNsUri, reader.LocalName, Config.SourceUri, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition);

						if (reader.IsEmptyElement) {
							(exceptions ??= new List<Exception>()).Add(new XamlParseException(CXAML1011, new[] { reader.Name }, propertyName as IXamlSourceInfo));
							return exceptions == null || Config.ContinueOnError;
						}

						reader.Read(); //Consume the property opening tag. The closing tag will be used as a return indicator in TryParseElements
					}
					else
						propertyName = XamlPropertyName.ImplicitProperty;


					if (!TryParseElements(reader, out var valueNodes, out var elementExceptions)) {
						AppendExceptions(ref exceptions, elementExceptions);
						return false;
					}

					if (!element.TryAdd(propertyName, valueNodes))
						(exceptions ??= new List<Exception>()).Add(new XamlParseException(CXAML1010, new[] { reader.Name }, propertyName as IXamlSourceInfo));

					AppendExceptions(ref exceptions, elementExceptions);
					break;
				}
			}

			return exceptions == null || Config.ContinueOnError;
		}

		static void AppendExceptions(ref IList<Exception> exceptions, IList<Exception> additionalExceptions)
		{
			if (additionalExceptions == null)
				return;
			if (exceptions == null)
				exceptions = additionalExceptions;
			else
				foreach (var e in additionalExceptions)
					exceptions.Add(e);
		}
	}
}
