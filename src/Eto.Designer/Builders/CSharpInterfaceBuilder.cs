using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Collections.Generic;
using Microsoft.CSharp;

namespace Eto.Designer.Builders
{
	public class CSharpInterfaceBuilder : CodeDomInterfaceBuilder
	{
		protected override CodeDomProvider CreateCodeProvider()
		{
			var options = new Dictionary<string, string> { { "CompilerVersion", "v4.0" } };
			return new CSharpCodeProvider(options);
		}
	}
}

