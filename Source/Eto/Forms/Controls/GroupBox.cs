using System;
using Eto.Drawing;

namespace Eto.Forms
{
	[Handler(typeof(GroupBox.IHandler))]
	public class GroupBox : Panel
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }
		
		public GroupBox()
		{
		}

		[Obsolete("Use default constructor instead")]
		public GroupBox (Generator generator) : this (generator, typeof(IHandler))
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

		public new interface IHandler : Panel.IHandler
		{
			Font Font { get; set; }

			string Text { get; set; }
		}
	}
}
