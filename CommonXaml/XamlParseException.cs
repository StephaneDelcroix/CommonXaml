// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace CommonXaml
{
	[Serializable]
	public class XamlParseException : XamlException
	{
		public XamlParseException()
		{
		}

		public XamlParseException(string message) : base(message)
		{
		}

		public XamlParseException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected XamlParseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}

		public XamlParseException(XamlExceptionCode code, string[] messageArgs, IXamlSourceInfo sourceInfo, Exception innerExcetion = null)
			: base(code, messageArgs, sourceInfo, innerExcetion)
		{
		}

	}
}