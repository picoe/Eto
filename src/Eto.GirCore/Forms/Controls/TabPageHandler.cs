namespace Eto.GirCore.Forms.Controls
{
	public class TabPageHandler : GirPanel<Gtk.Box, TabPage, TabPage.ICallback>, TabPage.IHandler
	{
		readonly Gtk.Box tab;
		readonly Gtk.Label label;
		Gtk.Image? imageView;
		Image? image;

		public TabPageHandler()
		{
			Control = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
			tab = Gtk.Box.New(Gtk.Orientation.Horizontal, 6);
			label = Gtk.Label.New(null);
			tab.Append(label);
		}

		internal Gtk.Widget LabelControl => tab;

		protected override void SetContainerContent(Gtk.Widget content)
		{
			Control.Append(content);
		}

		protected override void RemoveContainerContent(Gtk.Widget content)
		{
			Control.Remove(content);
		}

		public string Text
		{
			get => label.GetText().ToEtoMnemonic();
			set => label.SetTextWithMnemonic(value.ToPlatformMnemonic());
		}

		public Image? Image
		{
			get => image;
			set
			{
				image = value;
				if (imageView != null)
				{
					tab.Remove(imageView);
					imageView = null;
				}
				if (image != null)
				{
					imageView = Drawing.GirImageHelper.CreateImage(image, new Size(16, 16));
					tab.Prepend(imageView);
				}
			}
		}
	}
}
