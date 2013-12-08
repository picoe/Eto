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
		
		public override string Text
		{
			get { return Handler.Text; }
			set
			{
				base.Text = value;  // Call the base setter. This parses the fields, if any.
				Handler.Text = MenuText; // retrieve the value from the parsed MenuText.
			}
		}

		public override string ToolTip
		{
			get { return Handler.ToolTip; }
			set { Handler.ToolTip = value; }
		}
		
		public override Image Image
		{
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}

		public override bool Enabled
		{
			get { return Handler.Enabled; }
			set 
			{
				base.Enabled = value;
				Handler.Enabled = value; 
			}
		}
	}
}

