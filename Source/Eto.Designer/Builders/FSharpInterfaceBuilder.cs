using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;
using System.Collections.Generic;
using FSharp.Compiler.CodeDom;

namespace Eto.Designer.Builders
{
	public class FSharpInterfaceBuilder : CodeInterfaceBuilder
	{
		protected override CodeDomProvider CreateCodeProvider()
		{
			return new FSharpCodeProvider();
		}
	}
}

