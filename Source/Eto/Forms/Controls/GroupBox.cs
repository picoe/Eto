using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IGroupBox : IDockContainer
	{
		Font Font { get; set; }

		string Text { get; set; }
	}
	
	public class GroupBox : DockContainer
	{
		new IGroupBox Handler { get { return (IGroupBox)base.Handler; } }
		
		public GroupBox () : this(Generator.Current)
		{
		}
		
		public GroupBox (Generator g) : this (g, typeof(IGroupBox))
		{
		}
		
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
