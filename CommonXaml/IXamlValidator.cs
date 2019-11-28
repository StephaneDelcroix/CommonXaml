// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace CommonXaml
{
	public interface IXamlValidator : IXamlNodeVisitor
	{
		IList<Exception> ValidationErrors { get; }
	}
}