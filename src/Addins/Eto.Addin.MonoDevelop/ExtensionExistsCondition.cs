using System;
using Mono.Addins;

namespace Eto.Addin.MonoDevelop
{
	/// <summary>
	/// Condition for the addin to ensure we don't add multiplat, multiplat/app or multiplat/library categories if they
	/// are already defined.
	/// </summary>
	public class ExtensionExistsCondition : ConditionType
	{
		static int isEvaluating;

		public override bool Evaluate(NodeElement conditionNode)
		{
			// don't allow recursion, getting the extension node can evaluate this condition again
			if (isEvaluating > 0)
				return false;
			
			isEvaluating++;
			try
			{
				var path = conditionNode.GetAttribute("path");
				var invertString = conditionNode.GetAttribute("invert");
				bool invert = string.Equals(invertString, "true", StringComparison.OrdinalIgnoreCase);

				// check if extension node is defined
				var extensionNode = AddinManager.GetExtensionNode(path);

				#if DEBUG
				Console.WriteLine("Extension '{0}' {1}", path, extensionNode != null ? "Exists" : "Does Not Exist");
				#endif
				return invert ? extensionNode == null : extensionNode != null;
			}
			finally
			{
				isEvaluating--;
			}
		}
	}
}