using Eto.Drawing;
using Eto.Forms;
using System;
using Eto.GtkSharp.Drawing;
using System.Text;

namespace Eto.GtkSharp.Forms
{
	public class FontDialogHandler : WidgetHandler<Gtk.FontSelectionDialog, FontDialog, FontDialog.ICallback>, FontDialog.IHandler
	{
		public FontDialogHandler()
		{
			Control = new Gtk.FontSelectionDialog(null);
		}

		public Font Font
		{
			get;
			set;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case FontDialog.FontChangedEvent:
				// handled in ShowDialog
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public DialogResult ShowDialog(Window parent)
		{
			if (parent != null)
			{
				Control.TransientFor = ((Gtk.Window)parent.ControlObject);
				Control.Modal = true;
			}
			if (Font != null)
			{
				var fontHandler = Font.Handler as FontHandler;
				var pangoFont = fontHandler.Control;
				var sb = new StringBuilder();
				sb.Append(fontHandler.FamilyName);
				if (pangoFont.Style != Pango.Style.Normal && Enum.IsDefined(typeof(Pango.Style), pangoFont.Style))
				{
					sb.Append(" ");
					sb.Append(pangoFont.Style.ToString());
				}
				if (pangoFont.Weight != Pango.Weight.Normal && Enum.IsDefined(typeof(Pango.Weight), pangoFont.Weight))
				{
					sb.Append(" ");
					sb.Append(pangoFont.Weight.ToString());
				}
				if (pangoFont.Stretch != Pango.Stretch.Normal && Enum.IsDefined(typeof(Pango.Stretch), pangoFont.Stretch) )
				{
					sb.Append(" ");
					sb.Append(pangoFont.Stretch.ToString());
				}
				sb.Append(" ");
				sb.Append(((int)fontHandler.Size).ToString());

				Console.WriteLine("Selecting font: {0}", sb);
				Control.SetFontName(sb.ToString());
			}
			else
				Control.SetFontName(string.Empty);

			Control.ShowAll();
			var response = (Gtk.ResponseType)Control.Run();
			Control.Hide();

			if (response == Gtk.ResponseType.Apply || response == Gtk.ResponseType.Ok)
			{
				Font = new Font(new FontHandler(Control.FontName));
				Callback.OnFontChanged(Widget, EventArgs.Empty);
				return DialogResult.Ok;
			}
			return DialogResult.Cancel;
		}
	}
}
