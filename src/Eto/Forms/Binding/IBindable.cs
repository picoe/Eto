namespace Eto.Forms;

/// <summary>
/// Interface for objects that support binding
/// </summary>
public interface IBindable
{
	/// <summary>
	/// Gets or sets the data context for the widget for binding
	/// </summary>
	/// <remarks>
	/// Subclasses may override the standard behaviour so that hierarchy of widgets can be taken into account.
	/// 
	/// For example, a Control may return the data context of a parent, if it is not set explicitly.
	/// </remarks>
	object DataContext { get; set; }
		
	/// <summary>
	/// Gets a value indicating that the <see cref="DataContext"/> property is changing.
	/// </summary>
	/// <remarks>
	/// This can be used to determine when to allow certain logic during the update of the data context.
	/// 
	/// It is used to disable binding setters on the model when the data context changes so that a binding
	/// does not cause the view model to be updated when the state hasn't been fully set yet.
	/// </remarks>
	/// <value><c>true</c> if the DataContext is currently changing, <c>false</c> otherwise.</value>
	bool IsDataContextChanging { get; }

	/// <summary>
	/// Event to handle when the <see cref="DataContext"/> has changed
	/// </summary>
	/// <remarks>
	/// This may be fired in the event of a parent in the hierarchy setting the data context.
	/// For example, the <see cref="Forms.Container"/> widget fires this event when it's event is fired.
	/// </remarks>
	event EventHandler<EventArgs> DataContextChanged;

	/// <summary>
	/// Gets the collection of bindings that are attached to this widget
	/// </summary>
	BindingCollection Bindings { get; }
}