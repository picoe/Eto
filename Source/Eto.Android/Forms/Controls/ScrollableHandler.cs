using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;
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
			Control = new aw.ScrollView(aa.Application.Context);
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

		public BorderType Border
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