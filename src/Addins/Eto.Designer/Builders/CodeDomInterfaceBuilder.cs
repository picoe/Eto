using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Eto.Forms;
using Eto.Drawing;
using Eto.Designer;

namespace Eto.Designer.Builders
{
	public abstract class CodeDomInterfaceBuilder : BaseCompiledInterfaceBuilder
	{
		protected abstract CodeDomProvider CreateCodeProvider();

		protected virtual void SetParameters(CompilerParameters parameters)
		{
			string referenceDir = GetReferenceAssembliesFolder();

			if (string.IsNullOrEmpty(referenceDir))
				throw new InvalidOperationException("Cannot find reference assemblies folder");

			if (EtoEnvironment.Platform.IsMono)
				parameters.ReferencedAssemblies.Add(Path.Combine(referenceDir, "mscorlib.dll"));

			parameters.ReferencedAssemblies.AddRange(new[]
			{ 
				Path.Combine(referenceDir, "System.dll"),
				Path.Combine(referenceDir, "System.Core.dll"),
				typeof(Control).Assembly.Location
			});

			/* Needed for PCL version of Eto.dll
			 */
			var facadeDir = Path.Combine(referenceDir, "Facades");

			if (Directory.Exists(facadeDir))
			{
				foreach (var file in Directory.GetFiles(facadeDir, "*.dll"))
				{
					parameters.ReferencedAssemblies.Add(file);
				}
			}
		}

		static void FixMonoPath()
		{
			if (EtoEnvironment.Platform.IsMac)
			{
				// hack for OS X el capitan. mcs moved from /usr/bin to /usr/local/bin and is not on the path when XS is running
				// this should be removed when mono/XS is fixed.
				var path = Environment.GetEnvironmentVariable("PATH");
				var paths = path.Split(':');
				if (!paths.Contains("/usr/local/bin", StringComparer.Ordinal))
				{
					path += ":/usr/local/bin";
					Environment.SetEnvironmentVariable("PATH", path);
				}
			}
		}

		protected override CompileResult Compile(string outputFile, IEnumerable<string> references, string code, out Assembly generatedAssembly)
		{
			var inMemory = string.IsNullOrEmpty(outputFile);
			var parameters = new CompilerParameters
			{
				GenerateInMemory = inMemory,
				TreatWarningsAsErrors = false,
				GenerateExecutable = false,
				OutputAssembly = inMemory ? null : outputFile
			};

			FixMonoPath();

			SetParameters(parameters);

			if (references != null)
			{
				foreach (var reference in references)
				{
					if (File.Exists(reference))
						parameters.ReferencedAssemblies.Add(reference);
				}
			}

			var codeProvider = CreateCodeProvider();
			var results = codeProvider.CompileAssemblyFromSource(parameters, code);

			var errors = results.Errors.Cast<CompilerError>().ToList();
			var result = new CompileResult { Errors = errors.Select(r => r.ErrorText).ToList() };
			if (errors.Count == 0 || errors.All(r => r.IsWarning))
			{
				if (inMemory)
					generatedAssembly = results.CompiledAssembly;
				else
					generatedAssembly = null;
				result.Success = true;
			}
			else
			{
				generatedAssembly = null;
				errors.ForEach(msg => Debug.WriteLine(msg.ToString()));
			}
			return result;
		}

	}
	
}
