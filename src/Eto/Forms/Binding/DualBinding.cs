using System;

namespace Eto.Forms
{
	/// <summary>
	/// Mode of the <see cref="DualBinding{T}"/>
	/// </summary>
	/// <remarks>
	/// This specifies what direction the updates of each of the properties are automatically handled.
	/// Only properties that have a Changed event, or objects that implement <see cref="System.ComponentModel.INotifyPropertyChanged"/>
	/// will handle automatically updating the binding.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum DualBindingMode
	{
		/// <summary>
		/// Binding will update the destination if the source property is changed
		/// </summary>
		OneWay,

		/// <summary>
		/// Binding will update both the destination or source if updated on either the source or destination, respectively
		/// </summary>
		TwoWay,

		/// <summary>
		/// Binding will update the source if the destination property is changed
		/// </summary>
		OneWayToSource,

		/// <summary>
		/// Binding will only set the destination from the source when initially bound
		/// </summary>
		/// <remarks>
		/// This is ideal when you want to set the values of the destination, then only update the source
		/// at certain times using the <see cref="DualBinding{T}.Update"/> method.
		/// </remarks>
		OneTime,

		/// <summary>
		/// Binding will only update when the <see cref="Binding.Update"/> method is called.
		/// </summary>
		Manual
	}

	/// <summary>
	/// Binding for joining two object bindings together
	/// </summary>
	/// <remarks>
	/// The DualBinding is the most useful binding, as it allows you to bind two objects together.
	/// This differs from the <see cref="IndirectBinding{T}"/> where it only specifies how to get/set the value from a single object.
	/// 
	/// </remarks>
	public class DualBinding<T> : Binding
	{
		bool channeling;
		DualBindingMode mode;

		/// <summary>
		/// Gets the source binding
		/// </summary>
		public DirectBinding<T> Source { get; private set; }

		/// <summary>
		/// Gets the destination binding
		/// </summary>
		public DirectBinding<T> Destination { get; private set; }

		/// <summary>
		/// Gets the mode of the binding
		/// </summary>
		public DualBindingMode Mode
		{
			get { return mode; }
			set
			{
				if (value != mode)
				{
					ClearBinding();
					var setInitialValue = mode == DualBindingMode.Manual;
					mode = value;
					SetBinding(setInitialValue);
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the DualBinding class with two object property bindings
		/// </summary>
		/// <param name="source">Object to retrieve the source value from</param>
		/// <param name="sourceProperty">Property to retrieve from the source</param>
		/// <param name="destination">Object to set the destination value to</param>
		/// <param name="destinationProperty">Property to set on the destination</param>
		/// <param name="mode">Mode of the binding</param>
		public DualBinding(object source, string sourceProperty, object destination, string destinationProperty, DualBindingMode mode = DualBindingMode.TwoWay)
			: this(
				new ObjectBinding<object, T>(source, sourceProperty),
				new ObjectBinding<object, T>(destination, destinationProperty),
				mode
				)
		{
		}

		/// <summary>
		/// Initializes a new instance of the DualBinding class with two specified bindings
		/// </summary>
		/// <param name="source">Binding for retrieving the source value from</param>
		/// <param name="destination">Binding for setting the destination value to</param>
		/// <param name="mode">Mode of the binding</param>
		public DualBinding(DirectBinding<T> source, DirectBinding<T> destination, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			Source = source;
			Destination = destination;
			this.mode = mode;

			SetBinding(true);
		}

		void ClearBinding()
		{
			if (mode == DualBindingMode.OneWay || mode == DualBindingMode.TwoWay)
				Source.DataValueChanged -= HandleSourceChanged;
			if (mode == DualBindingMode.OneWayToSource || mode == DualBindingMode.TwoWay)
				Destination.DataValueChanged -= HandleDestinationChanged;
		}

		void SetBinding(bool setInitialValue)
		{
			if (mode == DualBindingMode.OneWay || mode == DualBindingMode.TwoWay)
				Source.DataValueChanged += HandleSourceChanged;
			if (mode == DualBindingMode.OneWayToSource || mode == DualBindingMode.TwoWay)
				Destination.DataValueChanged += HandleDestinationChanged;


			if (setInitialValue)
			{
				switch (mode)
				{
					case DualBindingMode.OneTime:
					case DualBindingMode.OneWay:
					case DualBindingMode.TwoWay:
						SetDestination();
						break;
					case DualBindingMode.OneWayToSource:
						SetSource();
						break;
				}
			}
		}

		void HandleSourceChanged(object sender, EventArgs e)
		{
			SetDestination();
		}

		void HandleDestinationChanged(object sender, EventArgs e)
		{
			SetSource();
		}

		/// <summary>
		/// Sets the source object's property with the value of the destination
		/// </summary>
		public void SetSource()
		{
			if (!channeling)
			{
				channeling = true;
				var value = Destination.DataValue;
                var args = new BindingChangingEventArgs(value);
				OnChanging(args);
				if (!args.Cancel)
				{
					Source.DataValue = value;
					OnChanged(new BindingChangedEventArgs(value));
				}
				channeling = false;
			}
		}

		/// <summary>
		/// Sets the destination object's property with the value of the source
		/// </summary>
		public void SetDestination()
		{
			if (!channeling)
			{
				channeling = true;
				var value = Source.DataValue;
				var args = new BindingChangingEventArgs(value);
				OnChanging(args);
				if (!args.Cancel)
				{
					Destination.DataValue = value;
					OnChanged(new BindingChangedEventArgs(value));
				}
				channeling = false;
			}
		}

		/// <summary>
		/// Updates the binding value (sets the source with the value of the destination)
		/// </summary>
		public override void Update(BindingUpdateMode mode = BindingUpdateMode.Destination)
		{
			base.Update(mode);

			if (mode == BindingUpdateMode.Source)
				SetSource();
			else
				SetDestination();
		}

		/// <summary>
		/// Unbinds both the source and destination bindings
		/// </summary>
		public override void Unbind()
		{
			base.Unbind();

			Source.Unbind();
			Destination.Unbind();
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Eto.Forms.DualBinding`1"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Eto.Forms.DualBinding`1"/>.</returns>
		public override string ToString()
		{
			return $"DualBinding: {Source} <-> {Destination}";
		}
	}
}
