using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	public enum DualBindingMode
	{
		OneWay,
		TwoWay,
		OneWayToSource,
		OneTime
	}
	
	public class DualBinding : Binding
	{
		public ObjectBinding Source { get; private set; }

		public ObjectBinding Destination { get; private set; }

		bool channeling;
		
		public DualBindingMode Mode { get; private set; }
		
		public DualBinding (object source, string sourceProperty, object destination, string destinationProperty, DualBindingMode mode = DualBindingMode.TwoWay)
			: this (
				new ObjectSingleBinding(source, sourceProperty),
				new ObjectSingleBinding(destination, destinationProperty),
				mode
				)
		{
		}
		
		public DualBinding (ObjectBinding source, ObjectBinding destination, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			this.Source = source;
			this.Destination = destination;
			
			if (mode == DualBindingMode.OneWay || mode == DualBindingMode.TwoWay)
				source.DataValueChanged += HandleSourceChanged;
			if (mode == DualBindingMode.OneWayToSource || mode == DualBindingMode.TwoWay)
				destination.DataValueChanged += HandleDestinationChanged;

			// set initial value
			this.SetDestination ();
		}
		
		void HandleSourceChanged (object sender, EventArgs e)
		{
			SetDestination ();
		}

		void HandleDestinationChanged (object sender, EventArgs e)
		{
			SetSource ();
		}
		
		public void SetSource ()
		{
			if (!channeling) {
				channeling = true;
				Source.SetValue (Destination.GetValue ());
				channeling = false;
			}
		}

		public void SetDestination ()
		{
			if (!channeling) {
				channeling = true;
				Destination.SetValue (Source.GetValue ());
				channeling = false;
			}
		}
		
		public override void Update ()
		{
			base.Update ();
			
			SetSource ();
		}
		
		public override void Unbind ()
		{
			base.Unbind ();
			
			Source.Unbind ();
			Destination.Unbind ();
		}
	}
}
