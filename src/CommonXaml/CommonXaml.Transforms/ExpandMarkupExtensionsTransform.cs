// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using static CommonXaml.XamlExceptionCode;

namespace CommonXaml.Transforms
{
	public class ExpandMarkupExtensionsTransform : IXamlTransform
	{
		public IXamlTransform.TreeVisitingMode VisitingMode => IXamlTransform.TreeVisitingMode.TopDown;
		IList<Exception>? transformExceptions;
		public IList<Exception>? Errors { get => transformExceptions; }
		public bool ShouldSkipChildren(IXamlNode node) => false;

		public void Transform(XamlLiteral node)
		{
			if (node.Literal.StartsWith("{}", StringComparison.Ordinal)) {
				node.Literal = node.Literal.Substring(2);
				return;
			}
			else if (!node.Literal.StartsWith("{", StringComparison.Ordinal))
				return;

			var markup = node.Literal;
			if (ExpandMarkup(ref markup, node) is IXamlNode markupNode)
				node.Parent?.ReplaceNode(node, markupNode);
		}

		public void Transform(XamlElement node)
		{
		}

		IXamlNode? ExpandMarkup(ref string expression, IXamlNode originalNode)
		{
			if (expression.StartsWith("{}", StringComparison.Ordinal))
				return new XamlLiteral(expression.Substring(2), originalNode.NamespaceResolver, originalNode.SourceUri, originalNode.LineNumber, originalNode.LinePosition);
			if (expression[expression.Length - 1] != '}')
				AppendExceptions(ref transformExceptions, new XamlParseException(CXAML1020, new string[0], originalNode, null));

			var matching = MatchMarkup(out var match, expression, out var len);
			expression = expression.Substring(len).TrimStart();

			if (expression.Length == 0)
				AppendExceptions(ref transformExceptions, new XamlParseException(CXAML1020, new string[0], originalNode, null));

			if (matching)
				return Parse(match!, originalNode, ref expression);
			return null;
		}

		XamlElement? Parse(string match, IXamlNode originalNode, ref string remainder)
		{
			if (!XamlType.TryParse(match, originalNode.NamespaceResolver, originalNode, out var xamlType, out var exceptions)) {
				AppendExceptions(ref transformExceptions, exceptions);
				return null;
			}

			var element = new XamlElement(xamlType, originalNode.NamespaceResolver, originalNode.SourceUri, originalNode.LineNumber, originalNode.LinePosition);

			if (remainder.StartsWith("}", StringComparison.Ordinal))
				return element; //empty

			while (GetNextPiece(ref remainder, out var next) is string piece) {
				if (next != '=') { //implicit content property
					element.AddOrAppend(XamlPropertyName.ImplicitProperty, new XamlLiteral(piece, originalNode.NamespaceResolver, originalNode.SourceUri, originalNode.LineNumber, originalNode.LinePosition));
					continue;
				}

				remainder = remainder.TrimStart();
				IXamlNode? value;
				if (remainder.StartsWith("{", StringComparison.Ordinal)) {
					value = ExpandMarkup(ref remainder, originalNode);
					remainder = remainder.TrimStart();

					if (remainder.Length > 0 && remainder[0] == ',')
						remainder = remainder.Substring(1);
					else if (remainder.Length > 0 && remainder[0] == '}')
						remainder = remainder.Substring(1);
				} else
					value = (GetNextPiece(ref remainder, out _) is string literal)
						? new XamlLiteral(literal, originalNode.NamespaceResolver, originalNode.SourceUri, originalNode.LineNumber, originalNode.LinePosition)
						: null;

				if (value != null)
					element.TryAdd(new XamlPropertyName("", piece), new List<IXamlNode> { value});
			}
			return element;
		}

		static bool MatchMarkup(out string? match, string expression, out int end)
		{
			if (expression.Length < 2) {
				end = 1;
				match = null;
				return false;
			}

			if (expression[0] != '{') {
				end = 2;
				match = null;
				return false;
			}

			int i;
			bool found = false;
			for (i = 1; i < expression.Length; i++) {
				if (expression[i] == ' ')
					continue;
				found = true;
				break;
			}

			if (!found) {
				end = 3;
				match = null;
				return false;
			}

			int c;
			for (c = 0; c + i < expression.Length; c++) {
				if (expression[i + c] == ' ' || expression[i + c] == '}')
					break;
			}

			if (i + c == expression.Length) {
				end = 6;
				match = null;
				return false;
			}

			end = i + c;
			match = expression.Substring(i, c);
			return true;
		}

		static string? GetNextPiece(ref string remainder, out char next)
		{
			bool inString = false;
			int end = 0;
			char stringTerminator = '\0';
			remainder = remainder.TrimStart();
			if (remainder.Length == 0) {
				next = Char.MaxValue;
				return null;
			}

			var piece = new StringBuilder();
			// If we're inside a quoted string we append all chars to our piece until we hit the ending quote.
			while (end < remainder.Length &&
				   (inString || (remainder[end] != '}' && remainder[end] != ',' && remainder[end] != '='))) {
				if (inString) {
					if (remainder[end] == stringTerminator) {
						inString = false;
						end++;
						break;
					}
				}
				else {
					if (remainder[end] == '\'' || remainder[end] == '"') {
						inString = true;
						stringTerminator = remainder[end];
						end++;
						continue;
					}
				}

				// If this is an escape char, consume it and append the next char to our piece.
				if (remainder[end] == '\\') {
					end++;
					if (end == remainder.Length)
						break;
				}
				piece.Append(remainder[end]);
				end++;
			}

			if (inString && end == remainder.Length)
				throw new Exception("Unterminated quoted string");

			if (end == remainder.Length && !remainder.EndsWith("}", StringComparison.Ordinal))
				throw new Exception("Expression did not end with '}'");

			if (end == 0) {
				next = Char.MaxValue;
				return null;
			}

			next = remainder[end];
			remainder = remainder.Substring(end + 1);

			// Whitespace is trimmed from the end of the piece before stripping
			// quote chars from the start/end of the string. 
			while (piece.Length > 0 && char.IsWhiteSpace(piece[piece.Length - 1]))
				piece.Length--;

			if (piece.Length >= 2) {
				char first = piece[0];
				char last = piece[piece.Length - 1];
				if ((first == '\'' && last == '\'') || (first == '"' && last == '"')) {
					piece.Remove(piece.Length - 1, 1);
					piece.Remove(0, 1);
				}
			}

			return piece.ToString();
		}

		static void AppendExceptions(ref IList<Exception>? exceptions, Exception additionalException)
		{
			if (additionalException == null)
				return;
			AppendExceptions(ref exceptions, new List<Exception> { additionalException });
		}

		static void AppendExceptions(ref IList<Exception>? exceptions, IList<Exception>? additionalExceptions)
		{
			if (additionalExceptions == null)
				return;
			if (exceptions == null)
				exceptions = additionalExceptions;
			else
				foreach (var e in additionalExceptions)
					exceptions.Add(e);
		}
	}
}
