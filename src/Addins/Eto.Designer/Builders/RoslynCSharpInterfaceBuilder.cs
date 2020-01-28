#if RoslynCS
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Designer.Builders
{
	public class RoslynCSharpInterfaceBuilder : RoslynInterfaceBuilder
	{
		protected override SyntaxTree ParseText(string code)
		{
			return CSharpSyntaxTree.ParseText(code);
		}

		protected override Compilation CreateCompilation(SyntaxTree syntaxTree, string assemblyName, IEnumerable<PortableExecutableReference> references)
		{
			return CSharpCompilation.Create(
				assemblyName,
				syntaxTrees: new[] { syntaxTree },
				references: references,
				options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, 
					assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default
					)
				);
		}
	}
}
#endif
