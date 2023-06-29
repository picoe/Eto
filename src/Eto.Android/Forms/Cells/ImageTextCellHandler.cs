using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms.Cells
{
	public class ImageTextCellHandler : CellHandler<ImageTextCell>, ImageTextCell.IHandler
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

		public ImageInterpolation ImageInterpolation
		{
			get;
			set;
		}

		public override av.View CreateView(av.View view, object item)
		{
			var tv = view as aw.TextView ?? new aw.TextView(Platform.AppContextThemed);

			tv.Text = Widget.TextBinding?.GetValue(item);
			return tv;
		}
	}
}
