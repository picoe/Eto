using a = Android;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Android.Forms
{
	/// <summary>
	/// Base handler for <see cref="Panel"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class AndroidPanel<TControl, TWidget, TCallback> : AndroidContainer<TControl, TWidget, TCallback>, Panel.IHandler
		where TControl: av.View
		where TWidget: Panel
		where TCallback: Panel.ICallback
	{
		readonly aw.FrameLayout frame;
		Control content;
		Size minimumSize;

		protected aw.FrameLayout InnerFrame { get { return frame; } }

		protected AndroidPanel()
		{
			frame = CreateFrame();
		}

		protected virtual aw.FrameLayout CreateFrame()
		{
			return new aw.FrameLayout(Platform.AppContextThemed) { LayoutParameters = new av.ViewGroup.LayoutParams(av.ViewGroup.LayoutParams.MatchParent, av.ViewGroup.LayoutParams.MatchParent) };
		}

		protected override void Initialize()
		{
			base.Initialize();
			SetContent(frame);
		}

		public Control Content
		{
			get { return content; }
			set
			{
				if (content != null)
				{
					var view = content.GetContainerView();
					frame.RemoveView(view);
				}
				content = value;
				if (content != null)
				{
					var view = content.GetContainerView();
					view.LayoutParameters = new av.ViewGroup.LayoutParams(av.ViewGroup.LayoutParams.MatchParent, av.ViewGroup.LayoutParams.MatchParent);
					frame.AddView(view);
				}
			}
		}

		protected abstract void SetContent(av.View content);

		public override Size Size
		{
			get
			{
				return base.Size;
			}

			set
			{
				base.Size = value;

				var pxw = value.Width >= 0 ? Platform.DpToPx(value.Width) : av.ViewGroup.LayoutParams.WrapContent;
				var pxh = value.Height >= 0 ? Platform.DpToPx(value.Height) : av.ViewGroup.LayoutParams.WrapContent;

				InnerFrame.LayoutParameters = AndroidHelpers.CreateOrAdjustLayoutParameters(InnerFrame.LayoutParameters, pxw, pxh);
			}
		}

		public Padding Padding
		{
			get { return frame.GetPadding(); }
			set { frame.SetPadding(value); }
		}

		public Size MinimumSize
		{
			get { return minimumSize; }
			set
			{
				value = Platform.DpToPx(value);
				minimumSize = value;
				ContainerControl.SetMinimumWidth(value.Width);
				ContainerControl.SetMinimumHeight(value.Height);
			}
		}

		public ContextMenu ContextMenu
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}