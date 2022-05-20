// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace CommonXaml;

public interface IXamlLiteral : IXamlNode
{
	string Literal { get; }
}