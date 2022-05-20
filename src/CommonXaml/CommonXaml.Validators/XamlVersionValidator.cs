// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

using static CommonXaml.XamlExceptionCode;

namespace CommonXaml.Validators;

public class XamlVersionValidator : IXamlValidator<IXamlVersionValidationConfiguration>
{
	public IXamlVersionValidationConfiguration Config { get; }
	public IList<Exception>? Errors { get; internal set; }
	public TreeVisitingMode VisitingMode => TreeVisitingMode.TopDown;
	public bool ShouldSkipChildren(IXamlNode node) => false;

	public XamlVersionValidator(IXamlVersionValidationConfiguration config)
			=> Config = config;

	public bool Visit(IXamlLiteral node) => true;

	public bool Visit(IXamlElement node)
	{
		var success = true;
		foreach (var propertyName in node.Properties.Keys) {
			if (   propertyName.NamespaceUri == XamlPropertyIdentifier.Xaml2006Uri
				&& propertyName.LocalName != "Key"
				&& propertyName.LocalName != "Name"
				&& propertyName.LocalName != "Class"
				&& propertyName.LocalName != "FieldModifier") {
				Config.Logger.LogXamlParseException(CXAML1000, new[] { propertyName.LocalName, propertyName.NamespaceUri }, (IXamlSourceInfo)propertyName);
				if (!Config.ContinueOnError)
					return false;
				success = false;
			}

			if (propertyName.NamespaceUri == XamlPropertyIdentifier.Xaml2009Uri) {
				if ((int)Config.MinSupportedXamlVersion < (int)XamlVersion.Xaml2009) {
					Config.Logger.LogXamlParseException(CXAML1002, null, (IXamlSourceInfo)propertyName);
					if (!Config.ContinueOnError)
						return false;
					success = false;
				} else if (propertyName.LocalName != "Key"
						&& propertyName.LocalName != "Name"
						&& propertyName.LocalName != "Class"
						&& propertyName.LocalName != "FieldModifier"
						&& propertyName.LocalName != "TypeArguments"
						&& propertyName.LocalName != "DataType"
						&& propertyName.LocalName != "FactoryMethod"
						&& propertyName.LocalName != "Arguments") {
					Config.Logger.LogXamlParseException(CXAML1000, new[] { propertyName.LocalName, propertyName.NamespaceUri }, (IXamlSourceInfo)propertyName);
					if (!Config.ContinueOnError)
						return false;
					success = false;
				}
			}

			if (   (   propertyName.NamespaceUri == XamlPropertyIdentifier.Xaml2006Uri
					|| propertyName.NamespaceUri == XamlPropertyIdentifier.Xaml2009Uri)
				&& (   propertyName.LocalName == "Key"
					|| propertyName.LocalName == "Name"
					|| propertyName.LocalName == "Class"
					|| propertyName.LocalName == "FieldModifier"
					|| propertyName.LocalName == "TypeArguments"
					|| propertyName.LocalName == "DataType"
					|| propertyName.LocalName == "FactoryMethod")
				&& !(   node.Properties[propertyName] is IList<IXamlNode> nodes
				     && nodes.Count == 1
					 && nodes[0] is XamlLiteral)) {
				Config.Logger.LogXamlParseException(CXAML1001, new[] { propertyName.LocalName }, (IXamlSourceInfo)propertyName);
				if (!Config.ContinueOnError)
					return false;
				success = false;
			}
		}
		return success;
	}
}
