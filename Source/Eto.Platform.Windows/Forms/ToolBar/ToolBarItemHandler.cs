using System;
using System.Reflection;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows
{
	public interface IToolBarItemHandler
	{
		void CreateControl(ToolBarHandler handler);
		
	}
	
	public abstract class ToolBarItemHandler<T, W> : WidgetHandler<T, W>, IToolBarItem, IToolBarItemHandler
		where T: SWF.ToolStripItem
		where W: ToolBarItem
	{
		Icon icon;
		
		public string ID { get; set; }
		
		public abstract void CreateControl(ToolBarHandler handler);

		public virtual void InvokeButton()
		{
		}
		
		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}
		
		public string ToolTip
		{
			get { return Control.ToolTipText; }
			set { Control.ToolTipText = value; }
		}
		
		
		public Icon Icon
		{
			get { return icon; }
			set
			{
				this.icon = value;
				if (icon != null) Control.Image = ((IconHandler)icon.Handler).GetIconClosestToSize(16).ToBitmap();
				else Control.Image = null;
			}
		}
		
		public abstract bool Enabled { get; set; }

	}


}
