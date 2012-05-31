using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IGroupBox : IContainer
	{
		Font Font { get; set; }

		string Text { get; set; }
	}
	
	public class GroupBox : Container
	{
		IGroupBox handler;
		
		public GroupBox () : this(Generator.Current)
		{
		}
		
		public GroupBox (Generator g) : this (g, typeof(IGroupBox))
		{
		}
		
		protected GroupBox (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (IGroupBox)base.Handler;
		}

		public Font Font {
			get { return handler.Font; }
			set { handler.Font = value; }
		}
		
		public string Text {
			get { return handler.Text; }
			set { handler.Text = value; }
		}

	}
}
