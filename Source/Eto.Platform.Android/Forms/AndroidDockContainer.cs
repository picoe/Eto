using System;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Platform.Android.Forms
{
	public abstract class AndroidDockContainer<T, TWidget> : AndroidContainer<T, TWidget>, IDockContainer
		where TWidget: DockContainer
	{
		readonly aw.FrameLayout frame;
		Control content;

		protected AndroidDockContainer()
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
	}
}

