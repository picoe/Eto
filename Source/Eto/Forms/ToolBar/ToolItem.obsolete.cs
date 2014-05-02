using System;

namespace Eto.Forms
{
	/// <summary>
	/// Obsolete, use <see cref="ButtonToolItem"/> instead
	/// </summary>
	[Obsolete("Use ButtonToolItem instead")]
	public class ToolBarButton : ButtonToolItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ToolBarButton"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		public ToolBarButton(Generator generator = null)
			: base(generator)
		{
		}
	}

	/// <summary>
	/// Obsolete, use <see cref="CheckToolItem"/> instead
	/// </summary>
	[Obsolete("Use CheckToolItem instead")]
	public class CheckToolBarButton : CheckToolItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckToolBarButton"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		public CheckToolBarButton(Generator generator = null)
			: base(generator)
		{
		}
	}

	/// <summary>
	/// Obsolete, use <see cref="SeparatorToolItem"/> instead
	/// </summary>
	[Obsolete("Use SeparatorToolItem instead")]
	public class SeparatorToolBarItem : SeparatorToolItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SeparatorToolBarItem"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		public SeparatorToolBarItem(Generator generator = null)
			: base(generator)
		{
		}
	}
}

