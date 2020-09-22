// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;

using static CommonXaml.XamlExceptionCode;
namespace CommonXaml
{
	[DebuggerDisplay("{NamespaceUri}:{Name}")]
	public readonly struct XamlType
	{
		public  static readonly XamlType Empty;

		public XamlType(string namespaceUri, string name, IReadOnlyList<XamlType> typeArguments = null)
		{
			NamespaceUri = namespaceUri;
			Name = name;
			TypeArguments = typeArguments;
		}

		public string NamespaceUri { get; }
		public string Name { get; }
		public IReadOnlyList<XamlType> TypeArguments { get; }

		public static bool TryParse(string type, IXamlNamespaceResolver resolver, IXamlSourceInfo sourceInfo, out XamlType xamlType, out IList<Exception> exceptions)
		{
			exceptions = null;

			var parts = type.Split(new[] { ':' }, 2);
			string prefix, name;
			if (parts.Length == 2) {
				prefix = parts[0];
				name = parts[1];
			}
			else {
				prefix = "";
				name = parts[0];
			}

			var namespaceuri = resolver.LookupNamespace(prefix);
			if (namespaceuri == null)
				(exceptions ??= new List<Exception>()).Add(new XamlParseException(CXAML1012, new[] { prefix }, sourceInfo));

			xamlType = new XamlType(namespaceuri, name, null);
			return exceptions == null;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (!(obj is XamlType other))
				return false;

			if (NamespaceUri != other.NamespaceUri)
				return false;
			if (Name != other.Name)
				return false;
			if (TypeArguments ==null && other.TypeArguments == null)
				return true;
			if (TypeArguments == null || other.TypeArguments == null)
				return false;
			if (TypeArguments.Count != other.TypeArguments.Count)
				return false;
			for (var i = 0; i < TypeArguments.Count; i++)
				if (!(TypeArguments[i].Equals(other.TypeArguments[i])))
					return false;
			return true;
		}

        public override int GetHashCode() => HashCode.Combine(NamespaceUri, Name, TypeArguments);
        public static bool operator ==(XamlType x1, XamlType x2) => x1.Equals(x2);
		public static bool operator !=(XamlType x1, XamlType x2) => !(x1 == x2);
	}
}