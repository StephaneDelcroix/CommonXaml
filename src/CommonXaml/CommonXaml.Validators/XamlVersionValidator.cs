// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using CommonXaml.Parser;
using static CommonXaml.XamlExceptionCode;

namespace CommonXaml.Validators
{
	public class XamlVersionValidator : IXamlValidator
	{
		public IXamlVersionValidationConfiguration Config { get; }
		public IList<Exception> ValidationErrors { get; private set; }
		public IXamlNodeVisitor.TreeVisitingMode VisitingMode => IXamlNodeVisitor.TreeVisitingMode.TopDown;
		public bool ShouldSkipChildren(IXamlNode node) => false;

		public XamlVersionValidator(IXamlVersionValidationConfiguration config) => Config = config;

		public void Visit(XamlLiteral node)
		{
		}

		public void Visit(XamlElement node)
		{
			foreach (var propertyName in node.Properties.Keys) {
				if (   propertyName.NamespaceUri == XamlPropertyName.Xaml2006Uri
					&& propertyName.LocalName != "Key"
					&& propertyName.LocalName != "Name"
					&& propertyName.LocalName != "Class"
					&& propertyName.LocalName != "FieldModifier")
					AddValidationError(new XamlParseException(CXAML1000, new[] { propertyName.LocalName, propertyName.NamespaceUri }, propertyName as IXamlSourceInfo));

				if (propertyName.NamespaceUri == XamlPropertyName.Xaml2009Uri) {
					if ((int)Config.MinSupportedXamlVersion < (int)XamlVersion.Xaml2009)
						AddValidationError(new XamlParseException(CXAML1002, null, propertyName as IXamlSourceInfo));
					else if (  propertyName.LocalName != "Key"
							&& propertyName.LocalName != "Name"
							&& propertyName.LocalName != "Class"
							&& propertyName.LocalName != "FieldModifier"
							&& propertyName.LocalName != "TypeArguments"
							&& propertyName.LocalName != "DataType"
							&& propertyName.LocalName != "FactoryMethod"
							&& propertyName.LocalName != "Arguments")
						AddValidationError(new XamlParseException(CXAML1000, new[] { propertyName.LocalName, propertyName.NamespaceUri }, propertyName as IXamlSourceInfo));
				}

				if (   (   propertyName.NamespaceUri == XamlPropertyName.Xaml2006Uri
						|| propertyName.NamespaceUri == XamlPropertyName.Xaml2009Uri)
					&& (   propertyName.LocalName == "Key"
						|| propertyName.LocalName == "Name"
						|| propertyName.LocalName == "Class"
						|| propertyName.LocalName == "FieldModifier"
						|| propertyName.LocalName == "TypeArguments"
						|| propertyName.LocalName == "DataType"
						|| propertyName.LocalName == "FactoryMethod")
					&& !(node.Properties[propertyName] is IList<IXamlNode> nodes && nodes.Count==1 && nodes[0] is XamlLiteral))
					AddValidationError(new XamlParseException(CXAML1001, new[] { propertyName.LocalName }, propertyName as IXamlSourceInfo));
			}
		}

		void AddValidationError(Exception exception) => (ValidationErrors ??= new List<Exception>()).Add(exception);
	}
}
