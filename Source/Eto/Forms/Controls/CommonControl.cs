using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface ICommonControl : IControl
	{
		Font Font { get; set; }
	}

	public abstract class CommonControl : Control
	{
		new ICommonControl Handler { get { return (ICommonControl)base.Handler; } }

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected CommonControl(Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		protected CommonControl()
		{
		}

		public Font Font
		{
			get { return Handler.Font; }
			set { Handler.Font = value; }
		}
	}
}

