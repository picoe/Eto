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

namespace Eto.Android.Forms.Cells
{
	public class ImageViewCellHandler : CellHandler<ImageViewCell>, ImageViewCell.IHandler
	{
	public ImageInterpolation ImageInterpolation
		{
			get;
			set;
		}

		public override av.View CreateView(av.View view, object item)
		{
			return new aw.Space(Platform.AppContextThemed);
		}
	}
}
