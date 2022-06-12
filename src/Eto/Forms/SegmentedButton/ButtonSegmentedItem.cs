using System.Windows.Input;

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
		/// Initializes a new instance of the <see cref="T:Eto.Forms.ButtonSegmentedItem"/> class.
		/// </summary>
		public ButtonSegmentedItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ButtonSegmentedItem"/> class with the specified command.
		/// </summary>
		/// <seealso cref="SegmentedItem(Command)"/>
		/// <param name="command">Command to initialize the segmented item with.</param>
		public ButtonSegmentedItem(Command command)
			: base(command)
		{
		}

		/// <summary>
		/// Handler interface for the <see cref="ButtonSegmentedItem"/>.
		/// </summary>
		public new interface IHandler : SegmentedItem.IHandler
        {
        }
    }
}
