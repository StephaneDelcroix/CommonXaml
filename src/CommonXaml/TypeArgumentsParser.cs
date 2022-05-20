// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using static CommonXaml.XamlExceptionCode;

namespace CommonXaml;

public static class TypeArgumentsParser
{
	public static bool TryParseTypeArguments(string expression, IXamlNamespaceResolver resolver, IXamlSourceInfo sourceInfo, out IList<XamlType> types, ILogger? logger)
	{
		var success = true;

		types = new List<XamlType>();

		while (!string.IsNullOrWhiteSpace(expression)) {
			var match = expression;
			if (Parse(match, ref expression, resolver, sourceInfo, out var type, logger))
				types.Add(type);
			else
				success = false;
		}

		return success;
	}

	static bool Parse(string match, ref string remaining, IXamlNamespaceResolver resolver, IXamlSourceInfo sourceInfo, out XamlType xamltype, ILogger? logger)
	{
		xamltype = XamlType.Empty;
		remaining = string.Empty;
		var success = true;
		int parensCount = 0;
		bool isGeneric = false;

		int pos;
		for (pos = 0; pos < match.Length; pos++) {
			if (match[pos] == '(') {
				parensCount++;
				isGeneric = true;
			} else if (match[pos] == ')')
				parensCount--;
			else if (match[pos] == ',' && parensCount == 0) {
				remaining = match.Substring(pos + 1);
				break;
			}
		}
		var type = match.Substring(0, pos).Trim();

		IList<XamlType>? typeArguments = null;
		if (isGeneric) {
			if (!TryParseTypeArguments(type.Substring(type.IndexOf('(') + 1, type.LastIndexOf(')') - type.IndexOf('(') - 1), resolver, sourceInfo, out typeArguments, logger))
				success = false;
			type = type.Substring(0, type.IndexOf('('));
		}

		var parts = type.Split(new[] { ':' }, 2);

		string prefix, name;
		if (parts.Length == 2) {
			prefix = parts[0];
			name = parts[1];
		} else {
			prefix = "";
			name = parts[0];
		}

		var namespaceuri = resolver.LookupNamespace(prefix);
		if (namespaceuri == null) {
			logger.LogXamlParseException(CXAML1012, new[] { prefix }, sourceInfo);
			return false;
		}
		else
			xamltype = new XamlType(namespaceuri, name, typeArguments as List<XamlType>);
		return success;
	}
}