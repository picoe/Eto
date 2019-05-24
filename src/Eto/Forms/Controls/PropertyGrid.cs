using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Event arguments for the <see cref="PropertyGrid"/> when a value changes.
	/// </summary>
	public sealed class PropertyValueChangedEventArgs : EventArgs
    {
		/// <summary>
		/// Gets the old value of the property
		/// </summary>
        public object OldValue { get; }

		/// <summary>
		/// Name of the property that was changed
		/// </summary>
        public string PropertyName { get; }

		/// <summary>
		/// Item that the property was set on
		/// </summary>
		public object Item { get; }

		/// <summary>
		/// Initializes a new instance of the PropertyValueChangedEventArgs class.
		/// </summary>
		/// <param name="propertyName">Name of the property that was changed</param>
		/// <param name="oldValue">Old value before the change</param>
		/// <param name="item">Item that the property was set on.</param>
        public PropertyValueChangedEventArgs(string propertyName, object oldValue, object item)
        {
            PropertyName = propertyName;
            OldValue = oldValue;
			Item = item;
        }
    }

	/// <summary>
	/// Interface for custom type editors of the <see cref="PropertyGrid"/>
	/// </summary>
	/// <remarks>
	/// Use the System.ComponentModel.EditorAttribute to specify the editor for a particular property or type.
	/// For example:
	/// <code>
	/// [Editor(typeof(MyEditorClass), typeof(PropertyGridTypeEditor))]
	/// </code>
	/// </remarks>
	public abstract class PropertyGridTypeEditor
    {
		/// <summary>
		/// Creates the control for editing (and viewing for platforms that support it)
		/// </summary>
		/// <param name="args">Arguments to create the control</param>
		/// <returns>The control instance to set to the cell</returns>
        public abstract Control CreateControl(CellEventArgs args);

		/// <summary>
		/// Paints the cell when viewing for platforms that don't support custom controls for non-editing cells (E.g. Gtk, WinForms).
		/// </summary>
		/// <param name="args">Arguments to paint the cell</param>
		public abstract void PaintCell(CellPaintEventArgs args);
    }

	/// <summary>
	/// Control to edit the properties of one or more objects.
	/// </summary>
	[Handler(typeof(IHandler))]
	public class PropertyGrid : Control
	{
		new IHandler Handler => (IHandler)base.Handler;

		/// <summary>
		/// Gets or sets the selected object for the grid to edit
		/// </summary>
		public object SelectedObject
		{
			get => Handler.SelectedObject;
			set => Handler.SelectedObject = value;
		}

		/// <summary>
		/// Gets or sets the selected objects for the grid to edit
		/// </summary>
		/// <remarks>
		/// Only common properties (with the same name and type) will be shown.
		/// </remarks>
		public IEnumerable<object> SelectedObjects
		{
			get => Handler.SelectedObjects;
			set => Handler.SelectedObjects = value;
		}

		/// <summary>
		/// Gets or sets a value indicating that the categories should be shown
		/// </summary>
		[DefaultValue(true)]
		public bool ShowCategories
		{
			get => Handler.ShowCategories;
			set => Handler.ShowCategories = value;
		}

		/// <summary>
		/// Gets or sets a value indicating that the description panel should be shown
		/// </summary>
		/// <remarks>
		/// The description panel shows the name and description of the selected property
		/// </remarks>
		[DefaultValue(true)]
		public bool ShowDescription
		{
			get => Handler.ShowDescription;
			set => Handler.ShowDescription = value;
		}

		/// <summary>
		/// Refreshes the grid with new values from the selected object(s)
		/// </summary>
		public void Refresh() => Handler.Refresh();

		/// <summary>
		/// Event identifier for the <see cref="PropertyValueChanged"/> event.
		/// </summary>
		public const string PropertyValueChangedEvent = "PropertyGrid.PropertyValueChanged";

		/// <summary>
		/// Event to handle when a property value has been changed.
		/// </summary>
		public event EventHandler<PropertyValueChangedEventArgs> PropertyValueChanged
		{
			add => Properties.AddHandlerEvent(PropertyValueChangedEvent, value);
			remove => Properties.RemoveEvent(PropertyValueChangedEvent, value);
		}

		/// <summary>
		/// Raises the <see cref="PropertyValueChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnPropertyValueChanged(PropertyValueChangedEventArgs e)
		{
			Properties.TriggerEvent(PropertyValueChangedEvent, this, e);
		}

		static Callback s_callback = new Callback();

		/// <summary>
		/// Gets the callback object for this control to raise events.
		/// </summary>
		/// <returns>The callback object for this control.</returns>
		protected override object GetCallback() => s_callback;

		/// <summary>
		/// Callback implementation for the <see cref="PropertyGrid"/>.
		/// </summary>
		protected new class Callback : Control.Callback, ICallback
		{
			/// <summary>
			/// Raises the PropertyValueChanged event.
			/// </summary>
			public void OnPropertyValueChanged(PropertyGrid widget, PropertyValueChangedEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnPropertyValueChanged(e);
			}
		}

		/// <summary>
		/// Callback interface for the <see cref="PropertyGrid"/>
		/// </summary>
		public new interface ICallback : Control.ICallback
		{
			/// <summary>
			/// Raises the PropertyValueChanged event.
			/// </summary>
			void OnPropertyValueChanged(PropertyGrid widget, PropertyValueChangedEventArgs e);
		}

		/// <summary>
		/// Handler interface for the <see cref="PropertyGrid"/>
		/// </summary>
		public new interface IHandler : Control.IHandler
		{
			/// <summary>
			/// Gets or sets the selected object for the grid to edit
			/// </summary>
			object SelectedObject { get; set; }
			/// <summary>
			/// Gets or sets the selected objects for the grid to edit
			/// </summary>
			/// <remarks>
			/// Only common properties (with the same name and type) will be shown.
			/// </remarks>
			IEnumerable<object> SelectedObjects { get; set; }
			/// <summary>
			/// Gets or sets a value indicating that the categories should be shown
			/// </summary>
			bool ShowCategories { get; set; }
			/// <summary>
			/// Gets or sets a value indicating that the description panel should be shown
			/// </summary>
			/// <remarks>
			/// The description panel shows the name and description of the selected property
			/// </remarks>
			bool ShowDescription { get; set; }

			/// <summary>
			/// Refreshes the grid with new values from the selected object(s)
			/// </summary>
			void Refresh();
		}
	}
}
