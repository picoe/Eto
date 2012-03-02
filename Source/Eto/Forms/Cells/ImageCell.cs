using System;

namespace Eto.Forms
{
	public interface IImageCell : ICell
	{
	}
	
	public class ImageCell : SingleValueCell, IImageCell
	{
		public ImageCell (int column)
			: this()
		{
			Binding = new ColumnBinding (column);
		}
		
		public ImageCell (string property)
			: this()
		{
			Binding = new PropertyBinding (property);
		}

		public ImageCell ()
			: this(Generator.Current)
		{
		}
		
		public ImageCell (Generator g)
			: base(g, typeof(IImageCell), true)
		{
		}
	}
}

