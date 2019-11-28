// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace CommonXaml
{
	public interface IXamlNode : IXamlSourceInfo
	{
		IXamlNode Parent { get; }
		IXamlNamespaceResolver NamespaceResolver { get; }
	}
}