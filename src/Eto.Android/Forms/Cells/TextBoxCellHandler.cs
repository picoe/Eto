using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
using Eto.Forms;

namespace Eto.Android.Forms.Cells
	{
	public class TextBoxCellHandler : CellHandler<TextBoxCell>, TextBoxCell.IHandler
	{
		public TextAlignment TextAlignment
		{
			get;
			set;
		}

		public VerticalAlignment VerticalAlignment
		{
			get;
			set;
		}

		public AutoSelectMode AutoSelectMode
		{
			get;
			set;
		}

		public override av.View CreateView(av.View view, object item)
		{
			var tv = view as aw.TextView ?? new aw.TextView(Platform.AppContextThemed);

			tv.Text = Widget.Binding != null ? Widget.Binding.GetValue(item) : null;
			return tv;
		}
	}
}