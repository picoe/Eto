#if Roslyn
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Eto.Forms;
using Microsoft.CodeAnalysis;

namespace Eto.Designer.Builders
{
	public abstract class RoslynInterfaceBuilder : BaseCompiledInterfaceBuilder
	{
		protected virtual IEnumerable<string> GetReferences()
		{
			string referenceDir = GetReferenceAssembliesFolder();

			if (string.IsNullOrEmpty(referenceDir))
				throw new InvalidOperationException("Cannot find reference assemblies folder");

			yield return Path.Combine(referenceDir, "mscorlib.dll");
			yield return Path.Combine(referenceDir, "System.dll");
			yield return Path.Combine(referenceDir, "System.Core.dll");
			//yield return typeof(Control).Assembly.Location;
			//yield return typeof(System.Collections.Immutable.ImmutableList).Assembly.Location;

			/* Needed for PCL version of Eto.dll
			 */
			var facadeDir = Path.Combine(referenceDir, "Facades");

			if (Directory.Exists(facadeDir))
			{
				foreach (var file in Directory.GetFiles(facadeDir))
				{
					yield return file;
				}
			}
		}

		protected abstract SyntaxTree ParseText(string code);

		protected override CompileResult Compile(string outputFile, IEnumerable<string> references, string code, out Assembly generatedAssembly)
		{
			var syntaxTree = ParseText(code);

			string assemblyName = Path.GetRandomFileName();
			var refMetadata = GetReferences().Select(r => MetadataReference.CreateFromFile(r)).ToList();

			if (references != null)
			{
				foreach (var reference in references)
				{
					if (File.Exists(reference))
						refMetadata.Add(MetadataReference.CreateFromFile(reference));
				}
			}

			var compilation = CreateCompilation(syntaxTree, assemblyName, refMetadata);

			using (var ms = new MemoryStream())
			{
				var result = compilation.Emit(ms);

				if (!result.Success)
				{
					IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
						diagnostic.IsWarningAsError ||
						diagnostic.Severity == DiagnosticSeverity.Error);

					foreach (var diagnostic in failures)
					{
						Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
					}
					generatedAssembly = null;
					return new CompileResult { Success = false, Errors = failures.Select(r => r.GetMessage()).ToList() };
				}
				else
				{
					ms.Seek(0, SeekOrigin.Begin);
					generatedAssembly = Assembly.Load(ms.ToArray());
					return new CompileResult { Success = true };
				}
			}
		}

		protected abstract Compilation CreateCompilation(SyntaxTree syntaxTree, string assemblyName, IEnumerable<PortableExecutableReference> references);
	}
	
}
#endif