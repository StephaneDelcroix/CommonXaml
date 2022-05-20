// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace CommonXaml;

[Serializable]
public class XamlException : Exception
{
	public XamlException(string message, IXamlSourceInfo sourceInfo, Exception innerException) : base(message, innerException) => XamlSourceInfo = sourceInfo;

	public XamlException(XamlExceptionCode code, string[]? messageArgs, IXamlSourceInfo sourceInfo, Exception? innerException = null)
			: base(string.Format(code.ErrorMessage, messageArgs ?? new string[0]), innerException)
	{
		XamlSourceInfo = sourceInfo;
		ExceptionCode = code;
	}

	public IXamlSourceInfo XamlSourceInfo { get; }
	public XamlExceptionCode ExceptionCode { get; }
}