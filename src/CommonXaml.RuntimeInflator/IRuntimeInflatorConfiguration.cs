// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace CommonXaml.RuntimeInflator;

public interface IRuntimeInflatorConfiguration : IXamlNodeVisitorConfiguration
{
    IXamlTypeResolver Resolver { get; }
    Action<IXamlElement, object>? OnActivatedCallback { get; } 
}
