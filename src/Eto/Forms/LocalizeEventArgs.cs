using System;
using Eto.Forms;

namespace Eto.Forms
{
    /// <summary>
    /// Event arguments for localization
    /// </summary>
    public class LocalizeEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the text to localize
        /// </summary>
        /// <value>The text to localize.</value>
        public string Text { get; private set; }

        /// <summary>
        /// Gets or sets the localized text.
        /// </summary>
        /// <value>The localized text.</value>
        public string LocalizedText { get; set; }

        /// <summary>
        /// Gets the source widget for the localized text
        /// </summary>
        /// <value>The source object.</value>
        public object Source { get; private set; }

		internal string GetResultAndReset()
		{
            var result = LocalizedText ?? Text;
            Source = null;
            Text = null;
            LocalizedText = null;
            return result;
		}

        internal void Initialize(object source, string text)
        {
            Text = text;
            Source = source;
        }
    }
}
