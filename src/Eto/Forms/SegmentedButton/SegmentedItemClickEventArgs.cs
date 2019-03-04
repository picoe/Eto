using System;

namespace Eto.Forms
{
	/// <summary>
	/// Event arguments when clicking a segment in the <see cref="SegmentedButton"/>.
	/// </summary>
    public class SegmentedItemClickEventArgs : EventArgs
    {
		/// <summary>
		/// Gets the item that was clicked
		/// </summary>
		/// <value>The item that was clicked.</value>
        public SegmentedItem Item { get; }

		/// <summary>
		/// Gets the index of the item that was clicked.
		/// </summary>
		/// <value>The index of the item that was clicked.</value>
        public int Index { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.SegmentedItemClickEventArgs"/> class.
		/// </summary>
		/// <param name="item">Item that was clicked.</param>
		/// <param name="index">Index of the item that was clicked.</param>
        public SegmentedItemClickEventArgs(SegmentedItem item, int index)
        {
            Item = item;
            Index = index;
        }
    }
}
