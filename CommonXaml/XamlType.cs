// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics;

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

		public override int GetHashCode()
		{
			unchecked {
				return (NamespaceUri, Name, TypeArguments).GetHashCode();
			}
		}

		public static bool operator ==(XamlType x1, XamlType x2)
			=> x1.Equals(x2);
		public static bool operator !=(XamlType x1, XamlType x2)
			=> !(x1 == x2);
	}
}