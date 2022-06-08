//
// XamlTypeXamlExtensions.cs
//
// Author(s):
//       Stephane Delcroix <stephane@delcroix.org>
//
// Copyright (c) 2013 Mobile Inception
// Copyright (c) 2013-2014 Xamarin, Inc
// Copyright (c) 2022 Microsoft, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;

namespace CommonXaml;

static class XamlTypeExtensions
{
	public static T? GetTypeReference<T>(
		this XamlType xamlType,
		IEnumerable<(string namespaceUri, string clrNamespace, string assemblyName)> xmlnsMappings,
		Func<(string typeName, string clrNamespace, string? assemblyName, IReadOnlyList<T>? typeArguments), T> refFromTypeInfo)
		where T : class
	{
		var lookupAssemblies = new List<(string namespaceUri, string clrNamespace, string? assemblyName)>();
		(var namespaceUri, var name, var typeArguments) = xamlType;

		foreach (var def in xmlnsMappings) {
			if (def.namespaceUri != xamlType.NamespaceUri)
				continue;
			lookupAssemblies.Add(def);
		}

		if (lookupAssemblies.Count == 0) {
			var parsedXmlns = ParseXmlns(namespaceUri);
			if (parsedXmlns != null)
				lookupAssemblies.Add((namespaceUri, parsedXmlns.Value.clrNamespace, parsedXmlns.Value.assemblyName));
		}

		List<T>? typeArgs = null;
		if (typeArguments != null) {
			typeArgs = new();
			for (var i = 0; i < typeArguments.Count; i++)
				typeArgs[i] = typeArguments[i].GetTypeReference(xmlnsMappings, refFromTypeInfo)!;
		}

        foreach (var potentialAssembly in lookupAssemblies) {
            T? type;
            if ((type = refFromTypeInfo(($"{name}Extension)", potentialAssembly.clrNamespace, potentialAssembly.assemblyName, typeArgs?.AsReadOnly()))) != null)
                return type;
            if ((type = refFromTypeInfo((name, potentialAssembly.clrNamespace, potentialAssembly.assemblyName, typeArgs?.AsReadOnly()))) != null)
                return type;
        }

        return null;
	}

	static (string clrNamespace, string? assemblyName)? ParseXmlns(string xmlns)
	{
		var parts = xmlns.Split(';');
		if (xmlns.StartsWith("clr-namespace:", StringComparison.Ordinal)) {
			var clrNamespace = parts[0].Substring(14);
			for (var i = 1; i < parts.Length; i++) {
				if (!parts[i].StartsWith("assembly=", StringComparison.Ordinal))
					continue;
				return (clrNamespace, parts[i].Substring(9));
			}
			return (clrNamespace, null);
		} else if (xmlns.StartsWith("using:", StringComparison.Ordinal))
			return (parts[0].Substring(6), null);

		return null;
	}
}