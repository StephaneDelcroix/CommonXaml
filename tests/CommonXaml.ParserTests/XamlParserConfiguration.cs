// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using CommonXaml.Parser;
using CommonXaml.Validators;

namespace CommonXaml.ParserTests
{
	class XamlParserConfiguration : IXamlParserConfiguration, IXamlVersionValidationConfiguration
	{
		public Uri SourceUri { get; set; }
		public XamlVersion MinSupportedXamlVersion { get; set; }
		public bool ContinueOnError { get; set; } = false;
	}
}