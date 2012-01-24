using System;
using System.Reflection;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Eto.Drawing;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac
{
	interface IToolBarBaseItemHandler
	{
		string ID { get; }
		void CreateControl();
		void ControlAdded(ToolBarHandler toolbar);
	}
	
	interface IToolBarItemHandler : IToolBarBaseItemHandler
	{
		void OnClick();
		bool Enabled { get; }
	}
	
	class ToolBarItemHandlerTarget : NSObject
	{
		public IToolBarItemHandler Handler { get; set; }
		
		[Export("validateToolbarItem:")]
		public bool ValidateToolbarItem(NSToolbarItem item)
		{
			return Handler.Enabled;
		}
		
		[Export("action")]
		public bool action()
		{
			Handler.OnClick();
			return true;
		}
	}

	public abstract class ToolBarItemHandler<T, W> : WidgetHandler<T, W>, IToolBarItem, IToolBarItemHandler
		where T: NSToolbarItem
		where W: ToolBarItem
	{
		Icon icon;
		bool enabled = true;
		
		public ToolBarItemHandler()
		{
			this.ID = Guid.NewGuid().ToString();
		}
		
		public virtual void CreateControl()
		{
			if (Control != null) return;
			Control = (T)new NSToolbarItem(this.ID);
			Control.Target = new ToolBarItemHandlerTarget{ Handler = this };
			Control.Action = new Selector("action");
			Control.Autovalidates = false;
			Control.Enabled = enabled;
			Control.ToolTip = this.ToolTip ?? string.Empty;
			if (icon != null) Control.Image = (NSImage)icon.ControlObject;
			Control.Label = this.Text;
		}
		
		public virtual void ControlAdded(ToolBarHandler toolbar)
		{
		}
		
		public virtual void InvokeButton()
		{
		}
		
		public string Text { get; set; }
		public string ToolTip { get; set; }

		public Icon Icon
		{
			get { return icon; }
			set
			{
				this.icon = value;
			}
		}
		
		public virtual bool Enabled
		{
			get { return (Control != null) ? Control.Enabled : enabled; }
			set { if (Control != null) Control.Enabled = value; enabled = value; }
		}
		
		public void OnClick()
		{
			this.InvokeButton();
		}
	}


}
