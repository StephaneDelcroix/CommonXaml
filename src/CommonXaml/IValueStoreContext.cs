// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;

namespace CommonXaml
{
    public interface IValueStoreContext<TValue>
    {
        IDictionary<IXamlNode, TValue> Values { get; } 
    }
}

