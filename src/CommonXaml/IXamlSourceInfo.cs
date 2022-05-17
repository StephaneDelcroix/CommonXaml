// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace CommonXaml
{
	public interface IXamlSourceInfo
	{
		int LineNumber { get; }
		int LinePosition { get; }
		Uri? SourceUri { get; }
#if NETSTANDARD2_1_OR_GREATER
        public bool HasSourceInfo() => LineNumber >= 0 && LinePosition >= 0 && SourceUri != null;
#else
		bool HasSourceInfo();
#endif
	}
}