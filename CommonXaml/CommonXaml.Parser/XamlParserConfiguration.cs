// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace CommonXaml.Parser
{
	public interface IXamlParserConfiguration
	{
		public Uri SourceUri { get; }
		public bool ContinueOnError { get; }
	}	
}