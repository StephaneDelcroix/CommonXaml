// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.using CommonXaml.Transforms;

using System;
using CommonXaml.Parser;
using CommonXaml.RuntimeInflator;
using CommonXaml.Validators;
using Microsoft.Extensions.Logging;

namespace CommonXaml.RuntimeInflatorTests;

class XamlParserConfiguration : IXamlParserConfiguration, IXamlVersionValidationConfiguration, IRuntimeInflatorConfiguration, IXamlTransformConfiguration
{
    public XamlParserConfiguration(Uri uri, XamlVersion minSupportedXamlVersion)
    {
        SourceUri = uri;
        MinSupportedXamlVersion = minSupportedXamlVersion;
    }

    public Uri SourceUri { get; set; }
    public XamlVersion MinSupportedXamlVersion { get; set; }

    public IXamlTypeResolver Resolver { get; }
        = new CachingXamlTypeResolver(new MockTypeSystem());

    public Action<IXamlElement, object>? OnActivatedCallback => null;

    public bool ContinueOnError { get; set; } = false;
    public ILogger? Logger => null;
}
