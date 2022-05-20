// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace CommonXaml;

public interface IXamlSourceInfo
{
	int LineNumber { get; }
	int LinePosition { get; }
	Uri? SourceUri { get; }
}

public static class IXamlSourceInfoExtensions
{
	public static bool HasSourceInfo(this IXamlSourceInfo self)
		=> self.LineNumber >= 0 && self.LinePosition >= 0 && self.SourceUri != null;
}