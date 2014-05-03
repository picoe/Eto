using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IGroupBox : IPanel
	{
		Font Font { get; set; }

		string Text { get; set; }
	}

	[Handler(typeof(IGroupBox))]
	public class GroupBox : Panel
	{
		new IGroupBox Handler { get { return (IGroupBox)base.Handler; } }
		
		public GroupBox()
		{
		}

		[Obsolete("Use default constructor instead")]
		public GroupBox (Generator generator) : this (generator, typeof(IGroupBox))
		{
		}
		
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected GroupBox (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		public Font Font {
			get { return Handler.Font; }
			set { Handler.Font = value; }
		}
		
		public string Text {
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}

	}
}
