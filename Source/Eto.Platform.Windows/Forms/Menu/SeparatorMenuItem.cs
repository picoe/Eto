using System;
using System.Collections;
using System.Reflection;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class SeparatorMenuItemHandler : MenuHandler<SWF.ToolStripSeparator, SeparatorMenuItem>, ISeparatorMenuItem, IMenu
	{
		
		public SeparatorMenuItemHandler()
		{
			Control = new SWF.ToolStripSeparator();
		}
		
		public bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
		
		public string Text
		{
			get { return string.Empty; }
			set { throw new NotSupportedException(); }
		}
		
		public Key Shortcut
		{
			get { return Key.None; }
			set { throw new NotSupportedException(); }
		}

		#region IMenu Members

		public void AddMenu(int index, MenuItem item)
		{
			throw new NotSupportedException();
		}

		public void RemoveMenu(MenuItem item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
