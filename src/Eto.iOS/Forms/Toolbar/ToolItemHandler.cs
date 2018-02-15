using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using ObjCRuntime;
using UIKit;
using sd = System.Drawing;

namespace Eto.iOS.Forms.Toolbar
{

	public abstract class ToolItemHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, ToolItem.IHandler, IToolBarItemHandler
		where TControl : UIBarButtonItem
		where TWidget : ToolItem
	{
		Image image;
		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				SetImage();
			}
		}

		void SetImage()
		{
			var nsimage = image.ToUI();
#if TODO
			if (tint != null && nsimage != null)
				nsimage = nsimage.Tint(tint.Value.ToNSUI());
#endif
			button.SetImage(nsimage, UIControlState.Normal);
		}

		Color? tint;
		public Color? Tint
		{
			get { return tint; }
			set
			{
				tint = value;
			}
		}

		UIButton button;

		public void CreateFromCommand(Command command)
		{
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			// Create a button so that any image can be used.
			// (A standard toolbar item uses only the alpha channel of the image.)
			button = new UIButton(new CoreGraphics.CGRect(0, 0, 40, 40));
			button.TouchUpInside += (s, e) => OnClick();
			Control = (TControl)new UIBarButtonItem(button);

			Control.Enabled = true;
		}

		public string Text
		{
			get { return Control.Title; }
			set { Control.Title = value; button.SetTitle(value, UIControlState.Normal); }
		}

		public string ToolTip { get; set; } // iOS does not support ToolTip

		public bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public virtual bool Selectable { get; set; }

		public virtual void ControlAdded(ToolBarHandler toolbar)
		{
		}

		public virtual void InvokeButton()
		{
		}

		public void OnClick()
		{
			InvokeButton();
		}

		UIBarButtonItem IToolBarBaseItemHandler.Control
		{
			get { return Control; }
		}

		public UIBarButtonItem ButtonItem
		{
			get { return Control; }
		}

		public UIButton Button
		{
			get { return button; }
		}

		public virtual void OnLoad(EventArgs e)
		{
		}

		public virtual void OnPreLoad(EventArgs e)
		{
		}

		public virtual void OnUnLoad(EventArgs e)
		{
		}
	}
	
}
