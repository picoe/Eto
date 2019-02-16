namespace Eto.Forms
{
	/// <summary>
	/// Segmented item that can be clicked.
	/// </summary>
    [Handler(typeof(IHandler))]
    public class ButtonSegmentedItem : SegmentedItem
    {
        new IHandler Handler => (IHandler)base.Handler;

		/// <summary>
		/// Handler interface for the <see cref="ButtonSegmentedItem"/>.
		/// </summary>
		public new interface IHandler : SegmentedItem.IHandler
        {
        }
    }
}
