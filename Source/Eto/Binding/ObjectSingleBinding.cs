using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	public class ObjectSingleBinding : ObjectBinding
	{
		object dataItem;
		object dataValueChangedReference;
		
		public override event EventHandler<BindingChangingEventArgs> Changing {
			add { InnerBinding.Changing += value; }
			remove { InnerBinding.Changing -= value; }
		}
		
		public override event EventHandler<BindingChangedEventArgs> Changed {
			add { InnerBinding.Changed += value; }
			remove { InnerBinding.Changed -= value; }
		}

		public SingleBinding InnerBinding { get; private set; }
		
		public override object DataItem {
			get { return dataItem; }
		}
		
		public ObjectSingleBinding (object dataItem, string property)
			: this(dataItem, new PropertyBinding(property))
		{
		}
		
		public ObjectSingleBinding (object dataItem, SingleBinding innerBinding)
		{
			this.dataItem = dataItem;
			this.InnerBinding = innerBinding;
		}
		
		public override object GetValue ()
		{
			return InnerBinding.GetValue (DataItem);
		}
		
		public override void SetValue (object value)
		{
			InnerBinding.SetValue (DataItem, value);
		}
		
		protected override void HandleEvent (string handler)
		{
			switch (handler) {
			case DataValueChangedEvent:
				if (dataValueChangedReference == null)
					dataValueChangedReference = InnerBinding.AddValueChangedHandler (
						DataItem,
						new EventHandler<EventArgs>(HandleChangedEvent)
					);
				break;
			default:
				base.HandleEvent (handler);
				break;
			}
		}
		
		protected override void RemoveEvent (string handler)
		{
			switch (handler) {
			case DataValueChangedEvent:
				if (dataValueChangedReference != null) {
					InnerBinding.RemoveValueChangedHandler (
						dataValueChangedReference,
						new EventHandler<EventArgs>(HandleChangedEvent)
					);
					dataValueChangedReference = null;
				}
				break;
			default:
				base.RemoveEvent (handler);
				break;
			}
		}
		
		public override void Unbind ()
		{
			base.Unbind ();

			RemoveEvent (DataValueChangedEvent);
		}
		
		void HandleChangedEvent (object sender, EventArgs e)
		{
			OnDataValueChanged (e);
		}
	}
}
