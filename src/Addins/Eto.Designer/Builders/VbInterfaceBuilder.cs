using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualBasic;

namespace Eto.Designer.Builders
{
	public class VbInterfaceBuilder : CodeDomInterfaceBuilder
	{
		protected override CodeDomProvider CreateCodeProvider()
		{
			var options = new Dictionary<string, string> { { "CompilerVersion", "v4.0" } };
			return new VBCodeProvider(options);
		}

		protected override void SetParameters(CompilerParameters parameters)
		{
			base.SetParameters(parameters);
			parameters.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");
		}
	}
}

