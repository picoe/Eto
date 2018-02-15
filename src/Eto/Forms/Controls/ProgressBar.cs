using System;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Control to show progress of a long running task
	/// </summary>
	/// <seealso cref="Spinner"/>
	[Handler(typeof(ProgressBar.IHandler))]
	public class ProgressBar : Control
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets or sets the value of the progress bar that represents 100% complete. The default is 100.
		/// </summary>
		/// <value>The maximum value.</value>
		[DefaultValue(100)]
		public int MaxValue
		{
			get { return Handler.MaxValue; }
			set { Handler.MaxValue = value; }
		}

		/// <summary>
		/// Gets or sets the minimum value of the progress that represents 0% complete. The default is 0.
		/// </summary>
		/// <value>The minimum value.</value>
		public int MinValue
		{
			get { return Handler.MinValue; }
			set { Handler.MinValue = value; }
		}

		/// <summary>
		/// Gets or sets the current progress that falls between <see cref="MinValue"/> and <see cref="MaxValue"/>
		/// </summary>
		/// <value>The value.</value>
		public int Value
		{
			get { return Handler.Value; }
			set { Handler.Value = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the progress is indeterminate
		/// </summary>
		/// <remarks>
		/// When indeterminate is true, the <see cref="MaxValue"/>/<see cref="MinValue"/>/<see cref="Value"/> are ignored
		/// and will typically show in a continuous style.
		/// </remarks>
		/// <value><c>true</c> if indeterminate; otherwise, <c>false</c>.</value>
		public bool Indeterminate
		{
			get { return Handler.Indeterminate; }
			set { Handler.Indeterminate = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="ProgressBar"/> control
		/// </summary>
		public new interface IHandler : Control.IHandler
		{
			/// <summary>
			/// Gets or sets the value of the progress bar that represents 100% complete. The default is 100.
			/// </summary>
			/// <value>The maximum value.</value>
			int MaxValue { get; set; }

			/// <summary>
			/// Gets or sets the minimum value of the progress that represents 0% complete. The default is 0.
			/// </summary>
			/// <value>The minimum value.</value>
			int MinValue { get; set; }

			/// <summary>
			/// Gets or sets the current progress that falls between <see cref="MinValue"/> and <see cref="MaxValue"/>
			/// </summary>
			/// <value>The value.</value>
			int Value { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the progress is indeterminate
			/// </summary>
			/// <remarks>
			/// When indeterminate is true, the <see cref="MaxValue"/>/<see cref="MinValue"/>/<see cref="Value"/> are ignored
			/// and will typically show in a continuous style.
			/// </remarks>
			/// <value><c>true</c> if indeterminate; otherwise, <c>false</c>.</value>
			bool Indeterminate { get; set; }
		}
	}
}

