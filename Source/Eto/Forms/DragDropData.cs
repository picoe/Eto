using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Forms
{
	/// <summary>
	/// Drag/Drop action data.
	/// </summary>
	public class DragDropData
	{
		/// <summary>
		/// Gets or sets text data.
		/// </summary>
		public string Text = string.Empty;

		/// <summary>
		/// Gets or sets Uri data.
		/// </summary>
		public List<Uri> Uris = new List<Uri>();
	}
}
