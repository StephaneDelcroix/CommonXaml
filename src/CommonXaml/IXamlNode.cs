// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace CommonXaml
{
	public interface IXamlNode : IXamlSourceInfo
	{
		XamlElement? Parent { get; }
		IXamlNamespaceResolver NamespaceResolver { get; }
	}
}