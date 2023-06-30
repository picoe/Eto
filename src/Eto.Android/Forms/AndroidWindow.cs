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
			set 
			{
				activity = value;
				ToolBar = ToolBar;
			}
		}

		protected virtual a.App.Activity CreateActivity()
		{
			return null; // todo
		}

		protected AndroidWindow()
		{
			Control = InnerFrame;
		}

		private aw.FrameLayout toolBarContainer;
		private aw.LinearLayout Lin;

		protected override aw.FrameLayout CreateFrame()
		{
			var Frame = base.CreateFrame();

			Lin = new aw.LinearLayout(Platform.AppContextThemed)
			{
				Orientation = aw.Orientation.Vertical,
				LayoutParameters = new av.ViewGroup.LayoutParams(av.ViewGroup.LayoutParams.MatchParent, av.ViewGroup.LayoutParams.MatchParent)
			};

			toolBarContainer = new aw.FrameLayout(Platform.AppContextThemed);

			Lin.AddView(toolBarContainer);
			Lin.AddView(Frame);

			return Frame;
		}

		public override av.View ContainerControl
		{
			get { return Lin; }
		}

		public bool Closeable
		{
			get { return false; }
			set { }
		}

		public virtual void Close()
		{
			//a.App.Application.Context.Start
		}

		protected override void SetContent(av.View content)
		{
		}

		public Eto.Forms.ToolBar ToolBar
		{
			get
			{
				return toolBar;
			}
			set
			{
				toolBar = value;
				toolBarContainer.RemoveAllViews();

				if (Activity != null && toolBar != null)
				{
					var tb = (aw.Toolbar)toolBar.ControlObject;

					toolBarContainer.AddView(tb);
				}
			}
		}

		private Eto.Forms.ToolBar toolBar;

		public double Opacity { get; set; }

		public string Title { get; set; }

		public Screen Screen => Screen.PrimaryScreen;

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
			get;
			set;
		}

		public bool Maximizable
		{
			get;
			set;
		}

		public bool Minimizable
		{
			get;
			set;
		}

		public bool ShowInTaskbar
		{
			get;
			set;
		}

		public bool Topmost
		{
			get;
			set;
		}

		public WindowState WindowState
		{
			get;
			set;
		}

		public Rectangle RestoreBounds
		{
			get { throw new NotImplementedException(); }
		}

		public WindowStyle WindowStyle
		{
			get;
			set;
		}

		public void BringToFront()
		{
			
		}

		public void SendToBack()
		{
			
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

		public Boolean MovableByWindowBackground
		{
			get;
			set;
		}

		public Size MaximumSize { get; set; }
		public bool AutoSize { get; set; }
	}
}