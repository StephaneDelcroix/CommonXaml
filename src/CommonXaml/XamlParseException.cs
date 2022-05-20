// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace CommonXaml;

[Serializable]
public class XamlParseException : XamlException
{
	public XamlParseException(string message, IXamlSourceInfo sourceInfo, Exception innerException) : base(message, sourceInfo, innerException)
	{
	}

	public XamlParseException(XamlExceptionCode code, string[]? messageArgs, IXamlSourceInfo sourceInfo, Exception? innerExcetion = null)
		: base(code, messageArgs, sourceInfo, innerExcetion)
	{
	}
}