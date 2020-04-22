namespace Eto.Forms
{
    /// <summary>
    /// Provides access to common data formats in a <see cref="DataObject"/>.
    /// </summary>
    public static class DataFormats
    {
        static IHandler Handler => Platform.Instance.CreateShared<IHandler>();

        /// <summary>
        /// Gets the data format used for plain text
        /// </summary>
        public static string Text => Handler.Text;

        /// <summary>
        /// Gets the data format used for html
        /// </summary>
        public static string Html => Handler.Html;

        /// <summary>
        /// Gets the data format used for a color object
        /// </summary>
        public static string Color => Handler.Color;

        /// <summary>
        /// Handler interface for implementations of the DataFormats object.
        /// </summary>
        public interface IHandler
        {
            /// <summary>
            /// Gets the data format used for plain text
            /// </summary>
            string Text { get; }
            /// <summary>
            /// Gets the data format used for html
            /// </summary>
            string Html { get; }
            /// <summary>
            /// Gets the data format used for a color object
            /// </summary>
            string Color { get; }
        }
    }
}
