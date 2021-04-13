using Eto.Forms;
using System.Threading.Tasks;

namespace Eto.GtkSharp.Forms
{
	public class EtoWindow : Gtk.Window
	{
		public IGtkWindow Handler { get; set; }
		public EtoWindow(Gtk.WindowType type) : base(type)
		{
		}

#if GTK3
		protected override void OnGetPreferredHeightForWidth(int width, out int minimum_height, out int natural_height)
		{
			base.OnGetPreferredHeightForWidth(width, out minimum_height, out natural_height);
			var size = Handler.UserPreferredSize;
			if (size.Height > 0)
				natural_height = size.Height;
		}

		protected override void OnGetPreferredWidthForHeight(int height, out int minimum_width, out int natural_width)
		{

			base.OnGetPreferredWidthForHeight(height, out minimum_width, out natural_width);
			var size = Handler.UserPreferredSize;
			if (size.Width > 0)
				natural_width = size.Width;
		}

		protected override void OnGetPreferredWidth(out int minimum_width, out int natural_width)
		{
			base.OnGetPreferredWidth(out minimum_width, out natural_width);
			var size = Handler.UserPreferredSize;
			if (size.Width > 0)
				natural_width = size.Width;
		}

		protected override void OnGetPreferredHeight(out int minimum_height, out int natural_height)
		{
			base.OnGetPreferredHeight(out minimum_height, out natural_height);
			var size = Handler.UserPreferredSize;
			if (size.Height > 0)
				natural_height = size.Height;
		}
#endif
	}

	public class FormHandler : GtkWindow<Gtk.Window, Form, Form.ICallback>, Form.IHandler
	{
		public FormHandler(Gtk.Window window)
		{
			Control = window;
		}

		public FormHandler()
		{
			Control = new EtoWindow(Gtk.WindowType.Toplevel) { Handler = this };
#if GTK2
			Control.AllowGrow = true;
#else
			Control.Resizable = true;
#endif
			Control.SetPosition(Gtk.WindowPosition.Center);

			var vbox = new Gtk.VBox();
			vbox.PackStart(WindowActionControl, false, true, 0);
			vbox.PackStart(WindowContentControl, true, true, 0);
			Control.Child = vbox;
		}

		#if NET40
		public void Show ()
		{
			Control.Child.ShowAll ();
			if (ShowActivated || !Control.AcceptFocus)
				Control.Show ();
			else {
				Control.AcceptFocus = false;
				Control.Show ();
				Control.AcceptFocus = true;
			}
		}
		#else
		public async void Show()
		{
			Control.Child.ShowAll();
			if (ShowActivated || !Control.AcceptFocus)
				Control.Show();
			else
			{
				Control.AcceptFocus = false;
				Control.Show();
				await Task.Delay(1); // why???  Only way I can get it to work properly on ubuntu 16.04
				Control.AcceptFocus = CanFocus; // in case user changes it right after this call, but should be true
			}
		}
		#endif

		static object ShowActivated_Key = new object();

		public bool ShowActivated
		{
			get { return Widget.Properties.Get<bool>(ShowActivated_Key, true); }
			set { Widget.Properties.Set(ShowActivated_Key, value, true); }
		}

		static object CanFocus_Key = new object();

		public bool CanFocus
		{
			get { return Widget.Properties.Get<bool>(CanFocus_Key, true); }
			set { Widget.Properties.Set(CanFocus_Key, value, () => Control.AcceptFocus = value, true); }
		}
	}
}
