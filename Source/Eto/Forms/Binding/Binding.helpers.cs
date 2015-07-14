using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Eto.Forms;
using System.Windows.Input;

namespace Eto.Forms
{
	partial class Binding
	{
		/// <summary>
		/// Creates a new indirect delegate binding.
		/// </summary>
		/// <param name="getValue">Delegate to get the value for the binding.</param>
		/// <param name="setValue">Delegate to set the value for the binding.</param>
		/// <param name="addChangeEvent">Delegate to register the change event, when needed by the consumer of this binding.</param>
		/// <param name="removeChangeEvent">Delegate to remove the change event.</param>
		/// <param name="defaultGetValue">Default get value, when the object instance is null.</param>
		/// <param name="defaultSetValue">Default set value, when the incoming value is null.</param>
		public static IndirectBinding<TValue> Delegate<T, TValue>(Func<T, TValue> getValue, Action<T, TValue> setValue = null, Action<T, EventHandler<EventArgs>> addChangeEvent = null, Action<T, EventHandler<EventArgs>> removeChangeEvent = null, TValue defaultGetValue = default(TValue), TValue defaultSetValue = default(TValue))
		{
			return new DelegateBinding<T, TValue>(getValue, setValue, addChangeEvent, removeChangeEvent, defaultGetValue, defaultSetValue);
		}

		/// <summary>
		/// Creates a new direct delegate binding.
		/// </summary>
		/// <param name="getValue">Delegate to get the value for the binding.</param>
		/// <param name="setValue">Delegate to set the value for the binding.</param>
		/// <param name="addChangeEvent">Delegate to register the change event, when needed by the consumer of this binding.</param>
		/// <param name="removeChangeEvent">Delegate to remove the change event.</param>
		public static DirectBinding<TValue> Delegate<TValue>(Func<TValue> getValue, Action<TValue> setValue = null, Action<EventHandler<EventArgs>> addChangeEvent = null, Action<EventHandler<EventArgs>> removeChangeEvent = null)
		{
			return new DelegateBinding<TValue>(getValue, setValue, addChangeEvent, removeChangeEvent);
		}

		/// <summary>
		/// Creates a new direct property binding to the specified <paramref name="model"/> object.
		/// </summary>
		/// <param name="model">Model object to bind to.</param>
		/// <param name="propertyExpression">Expression to the property of the model object.</param>
		/// <typeparam name="T">The model type.</typeparam>
		/// <typeparam name="TValue">The property value type.</typeparam>
		public static DirectBinding<TValue> Property<T, TValue>(T model, Expression<Func<T, TValue>> propertyExpression)
		{
			var propertyInfo = propertyExpression.GetMemberInfo();
			if (propertyInfo != null)
			{
				return new ObjectBinding<T, TValue>(model, new PropertyBinding<TValue>(propertyInfo.Member.Name));
			}
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Must be a expression to a property"), "propertyExpression");
		}

		/// <summary>
		/// Creates a new indirect property binding using the specified <paramref name="propertyExpression"/>.
		/// </summary>
		/// <param name="propertyExpression">Expression of the property to bind to.</param>
		/// <typeparam name="T">The type of the model.</typeparam>
		/// <typeparam name="TValue">The property value type.</typeparam>
		public static IndirectBinding<TValue> Property<T, TValue>(Expression<Func<T, TValue>> propertyExpression)
		{
			var propertyInfo = propertyExpression.GetMemberInfo();
			if (propertyInfo != null)
			{
				return new PropertyBinding<TValue>(propertyInfo.Member.Name);
			}
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Must be a expression to a property"), "propertyExpression");
		}

		/// <summary>
		/// Creates a new indirect property binding using the specified <paramref name="propertyName"/>.
		/// </summary>
		/// <param name="propertyName">Name of the property to bind to.</param>
		/// <typeparam name="TValue">The type of the property.</typeparam>
		public static IndirectBinding<TValue> Property<TValue>(string propertyName)
		{
			return new PropertyBinding<TValue>(propertyName);
		}

		/// <summary>
		/// Adds an event handler for a specified <paramref name="propertyName"/> of a <see cref="INotifyPropertyChanged"/> object.
		/// </summary>
		/// <remarks>
		/// This can be used to translate an INotifyPropertyChanged event for a particular property to a single event.
		/// Typically, this would be used when creating a <see cref="DelegateBinding{T,TValue}"/> to attach to property notified
		/// events instead of singular events.
		/// </remarks>
		/// <param name="obj">INotifyPropertyChanged object to attach the event handler to</param>
		/// <param name="propertyName">Name of the property to trigger the changed event.</param>
		/// <param name="eh">Event handler delegate to trigger when the specified property changes</param>
		/// <seealso cref="RemovePropertyEvent"/>
		public static void AddPropertyEvent(object obj, string propertyName, EventHandler<EventArgs> eh)
		{
			var notifyObject = obj as INotifyPropertyChanged;
			if (notifyObject != null)
				new PropertyNotifyHelper(notifyObject, propertyName).Changed += eh;
		}

		/// <summary>
		/// Adds an event handler for a specified <paramref name="propertyExpression"/> of a <see cref="INotifyPropertyChanged"/> object.
		/// </summary>
		/// <remarks>
		/// This can be used to translate an INotifyPropertyChanged event for a particular property to a single event.
		/// Typically, this would be used when creating a <see cref="DelegateBinding{T,TValue}"/> to attach to property notified
		/// events instead of singular events.
		/// </remarks>
		/// <param name="obj">INotifyPropertyChanged object to attach the event handler to</param>
		/// <param name="propertyExpression">Expression to the property to trigger the changed event.</param>
		/// <param name="eh">Event handler delegate to trigger when the specified property changes</param>
		/// <seealso cref="RemovePropertyEvent"/>
		public static void AddPropertyEvent<T, TProperty>(T obj, Expression<Func<T, TProperty>> propertyExpression, EventHandler<EventArgs> eh)
		{
			var notifyObject = obj as INotifyPropertyChanged;
			if (notifyObject != null)
			{
				var propertyInfo = propertyExpression.GetMemberInfo();
				if (propertyInfo != null)
					new PropertyNotifyHelper(notifyObject, propertyInfo.Member.Name).Changed += eh;
			}
		}

		/// <summary>
		/// Removes an event handler previously attached with the AddPropertyEvent method.
		/// </summary>
		/// <param name="obj">INotifyPropertyChanged object to remove the event handler from</param>
		/// <param name="eh">Event handler delegate to remove</param>
		/// <seealso cref="AddPropertyEvent(object,string,EventHandler{EventArgs})"/>
		public static void RemovePropertyEvent(object obj, EventHandler<EventArgs> eh)
		{
			var helper = eh.Target as PropertyNotifyHelper;
			if (helper != null)
				helper.Unregister(obj);
		}

		/// <summary>
		/// Executes a command retrieved using the specified <paramref name="commandBinding"/> from the <paramref name="dataContext"/>.
		/// </summary>
		/// <remarks>
		/// This helper method is useful for binding general events to fire an <see cref="ICommand"/> that is in your view model.
		/// The command will only be executed if its <see cref="ICommand.CanExecute"/> returns <c>true</c>.
		/// 
		/// Most controls (e.g. <see cref="Eto.Forms.Button"/>) have a special Command parameter that can be set instead, 
		/// which takes into account the enabled state of the command and will enable/disable the control automatically.
		/// </remarks>
		/// <example>
		/// This example will fire the MyModel.MyCommand when the mouse is down on the specified panel.
		/// The MyModel instance is based off the panel's current DataContext.
		/// <code>
		/// var panel = new Panel();
		/// panel.MouseDown += (sender, e) => Binding.ExecuteCommand(panel.DataContext, Binding.Property((MyModel m) => m.MyCommand));
		/// </code>
		/// </example>
		/// <param name="dataContext">Data context object to get the ICommand via the commandBinding.</param>
		/// <param name="commandBinding">Binding to get the ICommand from the data context</param>
		/// <param name="parameter">Parameter to pass to the command when executing or checking if it can execute.</param>
		public static void ExecuteCommand(object dataContext, IndirectBinding<ICommand> commandBinding, object parameter = null)
		{
			var command = commandBinding.GetValue(dataContext);
			if (command != null && command.CanExecute(parameter))
				command.Execute(parameter);
		}

		/// <summary>
		/// Executes a command retrieved using a property <paramref name="commandExpression"/> from the  <paramref name="dataContext"/>.
		/// </summary>
		/// <remarks>
		/// This helper method is useful for binding general events to fire an <see cref="ICommand"/> that is in your view model.
		/// The command will only be executed if its <see cref="ICommand.CanExecute"/> returns <c>true</c>.
		/// 
		/// Most controls (e.g. <see cref="Eto.Forms.Button"/>) have a special Command parameter that can be set instead, 
		/// which takes into account the enabled state of the command and will enable/disable the control automatically.
		/// </remarks>
		/// <example>
		/// This example will fire the MyModel.MyCommand when the mouse is down on the specified panel.
		/// The MyModel instance is based off the panel's current DataContext.
		/// <code>
		/// var panel = new Panel();
		/// panel.MouseDown += (sender, e) => Binding.ExecuteCommand(panel.DataContext, (MyModel m) => m.MyCommand);
		/// </code>
		/// </example>
		/// <param name="dataContext">Data context object to get the ICommand via the commandBinding.</param>
		/// <param name="commandExpression">Property expression to get the ICommand from the data context</param>
		/// <param name="parameter">Parameter to pass to the command when executing or checking if it can execute.</param>
		public static void ExecuteCommand<T>(object dataContext, Expression<Func<T, ICommand>> commandExpression, object parameter = null)
		{
			ExecuteCommand(dataContext, Binding.Property(commandExpression), parameter);
		}
	}
}
