#if !NETSTANDARD1_0
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Resources;

namespace Eto.Build.Tasks
{
	public class _GetCommonFiles : Task
	{
		public _GetCommonFiles() : base() { }
		public _GetCommonFiles(ResourceManager taskResources) : base(taskResources) { }
		public _GetCommonFiles(ResourceManager taskResources, string helpKeywordPrefix) : base(taskResources, helpKeywordPrefix) { }

		[Required]
		public ITaskItem[] Files { get; set; }

		[Required]
		public ITaskItem[] Names { get; set; }

		[Output]
		public ITaskItem[] Result { get; set; }

		public override bool Execute()
		{
			var result = new List<ITaskItem>();
			foreach (var fileItem in Files)
			{
				string name = fileItem.GetMetadata("Filename");
				foreach (var nameItem in Names)
				{
					if (string.Equals(name, nameItem.ItemSpec, StringComparison.OrdinalIgnoreCase))
					{
						result.Add(new TaskItem(fileItem.ItemSpec));
						break;
					}
				}
			}
			Result = result.ToArray();
			return true;
		}
	}
}
#endif // !NETSTANDARD1_0