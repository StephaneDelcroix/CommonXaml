// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.using CommonXaml.Transforms;

using System;
using CommonXaml.Parser;
using CommonXaml.RuntimeInflator;
using CommonXaml.Validators;

namespace CommonXaml.RuntimeInflatorTests
{
    class XamlParserConfiguration : IXamlParserConfiguration, IXamlVersionValidationConfiguration, IRuntimeInflatorConfiguration
    {
        public XamlParserConfiguration(Uri uri, XamlVersion xaml2009)
        {
            SourceUri = uri;
            MinSupportedXamlVersion = xaml2009;
        }

        public Uri SourceUri { get; set; }
        public XamlVersion MinSupportedXamlVersion { get; set; }

        public bool ContinueOnError { get; set; } = false;
        public IXamlTypeResolver Resolver { get; } = new MockTypeSystem();

        public Action<XamlElement, object>? OnActivatedCallback => null;
    }
}
