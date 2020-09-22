// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace CommonXaml
{
	[Serializable]
	public class XamlException : Exception
	{
		public XamlException()
		{
		}

		public XamlException(string message) : base(message)
		{
		}

		public XamlException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected XamlException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}

		public XamlException(XamlExceptionCode code, string[] messageArgs, IXamlSourceInfo sourceInfo, Exception innerExcetion = null)
			: base(string.Format(code.ErrorMessage, messageArgs ?? new string[0]), innerExcetion)
		{
			XamlSourceInfo = sourceInfo;
			ExceptionCode = code;
		}

		public IXamlSourceInfo XamlSourceInfo { get; }
		public XamlExceptionCode ExceptionCode { get; }
	}
}