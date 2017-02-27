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
	public interface ICellHandler
	{
		av.View CreateView(av.View view, object item);
	}

	public abstract class CellHandler<TWidget> : WidgetHandler<TWidget>, Cell.IHandler, ICellHandler
		where TWidget: Cell
	{
		public abstract av.View CreateView(av.View view, object item);
	}

	public class TextBoxCellHandler : CellHandler<TextBoxCell>, TextBoxCell.IHandler
	{
		public TextAlignment TextAlignment
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

		public VerticalAlignment VerticalAlignment
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

		public override av.View CreateView(av.View view, object item)
		{
			var tv = view as aw.TextView ?? new aw.TextView(aa.Application.Context);

			tv.Text = Widget.Binding != null ? Widget.Binding.GetValue(item) : null;
			return tv;
		}
	}
}