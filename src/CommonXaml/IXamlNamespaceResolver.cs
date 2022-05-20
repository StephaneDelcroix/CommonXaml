// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace CommonXaml;

public interface IXamlNamespaceResolver
{
	string LookupNamespace(string prefix);
	string LookupPrefix(string namespaceName);
}