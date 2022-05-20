// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace CommonXaml.Parser;

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
}