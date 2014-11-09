using System;
using System.Collections;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms.Forms.Menu
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class RadioMenuItemHandler : MenuHandler<SWF.ToolStripMenuItem, RadioMenuItem, RadioMenuItem.ICallback>, RadioMenuItem.IHandler
	{
		ArrayList group;

		public RadioMenuItemHandler()
		{
			Control = new SWF.ToolStripMenuItem();
			Control.Click += control_Click;
		}

		void control_Click(object sender, EventArgs e)
		{
			Callback.OnClick(Widget, e);
		}

		public void Create(RadioMenuItem controller)
		{
			if (controller != null)
			{
				var controllerInner = (RadioMenuItemHandler)controller.Handler;
				if (controllerInner.group == null)
				{
					controllerInner.group = new ArrayList();
					controllerInner.group.Add(controller);
					controllerInner.Control.Click += controllerInner.control_RadioSwitch;
				}
				controllerInner.group.Add(Widget);
				Control.Click += controllerInner.control_RadioSwitch;
			}
		}
		#region IMenuItem Members

		public bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public string Text
		{
			get	{ return Control.Text; }
			set { Control.Text = value; }
		}
		
		public string ToolTip
		{
			get { return Control.ToolTipText; }
			set { Control.ToolTipText = value; }
		}

		public Keys Shortcut
		{
			get { return Control.ShortcutKeys.ToEto (); }
			set 
			{
				var key = value.ToSWF ();
				if (SWF.ToolStripManager.IsValidShortcut(key)) Control.ShortcutKeys = key;
			}
		}

		public bool Checked
		{
			get { return Control.Checked; }
			set { Control.Checked = value; }
		}

		#endregion

		void control_RadioSwitch(object sender, EventArgs e)
		{
			if (group != null)
			{
				foreach (RadioMenuItem item in group)
				{
					item.Checked = (item.ControlObject == sender);
				}
			}
		}
	}
}
