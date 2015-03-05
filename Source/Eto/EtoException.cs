using System;

namespace Eto
{
	/// <summary>
	/// General exception for errors in the Eto framework
	/// </summary>
	[Serializable]
	public class EtoException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:EtoException"/> class
		/// </summary>
		public EtoException ()
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:EtoException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public EtoException (string message) : base (message)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:EtoException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public EtoException (string message, Exception inner) : base (message, inner)
		{
		}
	}
}
