using System;

namespace Eto.Forms
{
	public interface IToolBarItem : IInstanceWidget
	{
	}
	
	public abstract class ToolBarItem : CommandBase
	{
		public ToolBarItem (Generator g, Type type) : base(g, type)
		{
		}
	}
}
