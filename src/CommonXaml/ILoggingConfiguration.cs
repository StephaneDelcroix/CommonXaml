// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;

namespace CommonXaml;

public interface ILoggingConfiguration
{
    ILogger? Logger { get; }
    bool ContinueOnError { get; }
}