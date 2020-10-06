using System;

namespace CommonXaml.RuntimeInflator
{
    public interface IXamlTypeResolver
    {
        bool TryResolve(XamlType xamlType, Action<Exception> errorHandler, out Type? type);        
    }
}
