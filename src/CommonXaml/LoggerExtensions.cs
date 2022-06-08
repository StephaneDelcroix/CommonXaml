// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Microsoft.Extensions.Logging;

namespace CommonXaml;

public static class LoggerExtensions
{
    public static void LogXamlException(this ILogger? logger, string message, IXamlSourceInfo sourceInfo, Exception innerException)
    {
        var exception = new XamlException(message, sourceInfo, innerException);
        if (logger is not null)
            logger.LogError(exception, message);
        else throw exception;
    }

    public static void LogXamlException(this ILogger? logger, XamlExceptionCode code, string[]? messageArgs, IXamlSourceInfo sourceInfo, Exception? innerException = null)
    {
        var exception = new XamlException(code, messageArgs, sourceInfo, innerException);
        if (logger is not null)
            logger.LogError(exception, code.ErrorMessage, messageArgs ?? new string[0]);
        else throw exception;
    }

    public static void LogXamlParseException(this ILogger? logger, string message, IXamlSourceInfo sourceInfo, Exception innerException)
    {
        var exception = new XamlException(message, sourceInfo, innerException);
        if (logger is not null)
            logger.LogError(exception, message);
        else throw exception;
    }

    public static void LogXamlParseException(this ILogger? logger, XamlExceptionCode code, string[]? messageArgs, IXamlSourceInfo sourceInfo, Exception? innerException = null)
    {
        var exception = new XamlException(code, messageArgs, sourceInfo, innerException);
        if (logger is not null)
            logger.LogError(exception, code.ErrorMessage, messageArgs ?? new string[0]);
        else throw exception;
    }

    public static void LogException(this ILogger? logger, Exception exception)
    {
        if (logger is not null)
            logger.LogError(exception, exception.Message);
        else throw exception;
    }
}