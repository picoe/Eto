using System;

namespace Eto.Forms
{
	public interface IToolBarItem : IInstanceWidget
	{
	}
	
	public abstract class ToolBarItem : BaseAction
	{
		public ToolBarItem (Generator g, Type type) : base(g, type)
		{
		}
	}
}
