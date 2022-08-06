using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms.Controls
{
	public class SplitterHandler : AndroidContainer<aw.GridLayout, Splitter, Splitter.ICallback>, Splitter.IHandler
	{
		public SplitterFixedPanel FixedPanel { get; set; }
		public double RelativePosition { get; set; }
		public int SplitterWidth { get; set; }
		public int Panel1MinimumSize { get; set; }
		public int Panel2MinimumSize { get; set; }

		private Control panel1;
		private Control panel2;

		public Control Panel1
		{
			get => panel1;
			set
			{
				panel1 = value;
				Update();
			}
		}

		public Control Panel2
		{
			get => panel2;
			set
			{
				panel2 = value;
				Update();
			}
		}

		private Orientation orientation;

		public Orientation Orientation
		{
			get => orientation;
			set
			{
				orientation = value;
				Update();
			}
		}

		private int position;

		public int Position
		{
			get => position;
			set
			{
				position = value;
				Update();
			}
		}

		public override av.View ContainerControl => Control;

		protected override aw.GridLayout CreateControl()
		{
			Control = new aw.GridLayout(Platform.AppContextThemed);
			return Control;
		}

		private void Update()
		{
			Control.RemoveAllViews();
			Control.ColumnCount = Orientation == Orientation.Horizontal ? 2 : 1;
			Control.RowCount = Orientation == Orientation.Vertical ? 2 : 1;

			var Panel1Control = panel1?.GetAndroidControl().ContainerControl;

			if (Panel1Control == null)
				Panel1Control = new av.View(Platform.AppContextThemed);

			var Panel2Control = panel2?.GetAndroidControl().ContainerControl;

			if (Panel2Control == null)
				Panel2Control = new av.View(Platform.AppContextThemed);
			
			var spec1 = aw.GridLayout.InvokeSpec(0, 1, 1);
			var spec2 = aw.GridLayout.InvokeSpec(1, 1, 1);

			if (orientation == Orientation.Horizontal)
			{
				var lay1 = new aw.GridLayout.LayoutParams(spec1, spec1);
				var lay2 = new aw.GridLayout.LayoutParams(spec1, spec2);
				lay1.SetGravity(av.GravityFlags.FillVertical);
				lay2.SetGravity(av.GravityFlags.FillVertical);
				lay1.Width = 0;
				lay2.Width = 0;
				lay1.Height = 0;
				lay2.Height = 0;
				Control.AddView(Panel1Control, lay1);
				Control.AddView(Panel2Control, lay2);
			}

			else
			{
				var lay1 = new aw.GridLayout.LayoutParams(spec1, spec1);
				var lay2 = new aw.GridLayout.LayoutParams(spec2, spec1);
				lay1.SetGravity(av.GravityFlags.FillHorizontal);
				lay2.SetGravity(av.GravityFlags.FillHorizontal);
				lay1.Width = 0;
				lay2.Width = 0;
				lay1.Height = 0;
				lay2.Height = 0;
				Control.AddView(Panel1Control, lay1);
				Control.AddView(Panel2Control, lay2);
			}
		}
	}
}