using System;
namespace Eto.Forms
{
	/// <summary>
	/// Extensions for widget objects
	/// </summary>
	public static class WidgetExtensions
	{
		/// <summary>
		/// Allows execution of extra code on a widget in a declarative manner.
		/// </summary>
		/// <typeparam name="T">Type of the widget</typeparam>
		/// <param name="widget">Widget to perform the action on</param>
		/// <param name="action">Action to execute on the widget before returning</param>
		/// <returns>Widget instance</returns>
		public static T With<T>(this T widget, Action<T> action)
			where T: Widget
		{
			action(widget);
			return widget;
		}
	}
}

