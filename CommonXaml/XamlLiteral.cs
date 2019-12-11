// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;

namespace CommonXaml
{
	[DebuggerDisplay("{Literal}")]
	public class XamlLiteral : IXamlNode
	{
		public XamlLiteral(string literal, IXamlNamespaceResolver namespaceResolver = null, Uri sourceUri = null, int lineNumber = -1, int linePosition = -1)
		{
			Literal = literal;
			NamespaceResolver = namespaceResolver;
			SourceUri = sourceUri;
			LineNumber = lineNumber;
			LinePosition = linePosition;
		}

		public string Literal { get; internal set; }

		public XamlElement Parent { get; internal set; }
		public IXamlNamespaceResolver NamespaceResolver { get; }

		public int LineNumber { get; }
		public int LinePosition { get; }
		public Uri SourceUri { get; }

		public bool HasSourceInfo() => LineNumber >= 0 && LinePosition >= 0 && SourceUri != null;
	}
}