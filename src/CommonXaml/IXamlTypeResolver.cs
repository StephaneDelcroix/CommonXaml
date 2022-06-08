// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;

namespace CommonXaml;

public interface IXamlTypeResolver<TType>
{
    bool TryResolve(XamlType xamlType, ILogger? logger, out TType? type);
}
