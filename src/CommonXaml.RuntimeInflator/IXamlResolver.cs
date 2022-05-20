using System;
using Microsoft.Extensions.Logging;

namespace CommonXaml.RuntimeInflator;

public interface IXamlTypeResolver
{
    bool TryResolve(XamlType xamlType, ILogger? logger, out Type? type);        
}
