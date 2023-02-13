using System;
using Eto.Drawing;
using Eto.Forms;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms.ToolBar
{
	public class ToolBarHandler : WidgetHandler<aw.Toolbar, Eto.Forms.ToolBar>, Eto.Forms.ToolBar.IHandler
	{
		Size imageSize = new Size(32, 32);
		Color? textColor = null;

		ToolBarDock dock = ToolBarDock.Top;

		public ToolBarHandler()
		{
			Control = new aw.Toolbar(Platform.AppContextThemed);
			Control.LayoutParameters = new av.ViewGroup.LayoutParams(av.ViewGroup.LayoutParams.MatchParent, av.ViewGroup.LayoutParams.WrapContent);
			Control.SetMinimumHeight(0);
			Control.SetContentInsetsAbsolute(0, 0);
			Control.SetPadding(0, 0, 0, 0);
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

		public void RemoveButton(ToolItem item, int index)
		{
			Control.RemoveView((av.View)item.ControlObject);
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

		public Size ImageSize
		{
			get
			{
				return imageSize;
			}
			set
			{
				imageSize = value;

				foreach (var item in Widget.Items)
					(item.Handler as IToolBarItemHandler).ImageSize = value;
				}
			}

		public Color? TextColor
			{
			get
				{
				return textColor;
				}
			set
				{
				textColor = value;

				foreach (var item in Widget.Items)
					(item as IToolBarItemHandler).TextColor = value ?? SystemColors.ControlText;
			}
		}

		public void Clear()
		{
			Control.RemoveAllViews();
		}
	}
}
