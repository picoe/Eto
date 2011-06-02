using System;

namespace Eto.Forms
{
	public interface ITreeView : IControl
	{
	}

	/// <summary>
	/// Summary description for TreeView.
	/// </summary>
	public class TreeView : Control
	{
		//private ITreeView inner;

		public TreeView(Generator g) : base(g, typeof(ITreeView))
		{
			//inner = (ITreeView)InnerControl;
		}
	}
}
