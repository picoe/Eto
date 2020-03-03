using Eto.Designer.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Eto.Designer
{
	public class BuilderInfo
	{
		public string Extension { get; set; }

		public string DesignExtension { get; set; }

		public Func<IInterfaceBuilder> CreateBuilder { get; set; }

		public Func<string, string> GetCodeFile { get; set; }
		public Func<string, string> GetDesignFile { get; set; }

		static IEnumerable<BuilderInfo> GetBuilders()
		{
			var csBuilder = new BuilderInfo
			{
				Extension = ".eto.cs",
				DesignExtension = "^.+(?<![jx]eto)[.]cs$",
				CreateBuilder = () => new CSharpInterfaceBuilder(),
				GetCodeFile = fileName => Regex.Replace(fileName, @"(.+)[.]eto([.]cs)", "$1$2"),
				GetDesignFile = fileName => Regex.Replace(fileName, @"(.+)([.]cs)", "$1.eto$2")
			};
			var vbBuilder = new BuilderInfo
			{
				Extension = ".eto.vb",
				DesignExtension = "^.+(?<![jx]eto)[.]vb$",
				CreateBuilder = () => new VbInterfaceBuilder(),
				GetCodeFile = fileName => Regex.Replace(fileName, @"(.+)[.]eto([.]vb)", "$1$2"),
				GetDesignFile = fileName => Regex.Replace(fileName, @"(.+)([.]vb)", "$1.eto$2")
			};

#if RoslynCS
			// use Roslyn on windows only, for now
			csBuilder.CreateBuilder = () => new RoslynCSharpInterfaceBuilder();
#endif
#if RoslynVB
			vbBuilder.CreateBuilder = () => new RoslynVbInterfaceBuilder();
#endif

			yield return csBuilder;
			yield return vbBuilder;

			yield return new BuilderInfo
			{
				Extension = ".eto.fs",
				DesignExtension = "^.+(?<![jx]eto)[.]fs$",
				CreateBuilder = () => new FSharpInterfaceBuilder(),
				GetCodeFile = fileName => Regex.Replace(fileName, @"(.+)[.]eto([.]fs)", "$1$2"),
				GetDesignFile = fileName => Regex.Replace(fileName, @"(.+)([.]fs)", "$1.eto$2")
			};
			yield return new BuilderInfo
			{
				Extension = ".xeto",
				DesignExtension = "^.+[.]xeto[.](cs|fs|vb)$",
				CreateBuilder = () => new XamlInterfaceBuilder(),
				GetCodeFile = fileName => fileName + ".cs",
				GetDesignFile = fileName => Regex.Replace(fileName, @"(.+[.]xeto)[.]cs", "$1")
			};
			yield return new BuilderInfo
			{
				Extension = ".jeto",
				DesignExtension = "^.+[.]jeto[.](cs|fs|vb)$",
				CreateBuilder = () => new JsonInterfaceBuilder(),
				GetCodeFile = fileName => fileName + ".cs",
				GetDesignFile = fileName => Regex.Replace(fileName, @"(.+[.]jeto)[.]cs", "$1")
			};
		}
		static List<BuilderInfo> builders => GetBuilders().ToList();

		public static bool Supports(string fileName)
		{
			return builders.Any(r => fileName.EndsWith(r.Extension, StringComparison.OrdinalIgnoreCase));
		}

		public static BuilderInfo Find(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;

			foreach (var item in builders)
			{
				if (fileName.EndsWith(item.Extension, StringComparison.OrdinalIgnoreCase))
					return item;
			}
			return null;
		}
		public static BuilderInfo FindCodeBehind(string fileName)
		{
			foreach (var item in builders)
			{
				if (Regex.IsMatch(fileName, item.DesignExtension, RegexOptions.IgnoreCase))
					return item;
			}
			return null;
		}

		public static bool IsCodeBehind(string fileName)
		{
			return builders.Any(r => Regex.IsMatch(fileName, r.DesignExtension, RegexOptions.IgnoreCase));
		}
	}
}
