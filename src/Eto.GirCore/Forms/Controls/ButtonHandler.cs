namespace Eto.GirCore.Forms.Controls
{
	public class ButtonHandler : GirControl<Gtk.Button, Button, Button.ICallback>, Button.IHandler
	{
		public ButtonHandler()
		{
			Control = Gtk.Button.New();
			Control.OnClicked += (sender, e) => Callback.OnClick(Widget, EventArgs.Empty);
		}

		public virtual string? Text
		{
			get { return Control.Label; }
			set { Control.Label = value; }
		}

		public Image Image { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ButtonImagePosition ImagePosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Size MinimumSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Color TextColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool UseMnemonic { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool AlwaysShowMnemonic { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
}