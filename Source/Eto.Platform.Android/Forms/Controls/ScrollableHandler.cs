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

namespace Eto.Platform.Android.Forms.Controls
{
	/// <summary>
	/// Handler for <see cref="IScrollable"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ScrollableHandler : AndroidDockContainer<aw.ScrollView, Scrollable>, IScrollable
	{
		public void UpdateScrollSizes()
		{
			Control = new aw.ScrollView(aa.Application.Context);
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