using System;
using System.Threading.Tasks;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Platform.Android.Forms
{
	public interface IAndroidWindow
	{
		a.App.Activity Activity { get; set; }
	}

	public abstract class AndroidWindow<TWidget> : AndroidDockContainer<aw.FrameLayout, TWidget>, IWindow, IAndroidWindow
		where TWidget: Window
	{
		a.App.Activity activity;
		public a.App.Activity Activity
		{
			get { return activity ?? (activity = CreateActivity()); }
			set { activity = value; }
		}

		protected virtual a.App.Activity CreateActivity()
		{
			return null; // todo
		}

		protected AndroidWindow()
		{
			Control = new aw.FrameLayout(a.App.Application.Context);
		}

		public override av.View ContainerControl
		{
			get { return Control; }
		}

		public void Close()
		{
			//a.App.Application.Context.Start
		}

		protected override void SetContent(av.View content)
		{
			Control.AddView(content);
		}

		public ToolBar ToolBar
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

		public double Opacity
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

		public string Title
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

		public Screen Screen
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}

