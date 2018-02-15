using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using System;

namespace Eto.WinForms.Forms.ToolBar
{
	public class ToolBarHandler : WidgetHandler<ToolStripEx, Eto.Forms.ToolBar>, Eto.Forms.ToolBar.IHandler
	{
		ToolBarDock dock = ToolBarDock.Top;

		public ToolBarHandler()
		{
			Control = new ToolStripEx();
			Control.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.StackWithOverflow;
			Control.AutoSize = true;
		}

		public ToolBarDock Dock
		{
			get { return dock; }
			set { dock = value; }
		}

		public void AddButton(ToolItem item, int index)
		{
			((IToolBarItemHandler)item.Handler).CreateControl(this, index);
		}

		public void RemoveButton(ToolItem item)
		{
			Control.Items.Remove((SWF.ToolStripItem)item.ControlObject);
		}

		public ToolBarTextAlign TextAlign
		{
			get
			{
				/*switch (control.TextAlign)
				{
					case SWF.ToolBarTextAlign.Right:
						return ToolBarTextAlign.Right;
					default:
					case SWF.ToolBarTextAlign.Underneath:
						return ToolBarTextAlign.Underneath;
				}
				 */
				return ToolBarTextAlign.Underneath;
			}
			set
			{
				switch (value)
				{
					case ToolBarTextAlign.Right:
						//control.TextAlign = SWF.ToolBarTextAlign.Right;
						break;
					case ToolBarTextAlign.Underneath:
						//control.TextAlign = SWF.ToolBarTextAlign.Underneath;
						break;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public void Clear()
		{
			Control.Items.Clear();
		}
	}

	/// <summary>
	/// This class adds on to the functionality provided in System.Windows.Forms.ToolStrip.
	/// <see cref="http://blogs.msdn.com/b/rickbrew/archive/2006/01/09/511003.aspx"/>
	/// </summary>
	public class ToolStripEx
		: SWF.ToolStrip
	{
		/// <summary>
		/// Gets or sets whether the ToolStripEx honors item clicks when its containing form does
		/// not have input focus.
		/// </summary>
		/// <remarks>
		/// Default value is false, which is the same behavior provided by the base ToolStrip class.
		/// </remarks>
		public bool ClickThrough { get; set; }

		protected override void WndProc(ref SWF.Message m)
		{
			base.WndProc(ref m);
			if (this.ClickThrough &&
				m.Msg == NativeConstants.WM_MOUSEACTIVATE &&
				m.Result == (IntPtr)NativeConstants.MA_ACTIVATEANDEAT)
				m.Result = (IntPtr)NativeConstants.MA_ACTIVATE;
		}
	}

	internal sealed class NativeConstants
	{
		private NativeConstants()
		{
		}
		internal const uint WM_MOUSEACTIVATE = 0x21;
		internal const uint MA_ACTIVATE = 1;
		internal const uint MA_ACTIVATEANDEAT = 2;
		internal const uint MA_NOACTIVATE = 3;
		internal const uint MA_NOACTIVATEANDEAT = 4;
	}
}
