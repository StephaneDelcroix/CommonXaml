// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.using CommonXaml.Transforms;

using System.Collections.Generic;
using CommonXaml.RuntimeInflator;

namespace CommonXaml.RuntimeInflatorTests;

class RuntimeContext : IActivatorContext
{
    public IDictionary<IXamlNode, object> Values { get; } = new Dictionary<IXamlNode, object>();
}
