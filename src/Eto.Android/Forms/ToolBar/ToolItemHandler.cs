using Eto.Forms;
using Eto.Drawing;
using System;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms.ToolBar
{
	public interface IToolBarItemHandler
	{
		Size ImageSize { get; set; }
		Color TextColor { get; set; }
		void CreateControl(ToolBarHandler handler, int index);
	}

	public abstract class ToolItemHandler<TControl, TWidget> : ToolItemHandler<TControl, TWidget, ToolItem.ICallback>
		where TControl : av.View
		where TWidget : ToolItem
	{ 
	}

	public abstract class ToolItemHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, ToolItem.IHandler, IToolBarItemHandler
		where TControl : av.View
		where TCallback : ToolItem.ICallback
		where TWidget : ToolItem
	{
		private bool enabled;
		private bool visible;
		private string text;
		private Image image;
		private Size imageSize = new Size(16, 16);
		
		public ToolItemHandler()
		{
			enabled = true;
			visible = true;
			text = String.Empty;
		}

		public void CreateControl(ToolBarHandler handler, int index)
		{
			imageSize = handler.ImageSize;

			var TheControl = GetInnerControl(handler);

			SetEverything();

			handler.Control.AddView(TheControl, index);
		}

		protected virtual void SetEverything()
		{
			SetPadding();
			SetEnabled(Enabled);
			SetVisible(Visible);
			SetText(Text);
			SetImage(Image, ImageSize);
		}

		/// <summary>
		/// Create or fetch the inner control representing the tool item
		/// </summary>
		protected abstract TControl GetInnerControl(ToolBarHandler handler);

		public String Text
		{
			get => text;
			set
			{
				if (text == value)
					return;

				text = value;

				if (HasControl)
					SetText(value);
			}
		}

		public virtual void SetText(String text)
		{
			if (!(Control is aw.TextView textControl))
				return;

			textControl.Text = text;
		}

		public string ToolTip
		{
			get; set;
		}

		public Image Image
		{
			get => image;
			set
			{
				image = value;
				
				if (HasControl)
					SetImage(value, ImageSize);
			}
		}

		public Size ImageSize
		{
			get => imageSize;
			set
			{
				imageSize = value;

				if (HasControl)
					SetImage(image, value);
			}
		}

		protected virtual void SetImage(Image image, Size size)
		{
			if (!(Control is aw.TextView textControl))
				return;

			AndroidHelpers.SetCompoundDrawable(textControl, ButtonImagePosition.Left, image, size);
		}

		public virtual Color TextColor { get; set; }

		public virtual bool Enabled
		{
			get => enabled;
			set
			{
				if (enabled == value)
					return;

				enabled = value;

				if (HasControl)
					SetEnabled(value);
			}
		}

		private void SetEnabled(bool enabled)
		{
			Control.Enabled = enabled;
		}

		private av.ViewGroup.LayoutParams GetLayoutParams()
		{
			var Gravity = GetLayoutParamsGravity();
			return new aw.Toolbar.LayoutParams(av.ViewGroup.LayoutParams.WrapContent, av.ViewGroup.LayoutParams.WrapContent, Gravity) { MarginStart = 0, MarginEnd = 0 };
		}

		private av.GravityFlags GetLayoutParamsGravity()
		{
			return av.GravityFlags.Left| av.GravityFlags.CenterVertical;
		}

		public bool Visible
		{
			get => visible;
			set
			{
				if (visible == value)
					return;

				visible = value;

				if (HasControl)
					SetVisible(value);
			}
		}

		protected virtual void SetVisible(bool visible)
		{
			Control.Visibility = visible ? av.ViewStates.Visible : av.ViewStates.Gone;
		}

		protected virtual void SetPadding()
		{
			Control.SetMinimumWidth(0);
			Control.SetMinimumHeight(0);

			if (Control is aw.TextView textControl)
			{
				textControl.SetMinWidth(0);
				textControl.SetMinHeight(0);
				textControl.SetMaxLines(1);
				textControl.Ellipsize = global::Android.Text.TextUtils.TruncateAt.End;
			}

			var Padding = Platform.DpToPx(8);
			Control.SetPadding(Padding, Padding, Padding, Padding);
		}

		public void CreateFromCommand(Command command)
		{
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
