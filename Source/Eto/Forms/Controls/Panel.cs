using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IPanel : IContainer
	{
	}
	
	public class Panel : Container
	{
		//IPanel inner;
		
		public Panel () : this(Generator.Current)
		{
		}
		
		public Panel (Generator g) : base(g, typeof(IPanel))
		{
			//inner = (IPanel)InnerControl;
		}
	}
}
