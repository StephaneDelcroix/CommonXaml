// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace CommonXaml
{
	public class XamlParserConfiguration
	{
		public Uri SourceUri { get; set; }
		public XamlVersion MinSupportedXamlVersion { get; set; } = XamlVersion.Xaml2006;
		public bool ContinueOnError { get; set; } = false;
		public IList<IXamlValidator> Validators { get; set; }
		public IList<IXamlTransform> Transforms { get; set; }
	}

	public enum XamlVersion
	{
		Xaml2006 = 0,
		Xaml2009
	}
}