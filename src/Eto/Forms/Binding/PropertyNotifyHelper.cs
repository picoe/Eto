using System;
using System.Diagnostics;
using Eto.Forms;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Eto.Forms
{
	/// <summary>
	/// Helper to turn a property changed event to an EventHandler for binding
	/// </summary>
	/// <remarks>
	/// Use <see cref="Binding.AddPropertyEvent"/> and <see cref="Binding.RemovePropertyEvent"/> to access
	/// this functionality.
	/// </remarks>
	class PropertyNotifyHelper
	{
		public string PropertyName { get; private set; }

		public event EventHandler<EventArgs> Changed;

		public PropertyNotifyHelper(INotifyPropertyChanged obj, string propertyName)
		{
			PropertyName = propertyName;
			obj.PropertyChanged += obj_PropertyChanged;
		}

		public void Unregister(object obj)
		{
			var notifyObject = obj as INotifyPropertyChanged;
			if (notifyObject != null)
				notifyObject.PropertyChanged -= obj_PropertyChanged;
		}

		void obj_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == PropertyName)
			{
				if (Changed != null)
					Changed(sender, EventArgs.Empty);
			}
		}

	}
	
}
