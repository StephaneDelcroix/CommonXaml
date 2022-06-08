// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Microsoft.Extensions.Logging;

namespace CommonXaml.RuntimeInflator;

public static class XamlTypeResolverExtensions
{
    public static bool TryResolveX2009LanguagePrimitive(this XamlType xamlType, ILogger? logger, out Type? type)
    {
        //https://docs.microsoft.com/en-us/dotnet/desktop/xaml-services/types-for-primitives
        type = null;
        if (   xamlType.NamespaceUri != XamlPropertyIdentifier.Xaml2009Uri
            && xamlType.NamespaceUri != XamlPropertyIdentifier.Xaml2022Uri)
            return false;

        switch (xamlType.Name) {
        case nameof(SByte):
            type = typeof(SByte);
            return true;
        case nameof(Int16):
            type = typeof(Int16);
            return true;
        case nameof(Int32):
            type = typeof(Int32);
            return true;
        case nameof(Int64):
            type = typeof(Int64);
            return true;
        case nameof(Byte):
            type = typeof(Byte);
            return true;
        case nameof(UInt16):
            type = typeof(UInt16);
            return true;
        case nameof(UInt32):
            type = typeof(UInt32);
            return true;
        case nameof(UInt64):
            type = typeof(UInt64);
            return true;
        case nameof(Single):
            type = typeof(Single);
            return true;
        case nameof(Double):
            type = typeof(Double);
            return true;
        case nameof(Boolean):
            type = typeof(Boolean);
            return true;
        case nameof(TimeSpan):
            type = typeof(TimeSpan);
            return true;
        case nameof(Char):
            type = typeof(Char);
            return true;
        case nameof(String):
            type = typeof(String);
            return true;
        case nameof(Decimal):
            type = typeof(Decimal);
            return true;
        case nameof(Uri):
            type = typeof(Uri);
            return true;
        case nameof(Object):
            type = typeof(Object);
            return true;
        case nameof(Array):
            type = typeof(Array);
            return true;
        default:
            return false;
        }
    }
}