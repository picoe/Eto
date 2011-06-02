using System;

namespace Eto.Forms
{
	public interface IGroupBox : IContainer, ITextControl
	{
	}
	
	public class GroupBox : Container
	{
		IGroupBox inner;
		
		public GroupBox() : this(Generator.Current) {}
		
		public GroupBox(Generator g) : base(g, typeof(IGroupBox))
		{
			inner = (IGroupBox)base.Handler;
		}
		
		public string Text
		{
			get { return inner.Text; }
			set { inner.Text = value; }
		}

	}
}
