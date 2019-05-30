using System;
using MonoDevelop.Ide.Templates;
using System.Linq;
using System.Xml;
using MonoDevelop.Core.StringParsing;
using System.Collections.Generic;
using MonoDevelop.Projects;
using System.IO;
using System.Text;
using MonoDevelop.Ide.StandardHeader;
using MonoDevelop.Projects.Policies;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.Editor;
using md = MonoDevelop;

namespace Eto.Addin.MonoDevelop
{
	public class EnhancedFile : FileDescriptionTemplate
	{
		readonly ReplacementFile inner;

		// why? because TextFileDescriptionTemplate doesn't give us a way to access the generated ProjectFile
		class ReplacementFile : TextFileDescriptionTemplate
		{
			public EnhancedFile Outer { get; set; }

			IStringTagModel tagModel;

			TagModel GetTagModel (SolutionFolderItem policyParent, Project project, string language, string identifier, string fileName)
			{
				var model = new TagModel();
				var projectModel = ProjectTagModel ?? Outer.ProjectTagModel;
				if (projectModel != null)
					model.InnerModels = new [] { projectModel };
				ModifyTags (policyParent, project, language, identifier, fileName, ref model.OverrideTags);
				return model;
			}

			public override Stream CreateFileContent (SolutionFolderItem policyParent, Project project, string language, string fileName, string identifier)
			{
				if (Outer.FormatCode)
				{
					return base.CreateFileContent(policyParent, project, language, fileName, identifier);
				}

				var model = GetTagModel (policyParent, project, language, identifier, fileName);
				string text = CreateContent (project, model.OverrideTags, language);

				text = ProcessContent (text, model);
				var memoryStream = new MemoryStream ();
				byte[] preamble = Encoding.UTF8.GetPreamble ();
				memoryStream.Write (preamble, 0, preamble.Length);
				if (AddStandardHeader) {
					string header = StandardHeaderService.GetHeader (policyParent, fileName, true);
					byte[] bytes = Encoding.UTF8.GetBytes (header);
					memoryStream.Write (bytes, 0, bytes.Length);
				}

				var textDocument = TextEditorFactory.CreateNewDocument ();
				//var textDocument = new TextDocument ();
				textDocument.Text = text;
				var textStylePolicy = (policyParent == null) ? PolicyService.GetDefaultPolicy<TextStylePolicy> ("text/plain") : policyParent.Policies.Get<TextStylePolicy> ("text/plain");
				string eolMarker = TextStylePolicy.GetEolMarker (textStylePolicy.EolMarker);
				byte[] eol = Encoding.UTF8.GetBytes (eolMarker);
				string indent = (!textStylePolicy.TabsToSpaces) ? null : new string (' ', textStylePolicy.TabWidth);
				foreach (var current in textDocument.GetLines()) {
					string line = textDocument.GetTextAt (current.Offset, current.Length);
					if (indent != null) {
						line = line.Replace ("	", indent);
					}
					byte[] bytes = Encoding.UTF8.GetBytes (line);
					memoryStream.Write (bytes, 0, bytes.Length);
					memoryStream.Write (eol, 0, eol.Length);
				}
				memoryStream.Position = 0;
				return memoryStream;				
			}

			protected override string ProcessContent(string content, IStringTagModel tags)
			{
				tags = new TagModel { InnerModels = new [] { tags, Outer.ProjectTagModel }, File = Outer };
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

		public EnhancedFile()
		{
			inner = new ReplacementFile { Outer = this };
		}

		public Dictionary<string, Replacement> Replacements { get; set; }

		public string ResourceId { get; set; }

		public bool FormatCode { get; set; } = true;

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

		public override void Load(System.Xml.XmlElement filenode, md.Core.FilePath baseDirectory)
		{
			var formatCodeString = filenode.GetAttribute("FormatCode");
			if (!string.IsNullOrEmpty(formatCodeString))
				FormatCode = bool.Parse(formatCodeString);
			ResourceId = filenode.GetAttribute("ResourceId");
			var replacements = filenode.SelectNodes("Replacements/Replacement");
			Replacements = replacements.OfType<XmlElement>().Select(r => new Replacement(r)).ToDictionary(r => r.Name, StringComparer.OrdinalIgnoreCase);
			inner.Load(filenode, baseDirectory);
		}

		class TagModel : IStringTagModel
		{
			public IStringTagModel[] InnerModels { get; set; }

			public Dictionary<string, string> OverrideTags = new Dictionary<string, string> (StringComparer.OrdinalIgnoreCase);

			public EnhancedFile File { get; set; }

			public object GetValue(string name)
			{
				Replacement parameter;
				if (File != null && File.Replacements != null && File.Replacements.TryGetValue(name, out parameter))
					return parameter.Value;

				string overrideValue;
				if (OverrideTags != null && OverrideTags.TryGetValue(name, out overrideValue))
					return overrideValue;

				if (InnerModels != null)
				{
					foreach (var inner in InnerModels)
					{
						var value = inner.GetValue(name);
						if (value != null)
							return value;
					}
				}
				return null;
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

		public override bool AddToProject (SolutionFolderItem policyParent, Project project, string language, string directory, string name)
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

