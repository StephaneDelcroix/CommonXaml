// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using System.Globalization;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace CommonXaml.RuntimeInflator;

public class ActivatorVisitor : IXamlNodeVisitor<IRuntimeInflatorConfiguration, IActivatorContext>
{
    public ActivatorVisitor(IRuntimeInflatorConfiguration config, IActivatorContext context, object? root=null)
    {
        Config = config;
        Context = context;
        Root = root;
    }

    public TreeVisitingMode VisitingMode => TreeVisitingMode.BottomUp;
    public IRuntimeInflatorConfiguration Config { get; }
    public IActivatorContext Context { get; }
    object? Root { get; }

    public bool ShouldSkipChildren(IXamlNode node) => false;

    public bool Visit(IXamlLiteral node)
    {
        Context.Values[node] = node.Literal;
        return true;
    }

    public bool Visit(IXamlElement node)
    {
        //if the root object is provided, do not instanciate it
        if (node.Parent is null && Root is not null) {
            Context.Values[node] = Root;
            return true;
        }

        if (!Config.Resolver.TryResolve(node.XamlType, Config.Logger, out var type))
            return false;

        if (   !TryCreateFromX2009LanguagePrimitive(node, type!, Config.Logger, out object? value)
            && !TryCreateFromFactory(node, type!, Config.Logger, out value)
            && !TryCreateFromParameterizedCtor(node, type!, Config.Logger, out value)
            && !TryCreateFromDefaultCtor(node, type!, Config.Logger, out value)) {
            return false;
        }

        Context.Values[node] = value!;

        //set Namescope, register source info, ...
        Config.OnActivatedCallback?.Invoke(node, value!);
        return true;
    }

    static bool TryCreateFromX2009LanguagePrimitive(IXamlElement node, Type type, ILogger? logger, out object? value)
    {
        value = null;

        if (node.XamlType.NamespaceUri != XamlPropertyIdentifier.Xaml2009Uri)
            return false;

        if (!node.TryGetImplicitProperty(out var properties) || properties.Count == 0) {
            if (type == typeof(string))
                value = string.Empty;
            else if (type == typeof(Uri))
                value = default(Uri);
            else
                value = Activator.CreateInstance(type);
            return true;
        }

        if (properties[0] is not XamlLiteral literal || properties.Count > 1)
            return false;   //this has been already validated            

        if (type == typeof(sbyte) && sbyte.TryParse(literal.Literal, NumberStyles.Number, CultureInfo.InvariantCulture, out var sbyteval))
            value = sbyteval;
        else if (type == typeof(short) && short.TryParse(literal.Literal, NumberStyles.Number, CultureInfo.InvariantCulture, out var int16val))
            value = int16val;
        else if (type == typeof(int) && int.TryParse(literal.Literal, NumberStyles.Number, CultureInfo.InvariantCulture, out var int32val))
            value = int32val;
        else if (type == typeof(long) && long.TryParse(literal.Literal, NumberStyles.Number, CultureInfo.InvariantCulture, out var int64val))
            value = int64val;
        else if (type == typeof(byte) && byte.TryParse(literal.Literal, NumberStyles.Number, CultureInfo.InvariantCulture, out var byteval))
            value = byteval;
        else if (type == typeof(ushort) && ushort.TryParse(literal.Literal, NumberStyles.Number, CultureInfo.InvariantCulture, out var uint16val))
            value = uint16val;
        else if (type == typeof(uint) && uint.TryParse(literal.Literal, NumberStyles.Number, CultureInfo.InvariantCulture, out var uint32val))
            value = uint32val;
        else if (type == typeof(ulong) && ulong.TryParse(literal.Literal, NumberStyles.Number, CultureInfo.InvariantCulture, out var uint64val))
            value = uint64val;
        else if (type == typeof(float) && float.TryParse(literal.Literal, NumberStyles.Number, CultureInfo.InvariantCulture, out var singleval))
            value = singleval;
        else if (type == typeof(double) && double.TryParse(literal.Literal, NumberStyles.Number, CultureInfo.InvariantCulture, out var doubleval))
            value = doubleval;
        else if (type == typeof(bool) && bool.TryParse(literal.Literal, out var boolval))
            value = boolval;
        else if (type == typeof(TimeSpan) && TimeSpan.TryParse(literal.Literal, CultureInfo.InvariantCulture, out TimeSpan timespanval))
            value = timespanval;
        else if (type == typeof(char) && char.TryParse(literal.Literal, out var charval))
            value = charval;
        else if (type == typeof(string))
            value = literal.Literal;
        else if (type == typeof(decimal) && decimal.TryParse(literal.Literal, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalval))
            value = decimalval;
        else if (type == typeof(Uri) && Uri.TryCreate(literal.Literal, UriKind.RelativeOrAbsolute, out Uri urival))
            value = urival;

        return value != null;
    }

    bool TryCreateFromFactory(IXamlElement node, Type type, ILogger? logger, out object? value)
    {
        value = null;
        return false;
    }

    bool TryCreateFromParameterizedCtor(IXamlElement node, Type type, ILogger? logger, out object? value)
    {
        value = null;
        return false;
    }

    bool TryCreateFromDefaultCtor(IXamlElement node, Type type, ILogger? logger, out object? value) {
        try {
            value = Activator.CreateInstance(type);
            return true;
        } catch (MissingMemberException mme) {
            logger.LogXamlException(mme.Message, node as IXamlSourceInfo, mme);
        } catch (Exception e) when (e.InnerException is XamlException || e.InnerException is XmlException) {
            logger.LogException(e.InnerException);
        }
        value = null;
        return false;   
    }
}