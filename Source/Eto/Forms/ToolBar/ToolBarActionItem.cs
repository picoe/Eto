using System;
using Eto.Drawing;

namespace Eto.Forms
{
	
	public interface IToolBarActionItem : IToolBarItem
	{
		string Text { get; set; }
		string ToolTip { get; set; }
		Image Image { get; set; }
		bool Enabled { get; set; }
	}
	
	public class ToolBarActionItem : ToolBarItem
	{
		new IToolBarActionItem Handler { get { return (IToolBarActionItem)base.Handler; } }

		public ToolBarActionItem()
			: this((Generator)null)
		{
		}

		public ToolBarActionItem(Generator generator)
			: this(generator, typeof(IToolBarActionItem))
		{
		}

		public ToolBarActionItem(Generator g, Type type)
			: base(g, type)
		{
		}
		
		public string Text
		{
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}

		public string ToolTip
		{
			get { return Handler.ToolTip; }
			set { Handler.ToolTip = value; }
		}
		
		public Image Image
		{
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}

		[Obsolete ("Use Image instead")]
		public Icon Icon
		{
			get { return Image as Icon; }
			set { Image = value; }
		}

		public bool Enabled
		{
			get { return Handler.Enabled; }
			set { Handler.Enabled = value; }
		}
	}
}

