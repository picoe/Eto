using System;
using System.Threading.Tasks;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Android.Forms
{
	public interface IAndroidWindow
	{
		a.App.Activity Activity { get; set; }
	}

	/// <summary>
	/// Base handler for <see cref="Window"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class AndroidWindow<TWidget, TCallback> : AndroidPanel<aw.FrameLayout, TWidget, TCallback>, Window.IHandler, IAndroidWindow
		where TWidget: Window
		where TCallback: Window.ICallback
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


		public MenuBar Menu
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Icon Icon { get { return null; } set { } }

		public bool Resizable
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

		public bool Maximizable
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

		public bool Minimizable
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

		public bool ShowInTaskbar
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

		public bool Topmost
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

		public WindowState WindowState
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

		public Rectangle RestoreBounds
		{
			get { throw new NotImplementedException(); }
		}

		public WindowStyle WindowStyle
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

		public void BringToFront()
		{
			throw new NotImplementedException();
		}

		public void SendToBack()
		{
			throw new NotImplementedException();
		}

		public void SetOwner(Window owner)
		{
		}

		public float LogicalPixelSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}