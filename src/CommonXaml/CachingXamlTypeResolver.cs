// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace CommonXaml;

public class CachingXamlTypeResolver<TType> : IXamlTypeResolver<TType>
{
    public CachingXamlTypeResolver(IXamlTypeResolver<TType> xamlTypeResolver)
        => XamlTypeResolver = xamlTypeResolver;

    IXamlTypeResolver<TType> XamlTypeResolver { get; }
    Dictionary<XamlType, TType> Cache { get; } = new();

    public bool TryResolve(XamlType xamlType, ILogger? logger, out TType? type)
    {
        if (Cache.TryGetValue(xamlType, out type))
            return true;
        if (XamlTypeResolver.TryResolve(xamlType, logger, out type)) {
            Cache[xamlType] = type!;
            return true;
        }
        return false;
    }
}