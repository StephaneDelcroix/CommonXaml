// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace CommonXaml.Parser;

public interface IXamlParserConfiguration : ILoggingConfiguration
{
	Uri SourceUri { get; }
}
