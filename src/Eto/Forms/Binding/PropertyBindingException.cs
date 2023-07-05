namespace Eto.Forms;

/// <summary>
/// Exception when getting/setting values in a <see cref="PropertyBinding{T}" />
/// </summary>
/// <remarks>
/// This exception is thrown explicitly if there is a problem getting or setting the value on the data item.
/// Since using descriptors can sometimes bury the actual stack trace, this can be useful to figure out what property
/// setter/getter is throwing.
/// </remarks>
[System.Serializable]
public class PropertyBindingException : System.Exception
{
	/// <summary>
	/// Initializes a new instance of the PropertyBindingException class.
	/// </summary>
	public PropertyBindingException() { }
	/// <summary>
	/// Initializes a new instance of the PropertyBindingException class with the specified message.
	/// </summary>
	/// <param name="message">Message of the exception</param>
	public PropertyBindingException(string message) : base(message) { }
	/// <summary>
	/// Initializes a new instance of the PropertyBindingException class with the specified message and inner exception.
	/// </summary>
	/// <param name="message">Message of the exception</param>
	/// <param name="inner">Original exception</param>
	public PropertyBindingException(string message, System.Exception inner) : base(message, inner) { }
	/// <summary>
	/// Initializes a new instance of the PropertyBindingException class from serialization.
	/// </summary>
	/// <param name="info">Serialization info</param>
	/// <param name="context">Streaming context</param>
	protected PropertyBindingException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
