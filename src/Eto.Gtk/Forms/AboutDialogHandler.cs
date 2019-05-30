using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class AboutDialogHandler : WidgetHandler<Gtk.AboutDialog, AboutDialog, AboutDialog.ICallback>, AboutDialog.IHandler
	{
		Image image;

		public AboutDialogHandler()
		{
			Control = new Gtk.AboutDialog();
			Control.WrapLicense = true;
		}

		public string Copyright
		{
			get { return Control.Copyright; }
			set { Control.Copyright = value; }
		}

		public string[] Designers
		{
			get { return Control.Artists; }
			set { Control.Artists = value; }
		}

		public string[] Developers
		{
			get { return Control.Authors; }
			set { Control.Authors = value; }
		}

		public string[] Documenters
		{
			get { return Control.Documenters; }
			set { Control.Documenters = value; }
		}

		public string License
		{
			get { return Control.License; }
			set { Control.License = value; }
		}

		public Image Logo
		{
			get { return image; }
			set
			{
				image = value;
				Control.Logo = image == null ? null : image.ToGdk();
			}
		}

		public string ProgramDescription
		{
			get { return Control.Comments; }
			set { Control.Comments = value; }
		}

		public string ProgramName
		{
			get { return Control.ProgramName; }
			set { Control.ProgramName = value; }
		}

		public string Title
		{
			get { return Control.Title; }
			set { Control.Title = value; }
		}

		public string Version
		{
			get { return Control.Version; }
			set { Control.Version = value; }
		}

		public Uri Website
		{
			get { return new Uri(Control.Website); }
			set { Control.Website = value.ToString(); }
		}

		public string WebsiteLabel
		{
			get { return Control.WebsiteLabel; }
			set { Control.WebsiteLabel = value; }
		}

		public DialogResult ShowDialog(Window parent)
		{
			if (parent != null)
			{
				Control.TransientFor = ((Gtk.Window)parent.ControlObject);
				Control.Modal = true;
			}

			Control.ShowAll();
			var response = (Gtk.ResponseType)Control.Run();
			Control.Hide();
			Control.Unrealize();

			if (response == Gtk.ResponseType.Ok)
				return DialogResult.Ok;

			return DialogResult.Cancel;
		}
	}
}
