using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

namespace Eto.Mac.Forms.ToolBar
{
	public class ToolBarViewHandler : MacView<NSToolbar, Eto.Forms.ToolBarView, Eto.Forms.ToolBar.ICallback>, Eto.Forms.ToolBarView.IHandler
	{
		Control content;
		static readonly object minimumSizeKey = new object();

		public ToolBarViewHandler()
		{
			// TODO: THIS IS JUST A STUB
		}

		public void AddItem(Control item, int index)
		{
			// TODO: THIS IS JUST A STUB
		}

		public void Clear()
		{
			// TODO: THIS IS JUST A STUB
		}

		public Size ClientSize
		{
			// TODO: is there an equal property for Mac?
			//get { return Control.ClientSize; }
			//set { Control.ClientSize = value; }
			get { return new Size(0, 0); }
			set { ; }
		}

		public override NSView ContainerControl
		{
			// TODO: Is there a way to convert NSToolbar to NSView
			//get { return Control; }
			get { return null; }
		}

		public Control Content
		{
			get { return content; }
			set
			{
				// TODO: THIS IS JUST A STUB
			}
		}

		public ContextMenu ContextMenu
		{
			get { return null; }
			set
			{
			}
		}

		public DockPosition Dock
		{
			get { return DockPosition.Top; }
			set { ; }
		}

		public override bool Enabled
		{
			get { return true; }
			set { }
		}

		public override Size MinimumSize
		{
			get { return Widget.Properties.Get<Size?>(minimumSizeKey) ?? Size.Empty; }
			set
			{
				if (value != MinimumSize)
				{
					Widget.Properties[minimumSizeKey] = value;
					// TODO: is there an equal property for Mac?
					//SetMinimumSize(useCache: true);
				}
			}
		}

		public virtual Padding Padding
		{
			// TODO: is there an equal property for Mac?
			//get { return this.Control.Padding.ToEto(); }
			//set { this.Control.Padding = value.ToSWF(); }
			get { return new Padding(); }
			set { ; }
		}

		public bool RecurseToChildren
		{
			get { return true; }
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
				}
			}
		}
	}
}
