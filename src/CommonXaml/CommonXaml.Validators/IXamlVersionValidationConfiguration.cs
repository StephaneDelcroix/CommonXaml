// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace CommonXaml.Validators;

public interface IXamlVersionValidationConfiguration : IXamlNodeVisitorConfiguration
{
	XamlVersion MinSupportedXamlVersion { get; }
}