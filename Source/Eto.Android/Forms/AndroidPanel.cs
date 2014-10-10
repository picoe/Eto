using System;
using Eto.Forms;
using Eto.Drawing;
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
					view.LayoutParameters = new av.ViewGroup.LayoutParams(av.ViewGroup.LayoutParams.MatchParent, av.ViewGroup.LayoutParams.MatchParent);
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