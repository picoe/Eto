
namespace Eto.Forms
{
	public interface IImageTextCell : ICell
	{
	}
	
	public class ImageTextCell : Cell
	{
		public IndirectBinding ImageBinding { get; set; }
		
		public IndirectBinding TextBinding { get; set; }
		
		public ImageTextCell (int imageColumn, int textColumn)
			: this()
		{
			ImageBinding = new ColumnBinding (imageColumn);
			TextBinding = new ColumnBinding (textColumn);
		}
		
		public ImageTextCell (string imageProperty, string textProperty)
			: this()
		{
			ImageBinding = new PropertyBinding(imageProperty);
			TextBinding = new PropertyBinding(textProperty);
		}
		
		public ImageTextCell ()
			: this(Generator.Current)
		{
		}
		
		public ImageTextCell (Generator g)
			: base(g, typeof(IImageTextCell), true)
		{
		}
	}
}

