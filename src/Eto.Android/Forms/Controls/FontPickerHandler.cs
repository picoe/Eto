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
	public class FontPickerHandler : AndroidControl<av.View, FontPicker, FontPicker.ICallback>, FontPicker.IHandler
	{
		public FontPickerHandler() : base()
		{
			Control = new av.View(Platform.AppContextThemed);
			Value = Fonts.Sans(20);
		}

		public Font Value { get; set; }

		public override av.View ContainerControl => Control;
	}
}
