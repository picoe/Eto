using System;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Android.Forms
{
	public interface IAndroidControl
	{
		av.View ContainerControl { get; }
	}

	/// <summary>
	/// Base handler for <see cref="Control"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class AndroidControl<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Control.IHandler, IAndroidControl
		where TControl: av.View
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		public abstract av.View ContainerControl { get; }

		public void Invalidate(bool invalidateChildren)
		{
			throw new NotImplementedException();
		}

		public void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			throw new NotImplementedException();
		}

		public void SuspendLayout()
		{
		}

		public void ResumeLayout()
		{
		}

		public void Focus()
		{
			Control.RequestFocus();
		}

		public virtual void OnPreLoad(EventArgs e)
		{
		}

		public virtual void OnLoad(EventArgs e)
		{
		}

		public virtual void OnLoadComplete(EventArgs e)
		{
		}

		public virtual void OnUnLoad(EventArgs e)
		{
		}

		public virtual void SetParent(Container parent)
		{
		}

		public PointF PointFromScreen(PointF point)
		{
			throw new NotImplementedException();
		}

		public PointF PointToScreen(PointF point)
		{
			throw new NotImplementedException();
		}

		Color? backgroundColor;

		public Color BackgroundColor
		{
			get { return backgroundColor ?? Colors.Transparent; }
			set
			{
				backgroundColor = value;
				ContainerControl.SetBackgroundColor(value.ToAndroid());
			}
		}

		public virtual Size Size
		{
			get { return new Size(ContainerControl.Width, ContainerControl.Height); }
			set
			{
				Control.SetMinimumWidth(value.Width);
				Control.SetMinimumHeight(value.Height);
			}
		}

		public virtual bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public bool HasFocus
		{
			get { return Control.IsFocused; }
		}

		public bool Visible
		{
			get { return Control.Visibility == av.ViewStates.Visible; }
			set { Control.Visibility = value ? av.ViewStates.Visible : av.ViewStates.Invisible; }
		}

		public virtual Point Location
		{
			get
			{
				throw new NotImplementedException();
			}
			set { }
		}

		public IEnumerable<string> SupportedPlatformCommands
		{
			get { return Enumerable.Empty<string>(); }
		}

		public void MapPlatformCommand(string systemAction, Command action)
		{
			throw new NotImplementedException();
		}


		public string ToolTip
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

		public Cursor Cursor
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

		public bool ShowBorder
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