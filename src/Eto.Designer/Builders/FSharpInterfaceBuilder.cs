using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;
using System.Collections.Generic;
using FSharp.Compiler.CodeDom;

namespace Eto.Designer.Builders
{
	public class FSharpInterfaceBuilder : CodeDomInterfaceBuilder
	{
		protected override CodeDomProvider CreateCodeProvider()
		{
			return new FSharpCodeProvider();
		}

		const string MacLib45Dir = "/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5";

		public static bool ExistsOnPath(string fileName)
		{
			if (GetFullPath(fileName) != null)
				return true;
			return false;
		}

		public static string GetFullPath(string fileName)
		{
			if (File.Exists(fileName))
				return Path.GetFullPath(fileName);

			var values = Environment.GetEnvironmentVariable("PATH");
			foreach (var path in values.Split(';'))
			{
				var fullPath = Path.Combine(path, fileName);
				if (File.Exists(fullPath))
					return fullPath;
			}
			return null;
		}

		protected override void SetParameters(CompilerParameters parameters)
		{
			base.SetParameters(parameters);

			if (EtoEnvironment.Platform.IsMac && EtoEnvironment.Platform.IsMono && !ExistsOnPath("fsc.exe"))
			{
				var path = Environment.GetEnvironmentVariable("PATH");
				path += ":" + MacLib45Dir;
				Environment.SetEnvironmentVariable("PATH", path);
			}
		}
	}
}

