using System;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Platform.Android.Forms
{
	/// <summary>
	/// Base handler for <see cref="IPanel"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class AndroidPanel<T, TWidget> : AndroidContainer<T, TWidget>, IPanel
		where TWidget: Panel
	{
		readonly aw.FrameLayout frame;
		Control content;
		Size minimumSize;

		protected aw.FrameLayout InnerFrame { get { return frame; } }

		protected AndroidPanel()
		{
			frame = new aw.FrameLayout(a.App.Application.Context);
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
					view.LayoutParameters = new av.ViewGroup.LayoutParams(av.ViewGroup.LayoutParams.FillParent, av.ViewGroup.LayoutParams.FillParent);
					frame.AddView(view);
				}
			}
		}

		protected abstract void SetContent(av.View content);

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
				minimumSize = value;
				ContainerControl.SetMinimumWidth(value.Width);
				ContainerControl.SetMinimumHeight(value.Height);
			}
		}
	}
}