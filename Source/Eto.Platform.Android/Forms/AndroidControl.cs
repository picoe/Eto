using System;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Platform.Android.Forms
{
	public interface IAndroidControl
	{
		av.View ContainerControl { get; }
	}

	/// <summary>
	/// Base handler for <see cref="IControl"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class AndroidControl<T, TWidget> : WidgetHandler<T, TWidget>, IControl, IAndroidControl
		where TWidget: Control
	{
		public abstract av.View ContainerControl { get; }

		public void Invalidate()
		{
			throw new NotImplementedException();
		}

		public void Invalidate(Rectangle rect)
		{
			throw new NotImplementedException();
		}

		public void SuspendLayout()
		{
			throw new NotImplementedException();
		}

		public void ResumeLayout()
		{
			throw new NotImplementedException();
		}

		public void Focus()
		{
			throw new NotImplementedException();
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

		public Size Size
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

		public virtual bool Enabled
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

		public bool HasFocus
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool Visible
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
	}
}