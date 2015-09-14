using System;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.VisualBasic;
using System.Collections.Generic;


namespace Eto.Designer.Builders
{
	public class VbInterfaceBuilder : CodeInterfaceBuilder
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

