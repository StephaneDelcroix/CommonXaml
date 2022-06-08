// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using CommonXaml.Parser;
using CommonXaml.Validators;
using Microsoft.Extensions.Logging;

namespace CommonXaml.ParserTests;

class XamlParserConfiguration : IXamlParserConfiguration, IXamlVersionValidationConfiguration, IXamlTransformConfiguration
{
    public XamlParserConfiguration(Uri sourceUri, XamlVersion minSupportedVersion, LogLevel logLevel = LogLevel.Error)
    {
        SourceUri = sourceUri;
        MinSupportedXamlVersion = minSupportedVersion;
        Logger = new MockLogger(logLevel);
    }

    public Uri SourceUri { get; }
    public XamlVersion MinSupportedXamlVersion { get; }
    public bool ContinueOnError { get; } = false;
    public ILogger? Logger { get; }
}
