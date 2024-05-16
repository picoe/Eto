
namespace Eto.WinUI.Forms.Controls
{
	public class LabelHandler : WinUIFrameworkElement<muc.TextBlock, Label, Label.ICallback>, Label.IHandler
	{
		public override mux.FrameworkElement ContainerControl => Control;
		protected override muc.TextBlock CreateControl() => new muc.TextBlock();

		public TextAlignment TextAlignment { get; set; }
		public VerticalAlignment VerticalAlignment { get; set; }
		public WrapMode Wrap { get; set; }
		public string Text
		{
			get => Control.Text;
			set => Control.Text = value;
		}
		public Color TextColor { get; set; }
		public Font Font { get; set; }
	}
}
