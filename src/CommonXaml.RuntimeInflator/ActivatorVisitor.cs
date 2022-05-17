// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.using CommonXaml.Transforms;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace CommonXaml.RuntimeInflator
{
    public class ActivatorVisitor : IXamlNodeVisitor
    {
        public ActivatorVisitor(IRuntimeInflatorConfiguration config) => Configuration = config;
        public IXamlNodeVisitor.TreeVisitingMode VisitingMode => IXamlNodeVisitor.TreeVisitingMode.BottomUp;
        public IList<Exception>? Errors { get; private set; }
        public IRuntimeInflatorConfiguration Configuration { get; }
        public bool ShouldSkipChildren(IXamlNode node) => false;

        public Dictionary<IXamlNode, object> Values { get; } = new Dictionary<IXamlNode, object>();

        public void Visit(XamlLiteral node) => Values[node] = node.Literal;

        public void Visit(XamlElement node)
        {
            void addError(Exception exception) => (Errors ??= new List<Exception>()).Add(exception);

            if (!Configuration.Resolver.TryResolve(node.XamlType, addError, out var type))
                return;
            if (   !TryCreateFromX2009LanguagePrimitive(node, type!, addError, out object? value)
                && !TryCreateFromFactory(node, type!, addError, out value)
                && !TryCreateFromParameterizedCtor(node, type!, addError, out value)
                && !TryCreateFromDefaultCtor(node, type!, addError, out value)) {
                addError(new Exception());
                return;
            }

            Values[node] = value!;

            //set Namescope, register source info, ...
            Configuration.OnActivatedCallback?.Invoke(node, value!);
        }

        static bool TryCreateFromX2009LanguagePrimitive(XamlElement node, Type type, Action<Exception> errorHandler, out object? value)
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

        bool TryCreateFromFactory(XamlElement node, Type type, Action<Exception> errorHandler, out object value) => throw new NotImplementedException();

        bool TryCreateFromParameterizedCtor(XamlElement node, Type type, Action<Exception> errorHandler, out object value) => throw new NotImplementedException();

        bool TryCreateFromDefaultCtor(XamlElement node, Type type, Action<Exception> errorHandler, out object? value) {
            try {
                value = Activator.CreateInstance(type);
                return true;
            } catch (MissingMemberException mme) {
                errorHandler(new XamlException(mme.Message, node as IXamlSourceInfo, mme));
            } catch (Exception e)  when (e.InnerException is XamlException || e.InnerException is XmlException) {
                errorHandler(e);
            }
            value = null;
            return false;   
        }
    }
}