// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace CommonXaml;

public static class ContinuationExtensions
{
    public static (bool success, IXamlNode root) Transform<TConfig>(this (bool success, IXamlNode node) continuation, IXamlTransform<TConfig> transform) where TConfig: IXamlTransformConfiguration
    {
        if (continuation.success || transform.Config.ContinueOnError)
            continuation.success &= continuation.node.Accept(transform);
        return (continuation.success, continuation.node);
    }

    public static (bool success, IXamlNode root) Validate<TConfig>(this (bool success, IXamlNode root) continuation, IXamlValidator<TConfig> validator) where TConfig: IXamlNodeVisitorConfiguration
        => continuation.Visit(validator);

    public static (bool success, IXamlNode root) Visit<TConfig>(this (bool success, IXamlNode node) continuation, IXamlNodeVisitor<TConfig> visitor) where TConfig : IXamlNodeVisitorConfiguration
    {
        if (continuation.success || visitor.Config.ContinueOnError)
            continuation.success &= continuation.node.Accept(visitor);
        return (continuation.success, continuation.node);
    }

    public static (bool success, IXamlNode node) AsReadOnly(this (bool success, IXamlNode node) continuation)
        => continuation.node switch
        {
            XamlLiteral literal => (continuation.success, literal.AsReadOnly()),
            XamlElement element => (continuation.success, element.AsReadOnly()),
            _ => continuation,
        };
}