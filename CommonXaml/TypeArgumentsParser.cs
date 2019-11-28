// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

using static CommonXaml.XamlExceptionCode;

namespace CommonXaml
{
	public static class TypeArgumentsParser
	{
		public static bool TryParseTypeArguments(string expression, IXamlNamespaceResolver resolver, IXamlSourceInfo sourceInfo, out IList<XamlType> types, out IList<Exception> exceptions)
		{
			types = new List<XamlType>();
			exceptions = null;

			while (!string.IsNullOrWhiteSpace(expression)) {
				var match = expression;
				if (Parse(match, ref expression, resolver, sourceInfo, out var type, out var parseexceptions))
					types.Add(type);
				else
					((List<Exception>)(exceptions ??= new List<Exception>())).AddRange(parseexceptions);
			}

			return exceptions == null;
		}

		static bool Parse(string match, ref string remaining, IXamlNamespaceResolver resolver, IXamlSourceInfo sourceInfo, out XamlType xamltype, out IList<Exception> exceptions)
		{
			exceptions = null;
			remaining = null;
			int parensCount = 0;
			bool isGeneric = false;

			int pos;
			for (pos = 0; pos < match.Length; pos++) {
				if (match[pos] == '(') {
					parensCount++;
					isGeneric = true;
				}
				else if (match[pos] == ')')
					parensCount--;
				else if (match[pos] == ',' && parensCount == 0) {
					remaining = match.Substring(pos + 1);
					break;
				}
			}
			var type = match.Substring(0, pos).Trim();

			IList<XamlType> typeArguments = null;
			if (isGeneric) {
				if (!TryParseTypeArguments(type.Substring(type.IndexOf('(') + 1, type.LastIndexOf(')') - type.IndexOf('(') - 1), resolver, sourceInfo, out typeArguments, out var parseexceptions))
					((List<Exception>)(exceptions ??= new List<Exception>())).AddRange(parseexceptions);
				type = type.Substring(0, type.IndexOf('('));
			}

			var split = type.Split(new[] { ':' }, 2);

			string prefix, name;
			if (split.Length == 2) {
				prefix = split [0];
				name = split [1];
			} else {
				prefix = "";
				name = split [0];
			}

			var namespaceuri = resolver.LookupNamespace(prefix);
			if (namespaceuri == null)
				(exceptions ??= new List<Exception>()).Add(new XamlParseException(CXAML1012, new[] { prefix }, sourceInfo));

			xamltype = new XamlType(namespaceuri, name, typeArguments as List<XamlType>);
			return exceptions == null;
		}
	}
}