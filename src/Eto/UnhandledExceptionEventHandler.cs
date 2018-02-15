using System;

namespace Eto
{
	/// <summary>
	/// Provides data for the event that is raised when there is an exception that is not handled otherwise
	/// </summary>
	/// <remarks>There is a System.UnhandledExceptionEventArgs class, but that doesn't seem to be available in
	/// the Core CLR, therefore we redefine it here.</remarks>
	public class UnhandledExceptionEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.UnhandledExceptionEventArgs"/> class.
		/// </summary>
		/// <param name="exception">The exception object.</param>
		/// <param name="isTerminating"><c>true</c> if the application will terminate, otherwise <c>false</c>.</param>
		public UnhandledExceptionEventArgs(object exception, bool isTerminating)
		{
			ExceptionObject = exception;
			IsTerminating = isTerminating;
		}

		/// <summary>
		/// Gets the unhandled exception object.
		/// </summary>
		public object ExceptionObject { get; private set; }

		/// <summary>
		/// Indicates whether the application is terminating.
		/// </summary>
		public bool IsTerminating { get; private set; }
	}
}
