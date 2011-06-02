using System;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IImageMenuItem : IMenuActionItem
	{
		Icon Icon { get; set; }
	}
	
	public class ImageMenuItem : MenuActionItem
	{
		IImageMenuItem inner;

		public ImageMenuItem(Generator g) : base(g, typeof(IImageMenuItem))
		{
			//BindingContext = new BindingContext();
			inner = (IImageMenuItem)base.Handler;
		}


		public Icon Icon
		{
			get { return inner.Icon; }
			set { inner.Icon = value; }
		}
		
	}
}
