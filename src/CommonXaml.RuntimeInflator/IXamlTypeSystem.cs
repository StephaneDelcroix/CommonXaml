using System;

namespace CommonXaml.RuntimeInflator
{
    public interface IXamlTypeSystem
    {
        Type Resolve(XamlType xamlType);
    }
}
