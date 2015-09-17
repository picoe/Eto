using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;
using System.Collections.Generic;


namespace Eto.Designer.Builders
{
	public class CSharpInterfaceBuilder : CodeInterfaceBuilder
	{
		protected override CodeDomProvider CreateCodeProvider()
		{
			var options = new Dictionary<string, string> { { "CompilerVersion", "v4.0" } };
			return new CSharpCodeProvider(options);
		}
	}
}

