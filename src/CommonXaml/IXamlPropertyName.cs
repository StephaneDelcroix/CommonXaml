// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace CommonXaml
{
	public interface IXamlPropertyIdentifier
	{
		public string NamespaceUri { get; }
		public string LocalName { get; }
	}
}