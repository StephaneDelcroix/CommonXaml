// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace CommonXaml.RuntimeInflator
{
    public interface IRuntimeInflatorConfiguration
    {
        IXamlTypeResolver Resolver { get; }

        Action<XamlElement, object>? OnActivatedCallback { get; } 
    }
}
