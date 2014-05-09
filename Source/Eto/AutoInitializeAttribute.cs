using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Eto
{
	/// <summary>
	/// Attribute to specify whether the handler interface should be initialized automatically
	/// </summary>
	/// <remarks>
	/// Handler interfaces that defer creation of the control to various Create() methods can apply this attribute
	/// so that the initialization can be done afterwards.
	/// 
	/// If auto initialization is disabled, the widget author must call Widget.Initialize() after the control is created.
	/// 
	/// Initialization applies styles to the widget and the handler, sets up events based on overridden event methods, etc.
	/// 
	/// This is only needed by widget authors in advanced scenarios.  The default is to auto initialize, so this is
	/// only needed if you want to disable this behaviour.
	/// </remarks>
	/// <example>
	/// An example handler that implements this behaviour:
	/// <code>
	/// [AutoInitialize(false)]
	/// public interface IMyWidget : IControl
	/// {
	/// 	void CreateWithParams(int param1, bool param2);
	/// }
	/// 
	/// [Handler(typeof(IMyWidget))]
	/// public class MyWidget : Control
	/// {
	/// 	public MyWidget(int param1, bool param2)
	/// 	{
	/// 		((IMyWidget)Handler).CreateWithParams(param1, param2);
	/// 		Initialize(); // ensure you call this on any constructor of your widget
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class AutoInitializeAttribute : Attribute
	{
		/// <summary>
		/// Gets a value indicating whether to auto initialize the handler, false to defer this to the widget author
		/// </summary>
		/// <value><c>true</c> if initialize; otherwise, <c>false</c>.</value>
		public bool Initialize { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.AutoInitializeAttribute"/> class.
		/// </summary>
		/// <param name="initialize">If set to <c>true</c> initialize the widget automatically, otherwise <c>false</c>.</param>
		public AutoInitializeAttribute(bool initialize)
		{
			Initialize = initialize;
		}
	}
}
