using System;
using MonoDevelop.Ide.Templates;
using System.Linq;
using System.Xml;
using MonoDevelop.Core.StringParsing;
using System.Collections.Generic;
using MonoDevelop.Projects;

namespace Eto.Addin.XamarinStudio
{
	public class ParameterizedFile : FileDescriptionTemplate
	{
		readonly ReplacementFile inner;

		// why? because TextFileDescriptionTemplate doesn't give us a way to access the generated ProjectFile
		class ReplacementFile : TextFileDescriptionTemplate
		{
			public ParameterizedFile Outer { get; set; }

			IStringTagModel tagModel;

			protected override string ProcessContent(string content, IStringTagModel tags)
			{
				tags = new TagModel { Inner = tags, File = Outer };
				tagModel = tags;
				return base.ProcessContent(content, tags);
			}

			public string ProcessContent(string content)
			{
				if (tagModel == null)
					throw new InvalidOperationException("Cannot process content with no tag model defined. Only use this after processing file content");
				return base.ProcessContent(content, tagModel);
			}
		}

		public ParameterizedFile()
		{
			inner = new ReplacementFile { Outer = this };
		}

		public Dictionary<string, Replacement> Replacements { get; set; }

		public string ResourceId { get; set; }

		public class Replacement
		{
			public string Name { get; set; }

			public string Value { get; set; }

			public Replacement(XmlElement element)
			{
				Name = element.GetAttribute("name");
				Value = element.GetAttribute("value");
			}
		}

		public override void Load(System.Xml.XmlElement filenode, MonoDevelop.Core.FilePath baseDirectory)
		{
			ResourceId = filenode.GetAttribute("ResourceId");
			var replacements = filenode.SelectNodes("Replacements/Replacement");
			Replacements = replacements.OfType<XmlElement>().Select(r => new Replacement(r)).ToDictionary(r => r.Name, StringComparer.OrdinalIgnoreCase);
			inner.Load(filenode, baseDirectory);
		}

		class TagModel : IStringTagModel
		{
			public IStringTagModel Inner { get; set; }

			public ParameterizedFile File { get; set; }

			public object GetValue(string name)
			{
				Replacement parameter;
				if (File.Replacements.TryGetValue(name, out parameter))
					return parameter.Value;
				return Inner.GetValue(name);
			}
		}

		public override bool SupportsProject(Project project, string projectPath)
		{
			return inner.SupportsProject(project, projectPath);
		}

		public override bool IsValidName(string name, string language)
		{
			return inner.IsValidName(name, language);
		}

		public override bool AddToProject(SolutionItem policyParent, Project project, string language, string directory, string name)
		{
			var file = inner.AddFileToProject(policyParent, project, language, directory, name);
			if (file == null)
				return false;

			if (!string.IsNullOrEmpty(ResourceId))
				file.ResourceId = inner.ProcessContent(ResourceId);

			/* TODO: Add required packages here?
			var dnproject = project as DotNetProject;
			if (dnproject != null)
			{
			}
			*/

			return true;
		}

		public override void Show()
		{
			inner.Show();
		}

		public override string Name
		{
			get { return inner.Name; }
		}
	}
}

