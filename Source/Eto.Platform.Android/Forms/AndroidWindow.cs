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

	/// <summary>
	/// Base handler for <see cref="IWindow"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class AndroidWindow<TWidget> : AndroidPanel<aw.FrameLayout, TWidget>, IWindow, IAndroidWindow
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
		}

		public override av.View ContainerControl
		{
			get { return InnerFrame; }
		}

		public void Close()
		{
			//a.App.Application.Context.Start
		}

		protected override void SetContent(av.View content)
		{
		}

		public ToolBar ToolBar { get; set; }

		public double Opacity { get; set; }

		public string Title { get; set; }

		public Screen Screen
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}