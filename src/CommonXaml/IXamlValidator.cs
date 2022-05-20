// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace CommonXaml;

public interface IXamlValidator<TConfig> : IXamlNodeVisitor<TConfig> where TConfig : IXamlNodeVisitorConfiguration
{
}