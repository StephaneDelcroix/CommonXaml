// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace CommonXaml.Parser
{
	class XamlSourceInfo : IXamlSourceInfo
	{
		public XamlSourceInfo(Uri sourceUri, int lineNumber, int linePosition)
		{
			SourceUri = sourceUri;
			LineNumber = lineNumber;
			LinePosition = linePosition;
		}

		public int LineNumber { get; set; }
		public int LinePosition { get; set; }
		public Uri SourceUri { get; set; }
#if !NETSTANDARD2_1_OR_GREATER
		public bool HasSourceInfo() => LineNumber >= 0 && LinePosition >= 0 && SourceUri != null;
#endif
	}
}