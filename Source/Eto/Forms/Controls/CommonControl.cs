using System;
using Eto.Drawing;

namespace Eto.Forms.Controls
{
	public interface ICommonControl : IControl 
	{
		Font Font { get; set; }
	}
	
	public abstract class CommonControl : Control
	{
		ICommonControl inner;
		
		protected CommonControl(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			this.inner = (ICommonControl)base.Handler;
		}
		
		public Font Font
		{
			get { return inner.Font; }
			set { inner.Font = value; }
		}
	}
}

