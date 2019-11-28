// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace CommonXaml
{
	public interface IXamlSourceInfo
	{
		int LineNumber { get; }
		int LinePosition { get; }
		Uri SourceUri { get; }
		public bool HasSourceInfo();
	}
}