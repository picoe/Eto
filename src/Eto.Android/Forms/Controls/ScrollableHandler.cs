using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms.Controls
{
	/// <summary>
	/// Handler for <see cref="Scrollable"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ScrollableHandler : AndroidPanel<aw.ScrollView, Scrollable, Scrollable.ICallback>, Scrollable.IHandler
	{
		public ScrollableHandler()
		{
			Control = new aw.ScrollView(Platform.AppContextThemed);
		}

		public void UpdateScrollSizes()
		{
		}

		public override av.View ContainerControl
		{
			get { return Control; }
		}

		protected override void SetContent(av.View content)
		{
			Control.AddView(content);
		}

		public Point ScrollPosition
		{
			get { return new Point(Control.ScrollX, Control.ScrollY); }
			set { Control.ScrollTo(value.X, value.Y); }
		}

		public float MinimumZoom { get; set; }

		public float MaximumZoom { get; set; }

		public float Zoom { get; set; }

		public Size ScrollSize
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

		public BorderType Border { get; set; }

		public Rectangle VisibleRect
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool ExpandContentWidth
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

		public bool ExpandContentHeight
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