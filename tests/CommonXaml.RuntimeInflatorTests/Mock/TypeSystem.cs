﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.using CommonXaml.Transforms;

using System;
using CommonXaml.RuntimeInflator;

namespace CommonXaml.RuntimeInflatorTests
{
    public class MockTypeSystem : IXamlTypeResolver
    {
        public bool TryResolve(XamlType xamlType, Action<Exception> errorHandler, out Type? type)
        {
            type = Resolve(xamlType);
            return false;
        }

        Type? Resolve(XamlType xamlType) => xamlType switch {
            ("http://commonxaml/controls", "Control", _) => typeof(Control),
            ("http://commonxaml/controls", "View", _) => typeof(View),
            _ => null
        };
    }
}