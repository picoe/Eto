using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


#if XAML
using System.Windows.Markup;
#endif

namespace Eto.Forms
{
	/// <summary>
	/// Platform handler for the <see cref="DockLayout"/> class
	/// </summary>
	public interface IDockLayout : ILayout
	{
		/// <summary>
		/// Gets or sets the padding around the content
		/// </summary>
		Padding Padding { get; set; }

		/// <summary>
		/// Gets or sets the content of the container
		/// </summary>
		Control Content { get; set; }
	}
	
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
		public static Container AddDockedControl (this Container container, Control control, Padding? padding = null)
		{
			var layout = container.Layout as DockLayout;
			if (layout == null)
				layout = new DockLayout (container);
			if (padding != null)
				layout.Padding = padding.Value;
			layout.Content = control;
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
	public class DockLayout : Layout
	{
		new IDockLayout Handler { get { return (IDockLayout)base.Handler; } }
		Control control;

		/// <summary>
		/// Gets or sets the default amount of padding for all new DockLayout objects
		/// </summary>
		public static Padding DefaultPadding = Padding.Empty;
		
		/// <summary>
		/// Gets an enumeration of all controls in this layout
		/// </summary>
		public override IEnumerable<Control> Controls {
			get {
				if (control != null)
					yield return control;
				else
					yield break;
			}
		}
		
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
		/// Used typically when creating for json or xaml.  Use <see cref="DockLayout(Container)"/> when
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
		public DockLayout (Container container)
			: base (container != null ? container.Generator : Generator.Current, container, typeof (IDockLayout))
		{
		}
		
		/// <summary>
		/// Obsolete. Use <see cref="Content"/> instead
		/// </summary>
		[Obsolete ("Use Content property instead")]
		public void Add (Control control)
		{
			Content = control;
		}

		/// <summary>
		/// Obsolete. Use <see cref="Content"/> instead
		/// </summary>
		[Obsolete ("Use Content property instead")]
		public void Remove (Control control)
		{
			Content = null;
		}
		
		/// <summary>
		/// Gets or sets the control to fill the content of the container
		/// </summary>
		public Control Content {
			get { return Handler.Content; }
			set {
                if (value.ControlObject is Control)
                    value = value.ControlObject as Control;

				control = value;
				if (control != null) {
					control.SetParentLayout (this);
					var load = Loaded && !control.Loaded;
					if (load) {
						control.OnPreLoad (EventArgs.Empty);
						control.OnLoad (EventArgs.Empty);
					}
					Handler.Content = control;
					if (load)
						control.OnLoadComplete (EventArgs.Empty);
				}
				else
					Handler.Content = control;
			}
		}
		
		/// <summary>
		/// Gets or sets the amount of padding around the child control
		/// </summary>
		public Padding Padding {
			get { return Handler.Padding; }
			set { Handler.Padding = value; }
		}
	}
}
