using System;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Non-modal form window
	/// </summary>
	/// <seealso cref="Dialog"/>
	[Handler(typeof(Form.IHandler))]
	public class Form : Window
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Form"/> class.
		/// </summary>
		public Form()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Form"/> class with the specified <paramref name="handler"/>
		/// </summary>
		/// <param name="handler">Handler to use as the implementation of the form.</param>
		public Form(IHandler handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Gets or sets a value indicating that the form should be activated when initially shown.
		/// </summary>
		/// <remarks>
		/// When <c>true</c>, the form will become the active/focussed window when the <see cref="Show"/> method is called.
		/// When <c>false</c>, the form will show but will not get focus until the user clicks on the form.
		/// </remarks>
		/// <seealso cref="CanFocus"/>
		[DefaultValue(true)]
		public bool ShowActivated
		{
			get { return Handler.ShowActivated; }
			set { Handler.ShowActivated = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating that this form can get keyboard/input focus when the user clicks on it or any child control.
		/// </summary>
		/// <remarks>
		/// This is useful for windows that provide interaction but do not steal focus from the current window, such as a tooltip, popover, etc.
		/// </remarks>
		/// <value><c>true</c> if the form can get focus; otherwise, <c>false</c>.</value>
		/// <seealso cref="ShowActivated"/>
		[DefaultValue(true)]
		public bool CanFocus
		{
			get { return Handler.CanFocus; }
			set { Handler.CanFocus = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Control"/> is visible to the user.
		/// </summary>
		/// <remarks>
		/// When the visibility of a control is set to false, it will still occupy space in the layout, but not be shown.
		/// The only exception is for controls like the <see cref="Splitter"/>, which will hide a pane if the visibility
		/// of one of the panels is changed.
		/// </remarks>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public override bool Visible
		{
			get => base.Visible;
			set
			{
				// when setting Visible = true for the first time it should be like calling Show()
				if (value && !Loaded)
					Show();
				else
					base.Visible = value;
			}
		}

		/// <summary>
		/// Show the form
		/// </summary>
		public void Show()
		{
			var loaded = Loaded;
			if (!loaded)
			{
				OnPreLoad(EventArgs.Empty);
				OnLoad(EventArgs.Empty);
				OnLoadComplete(EventArgs.Empty);

				Application.Instance.AddWindow(this);
			}

			Handler.Show();
		}

		/// <summary>
		/// Interface handler for the <see cref="Form"/> control
		/// </summary>
		public new interface IHandler : Window.IHandler
		{
			/// <summary>
			/// Show the form
			/// </summary>
			void Show();

			/// <summary>
			/// Gets or sets a value indicating that the form should be activated when initially shown.
			/// </summary>
			/// <remarks>
			/// When <c>true</c>, the form will become the active/focussed window when the <see cref="Show"/> method is called.
			/// When <c>false</c>, the form will show but will not get focus until the user clicks on the form.
			/// </remarks>
			/// <seealso cref="CanFocus"/>
			bool ShowActivated { get; set; }

			/// <summary>
			/// Gets or sets a value indicating that this form can get keyboard/input focus when the user clicks on it or any child control.
			/// </summary>
			/// <remarks>
			/// This is useful for windows that provide interaction but do not steal focus from the current window, such as a tooltip, popover, etc.
			/// </remarks>
			/// <value><c>true</c> if the form can get focus; otherwise, <c>false</c>.</value>
			/// <seealso cref="ShowActivated"/>
			bool CanFocus { get; set; }
		}
	}
}
