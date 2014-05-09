using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public abstract class CommonControl : Control
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

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

		public interface IHandler : Control.IHandler
		{
			Font Font { get; set; }
		}
	}
}

