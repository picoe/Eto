#if VB
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Designer.Builders
{
	public class RoslynVbInterfaceBuilder : RoslynInterfaceBuilder
	{
		protected override SyntaxTree ParseText(string code)
		{
			return VisualBasicSyntaxTree.ParseText(code);
		}

		protected override Compilation CreateCompilation(SyntaxTree syntaxTree, string assemblyName, IEnumerable<PortableExecutableReference> references)
		{
			return VisualBasicCompilation.Create(
				assemblyName,
				syntaxTrees: new[] { syntaxTree },
				references: references,
				options: new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
		}
	}
}
#endif