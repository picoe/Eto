using System;
using Eto.Drawing;


#if XAML
using System.Windows.Markup;
#endif

namespace Eto.Forms
{
	/// <summary>
	/// Extensions related to the <see cref="DockLayout"/> class
	/// </summary>
	public static class DockLayoutExtensions
	{
		/// <summary>
		/// Adds the <paramref name="control"/> to a <see cref="DockLayout"/> on the specified <paramref name="container"/>
		/// </summary>
		/// <remarks>
		/// This is just a shortcut to doing this, to make code more terse:
		/// <code>
		///		var layout = new DockLayout(container);
		///		layout.Content = control;
		///		layout.Padding = padding;
		/// </code>
		/// </remarks>
		/// <param name="container">Container to add the control to</param>
		/// <param name="control">Control to add to the container</param>
		/// <param name="padding">Amount of padding around the control, inside the container</param>
		/// <returns></returns>
		[Obsolete("use the Panel.Content property instead")]
		public static Container AddDockedControl (this Panel container, Control control, Padding? padding = null)
		{
			container.Content = control;
			if (padding != null)
				container.Padding = padding.Value;
			return container;
		}
	}
	
	/// <summary>
	/// Layout to fill the content of a <see cref="Container"/> with a single control
	/// </summary>
	/// <remarks>
	/// This layout is used to fill an entire container with a single content control.
	/// </remarks>
	[ContentProperty("Content")]
	[Obsolete("Use the Panel.Content property instead, or use a Panel")]
	public class DockLayout : Panel
	{
		[Obsolete("Use Panel directly instead")]
		public Container Container { get { return Parent; } }

		/// <summary>
		/// Creates a new <see cref="Panel"/> with a DockLayout and the specified content <paramref name="control"/>.
		/// </summary>
		/// <remarks>
		/// This is just a shorthand for the following, to make code more terse.
		/// <code>
		///		var panel = new Panel ();
		///		var layout = new DockLayout (panel);
		///		layout.Padding = padding;
		///		layout.Content = content;
		/// </code>
		/// </remarks>
		/// <param name="control">Control to set as the content of the panel using a DockLayout</param>
		/// <param name="padding">Amount of padding around the content, or null to use the default padding</param>
		/// <returns>A new <see cref="Panel"/> initialized with a DockLayout and the specified content control</returns>
		public static Panel CreatePanel (Control control, Padding? padding = null)
		{
			var panel = new Panel ();
			panel.AddDockedControl (control, padding);
			return panel;
		}
		
		/// <summary>
		/// Initializes a new instance of the DockLayout with an unspecified container
		/// </summary>
		/// <remarks>
		/// Used typically when creating for json or xaml.  Use <see cref="DockLayout(Panel)"/> when
		/// calling through code.
		/// </remarks>
		public DockLayout ()
			: this(null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the DockLayout for the specified container
		/// </summary>
		/// <param name="container">Container for the dock layout to manage</param>
		public DockLayout (Panel container)
		{
			if (container != null)
				container.Content = this;
		}

		/// <summary>
		/// Obsolete. Use <see cref="Panel.Content"/> instead
		/// </summary>
		[Obsolete ("Use Content property instead")]
		public void Add (Control control)
		{
			Content = control;
		}
	}
}
