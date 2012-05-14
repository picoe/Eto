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
		IGroupBox inner;
		
		public GroupBox() : this(Generator.Current) {}
		
		public GroupBox(Generator g) : base(g, typeof(IGroupBox))
		{
			inner = (IGroupBox)base.Handler;
		}

		public Font Font
		{
			get { return inner.Font; }
			set { inner.Font = value; }
		}
		
		public string Text
		{
			get { return inner.Text; }
			set { inner.Text = value; }
		}

	}
}
