namespace Eto.GirCore.Forms.Controls
{
	public class TabPageHandler : GirPanel<Gtk.Box, TabPage, TabPage.ICallback>, TabPage.IHandler
	{
		readonly Gtk.Box tab;
		readonly Gtk.Label label;
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
			set => image = value;
		}
	}
}
