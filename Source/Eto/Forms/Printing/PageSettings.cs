using Eto.Drawing;
using System;

namespace Eto.Forms
{
	/// <summary>
	/// Settings for a single printed page. Not currently mapped to any platform.
	/// </summary>
	[Handler(typeof(PageSettings.IHandler))]
	public class PageSettings : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.PageSettings"/> class.
		/// </summary>
		public PageSettings()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.PageSettings"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public PageSettings(Generator generator)
			: base(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Gets the printable area for the page
		/// </summary>
		/// <value>The printable area.</value>
		public RectangleF PrintableArea
		{
			get { return Handler.PrintableArea; }
		}

		/// <summary>
		/// Handler interface for the <see cref="PageSettings"/> control
		/// </summary>
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Gets the printable area for the page
			/// </summary>
			/// <value>The printable area.</value>
			RectangleF PrintableArea { get; }
		}
	}
}
