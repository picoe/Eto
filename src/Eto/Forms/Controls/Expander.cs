namespace Eto.Forms;

/// <summary>
/// A control that provides a way to expand or collapse a panel. It includes a header and button.
/// </summary>
/// <example>
/// <code>
/// var myClosedExpander = new Expander
/// {
///		Header = "This is closed by default",
///		Content = "Content 1"
/// }
/// var myOpenExpander = new Expander
/// {
///		Header = "This is opened by default",
///		Expanded = true,
///		Content = "Content 2"
/// }
/// </code>
/// </example>
[Handler(typeof(IHandler))]
public class Expander : Panel
{
	new IHandler Handler { get { return (IHandler)base.Handler; } }

	/// <summary>
	/// Identifier for the <see cref="ExpandedChanged"/> event.
	/// </summary>
	public const string ExpandedChangedEvent = "Expander.ExpandedChanged";

	/// <summary>
	/// Event that occurs when the <see cref="Expanded"/> property changes.
	/// </summary>
	public event EventHandler<EventArgs> ExpandedChanged
	{
		add { Properties.AddHandlerEvent(ExpandedChangedEvent, value); }
		remove { Properties.RemoveEvent(ExpandedChangedEvent, value); }
	}

	/// <summary>
	/// Raises the <see cref="ExpandedChanged"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnExpandedChanged(EventArgs e)
	{
		Properties.TriggerEvent(ExpandedChangedEvent, this, e);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Expander"/> class.
	/// </summary>
	static Expander()
	{
		EventLookup.Register<Expander>(c => c.OnExpandedChanged(null), Expander.ExpandedChangedEvent);
	}

	/// <inheritdoc/>
	public override System.Collections.Generic.IEnumerable<Control> Controls
	{
		get
		{
			if (Header != null)
				return base.Controls.Union(new [] { Header });
			else
				return base.Controls;
		}
	}

	static Callback callback = new Callback();

	/// <inheritdoc/>
	protected override object GetCallback()
	{
		return callback;
	}

	/// <summary>
	/// Gets or sets a value indicating whether <see cref="Panel.Content"/> is currently expanded/visible.
	/// </summary>
	/// <value><see langword="true"/> if expanded; otherwise, <see langword="false"/>.</value>
	public bool Expanded
	{
		get { return Handler.Expanded; }
		set { Handler.Expanded = value; }
	}

	/// <summary>
	/// Gets or sets the header control.
	/// </summary>
	/// <value>The header control.</value>
	/// <remarks>Note, that there will always be a small button appearing in the header.</remarks>
	public Control Header
	{
		get { return Handler.Header; }
		set 
		{ 
			SetParent(value, () => Handler.Header = value, Handler.Header);
		}
	}

	/// <summary>
	/// Callback interface for <see cref="Expander"/>.
	/// </summary>
	public new interface ICallback : Panel.ICallback
	{
		/// <summary>
		/// Raises the <see cref="Expander.ExpandedChanged"/> event.
		/// </summary>
		/// <param name="widget">Widget to raise the event.</param>
		/// <param name="e">Event arguments.</param>
		void OnExpandedChanged(Expander widget, EventArgs e);
	}

	/// <summary>
	/// Callback implementation for handlers of <see cref="Expander"/>.
	/// </summary>
	protected new class Callback : Panel.Callback, ICallback
	{
		/// <inheritdoc cref="ICallback.OnExpandedChanged"/>
		public void OnExpandedChanged(Expander widget, EventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnExpandedChanged(e);
		}
	}

	/// <summary>
	/// Handler interface for platform implementations of <see cref="Expander"/>.
	/// </summary>
	public new interface IHandler : Panel.IHandler
	{
		/// <inheritdoc cref="Expander.Expanded"/>
		bool Expanded { get; set; }

		/// <inheritdoc cref="Expander.Header"/>
		Control Header { get; set; }
	}
}