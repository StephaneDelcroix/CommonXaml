// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace CommonXaml
{
	public struct XamlExceptionCode
	{
		//x2006 and x2009 validations
		public static XamlExceptionCode CXAML1000 = new XamlExceptionCode(nameof(CXAML1000), "Unknown property '{0}' in xmlns '{1}'.", "");
		public static XamlExceptionCode CXAML1001 = new XamlExceptionCode(nameof(CXAML1001), "String literal expected for property '{0}'.", "");
		public static XamlExceptionCode CXAML1002 = new XamlExceptionCode(nameof(CXAML1002), "Unsupported Xaml version.", "");

		//xaml errors
		public static XamlExceptionCode CXAML1010 = new XamlExceptionCode(nameof(CXAML1010), "Duplicate property name '{0}'.", "");
		public static XamlExceptionCode CXAML1011 = new XamlExceptionCode(nameof(CXAML1011), "Unexpected empty element '<{0} />'.", "");
		public static XamlExceptionCode CXAML1012 = new XamlExceptionCode(nameof(CXAML1012), "No xmlns declaration for prefix '{0}'.", "");


		public string ErrorCode { get; }
		public string ErrorMessage { get; }
		public string ErrorLink { get; }

		XamlExceptionCode(string errorCode, string errorMessage, string errorLink)
		{
			ErrorCode = errorCode;
			ErrorMessage = errorMessage;
			ErrorLink = errorLink;
		}
	}
}