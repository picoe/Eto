using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IToolBarItem : IInstanceWidget
	{
	}
	
	public class ToolBarItem : InstanceWidget
	{
		IToolBarItem inner;
		
		public ToolBarItem(Generator g, Type type) : base(g, type)
		{
			inner = (IToolBarItem)Handler;
		}
	}
}
