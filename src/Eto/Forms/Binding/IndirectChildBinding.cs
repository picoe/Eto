using System;
using System.Reflection;

namespace Eto.Forms
{
	/// <summary>
	/// An indirect binding class to provide a way to bind to a child object of a value.
	/// </summary>
	/// <typeparam name="TParent">Property type from the parent object</typeparam>
	/// <typeparam name="TChild">Property type from the child object</typeparam>
	class IndirectChildBinding<TParent, TChild> : IndirectBinding<TChild>
	{
		IndirectBinding<TParent> _parent;
		IndirectBinding<TChild> _child;

		/// <summary>
		/// Initializes a new instance of the ChildBinding class
		/// </summary>
		/// <param name="parent">Binding from the parent to get the child</param>
		/// <param name="child">Binding for the child to get the actual value</param>
		public IndirectChildBinding(IndirectBinding<TParent> parent, IndirectBinding<TChild> child)
		{
			_parent = parent ?? throw new ArgumentNullException(nameof(parent));
			_child = child ?? throw new ArgumentNullException(nameof(child));
		}

		class BindingReference
		{
			public IndirectChildBinding<TParent, TChild> owner;
			public object parentReference;
			public object childReference;
			public EventHandler<EventArgs> handler;
			public object dataItem;

			public void ValueChanged(object sender, EventArgs e)
			{
				// remove binding from old child
				owner._child.RemoveValueChangedHandler(childReference, handler);

				// get new child and add binding 
				var newChild = owner._parent.GetValue(dataItem);
				childReference = owner._child.AddValueChangedHandler(newChild, handler);

				// now, fire the change event
				handler(sender, e);
			}
		}

		/// <inheritdoc/>
		public override object AddValueChangedHandler(object dataItem, EventHandler<EventArgs> handler)
		{
			var reference = new BindingReference();
			reference.owner = this;
			reference.dataItem = dataItem;
			reference.handler = handler;
			var childItem = _parent.GetValue(dataItem);
			reference.parentReference = _parent.AddValueChangedHandler(dataItem, reference.ValueChanged);
			reference.childReference = _child.AddValueChangedHandler(childItem, handler);
			return reference;
		}

		/// <inheritdoc/>
		public override void RemoveValueChangedHandler(object bindingReference, EventHandler<EventArgs> handler)
		{
			if (bindingReference is BindingReference reference)
			{
				_parent.RemoveValueChangedHandler(reference.parentReference, reference.ValueChanged);
				_child.RemoveValueChangedHandler(reference.childReference, handler);
			}
		}

		/// <inheritdoc/>
		protected override TChild InternalGetValue(object dataItem)
		{
			var childItem = _parent.GetValue(dataItem);
			return _child.GetValue(childItem);
		}

		bool? _isStruct;
		bool IsStruct => _isStruct ?? (_isStruct = typeof(TParent).GetTypeInfo().IsValueType).Value;

		/// <inheritdoc/>
		protected override void InternalSetValue(object dataItem, TChild value)
		{
			if (IsStruct)
			{
				// box value to object so it is passed by reference
				object childItem = _parent.GetValue(dataItem);
				// set the child value
				_child.SetValue(childItem, value);
				// set value back since it is a struct
				_parent.SetValue(dataItem, (TParent)childItem);
			}
			else
			{
				var childItem = _parent.GetValue(dataItem);
				_child.SetValue(childItem, value);
			}
		}
	}
}
