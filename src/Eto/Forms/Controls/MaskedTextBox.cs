using System;
using System.Text;
using System.Linq;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;


namespace Eto.Forms
{
	/// <summary>
	/// Mode for insertion of text when the user types into a control.
	/// </summary>
	public enum InsertKeyMode
	{
		/// <summary>
		/// Always insert, shifting any characters to the right of the caret position when inserting or deleting text.
		/// </summary>
		Insert,
		/// <summary>
		/// Always overwrite and do not shift characters when inserting or deleting text.
		/// </summary>
		Overwrite,
		/// <summary>
		/// Allow the user to toggle the insert mode (fn+Return on OS X or insert key on other keyboards)
		/// </summary>
		Toggle
	}

	/// <summary>
	/// Mode for when prompt characters are shown in a control.
	/// </summary>
	public enum ShowPromptMode
	{
		/// <summary>
		/// Always show the prompt characters.
		/// </summary>
		Always,
		/// <summary>
		/// Only show the prompt characters when the control has focus.
		/// </summary>
		OnFocus,
		/// <summary>
		/// Never show the prompt characters
		/// </summary>
		Never
	}

	/// <summary>
	/// Masked text box with a variable length numeric mask.
	/// </summary>
	/// <remarks>
	/// This provides a text box that limits the user input to only allow numeric values.
	/// </remarks>
	/// <typeparam name="T">Numeric type such as int, decimal, double, etc.</typeparam>
	public class NumericMaskedTextBox<T> : MaskedTextBox<T>
	{
		/// <summary>
		/// Gets the numeric provider.
		/// </summary>
		/// <value>The masked text provider.</value>
		public new NumericMaskedTextProvider<T> Provider
		{
			get { return (NumericMaskedTextProvider<T>)base.Provider; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the mask can accept a sign.
		/// </summary>
		/// <remarks>
		/// This defaults to whether the type specified by <typeparamref name="T"/> allows negative values.
		/// </remarks>
		/// <value><c>true</c> to allow sign character; otherwise, <c>false</c>.</value>
		public bool AllowSign
		{ 
			get { return Provider.AllowSign; }
			set { Provider.AllowSign = value; } 
		}

		/// <summary>
		/// Gets or sets a value indicating whether the mask can input a decimal.
		/// </summary>
		/// <remarks>
		/// This defaults to whether the type specified by <typeparamref name="T"/> allows decimals, such as when
		/// it is a decimal, double, or float.
		/// </remarks>
		/// <value><c>true</c> to allow decimal; otherwise, <c>false</c>.</value>
		public bool AllowDecimal
		{
			get { return Provider.AllowDecimal; }
			set { Provider.AllowDecimal = value; }
		}

		/// <summary>
		/// Gets or sets the culture for the <see cref="NumericMaskedTextProvider.DecimalCharacter"/> and <see cref="NumericMaskedTextProvider.SignCharacters"/> formatting characters.
		/// </summary>
		public CultureInfo Culture
		{
			get => Provider.Culture;
			set
			{
				Provider.Culture = value;
				UpdateText();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.NumericMaskedTextBox{T}"/> class.
		/// </summary>
		public NumericMaskedTextBox()
			: base(new NumericMaskedTextProvider<T>())
		{
		}

		/// <inheritdoc/>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.KeyData == Keys.Decimal)
			{
				var pos = CaretIndex;
				Provider.Insert(Provider.DecimalCharacter, ref pos);
				UpdateText();
				CaretIndex = pos;
				e.Handled = true;
			}
		}
	}

	/// <summary>
	/// Masked text box that provides a value converted to/from text
	/// </summary>
	/// <remarks>
	/// This is useful when the text can be converted to another type (e.g. DateTime, numeric, etc).
	/// 
	/// The <see cref="Provider"/> specified for the control is responsible for converting the value.
	/// </remarks>
	public class MaskedTextBox<T> : MaskedTextBox
	{
		/// <summary>
		/// Event to handle when the <see cref="Value"/> property changes
		/// </summary>
		public event EventHandler<EventArgs> ValueChanged
		{
			add { TextChanged += value; }
			remove { TextChanged -= value; }
		}

		/// <summary>
		/// Gets or sets the provider for the text box
		/// </summary>
		/// <value>The provider.</value>
		public new IMaskedTextProvider<T> Provider
		{
			get { return base.Provider as IMaskedTextProvider<T>; }
			set { base.Provider = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.MaskedTextBox{T}"/> class.
		/// </summary>
		public MaskedTextBox()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.MaskedTextBox{T}"/> class with the specified masked text provider.
		/// </summary>
		/// <param name="provider">Masked text provider to format the mask.</param>
		public MaskedTextBox(IMaskedTextProvider<T> provider)
			: base(provider)
		{
		}

		/// <summary>
		/// Gets or sets the translated value of the masked text.
		/// </summary>
		/// <value>The translated value.</value>
		public T Value
		{
			get { return Provider != null ? Provider.Value : default(T); }
			set
			{
				if (Provider != null)
					Provider.Value = value;
				UpdateText();
			}
		}

		/// <summary>
		/// Gets a binding for the <see cref="Value"/> property.
		/// </summary>
		/// <value>The value binding.</value>
		public BindableBinding<MaskedTextBox<T>, T> ValueBinding
		{
			get
			{
				return new BindableBinding<MaskedTextBox<T>, T>(
					this,
					c => c.Value,
					(c, v) => c.Value = v,
					(c, eh) => c.ValueChanged += eh,
					(c, eh) => c.ValueChanged -= eh
				);
			}
		}
	}

	/// <summary>
	/// Text box with masking capabilities.
	/// </summary>
	/// <remarks>
	/// This uses the <see cref="IMaskedTextProvider"/> as its interface to the mask.  
	/// The mask can implement any format it wishes, including both fixed or variable length masks.
	/// The MaskedTextBox allows you to mask, or limit which characters can be entered in the text box with either a fixed, variable, or custom mask.
	/// A fixed mask can be a phone number, postal code, or something that requires a specific format and can be created using the <see cref="FixedMaskedTextProvider"/>. 
	/// A variable mask limits which characters can be entered but is not limited to a fixed number of characters.
	/// An implementation of a variable mask is the <see cref="NumericMaskedTextBox{T}"/> which allows you to enter only numeric values in a text box, and places the positive / negative symbol at the beginning regardless of where you type it.
	/// </remarks>
	[ContentProperty("Provider")]
	public class MaskedTextBox : TextBox
	{
		IMaskedTextProvider provider;
		static readonly object InsertKeyModeKey = new object();
		static readonly object ShowPromptModeKey = new object();
		static readonly object SupportsInsertKey = new object();
		static readonly object OverwriteModeKey = new object();
		static readonly object ShowPlaceholderWhenEmptyKey = new object();
		static readonly object IsUpdatingTextKey = new object();

		/// <summary>
		/// Gets a cached value indicating the current platform supports getting the insert mode state.
		/// </summary>
		static bool SupportsInsert
		{
			get
			{
				// cache whether the platform supports the insert key for Keyboard.IsKeyLocked
				return Platform.Instance.GetSharedProperty(SupportsInsertKey, () => Keyboard.SupportedLockKeys.Contains(Keys.Insert));
			}
		}

		/// <summary>
		/// If the platform doesn't support global insert/overwrite mode, this stores an application-wide state of the insert mode
		/// </summary>
		static bool ManualOverwriteMode
		{
			get { return Platform.Instance.GetSharedProperty(OverwriteModeKey, () => false); }
			set { Platform.Instance.SetSharedProperty(OverwriteModeKey, value); }
		}

		/// <summary>
		/// Gets or sets the masked text provider to specify the mask format.
		/// </summary>
		/// <value>The masked text provider.</value>
		public IMaskedTextProvider Provider
		{
			get { return provider; }
			set
			{
				if (!ReferenceEquals(value, provider))
				{
					var oldProvider = provider;
					provider = value;
					if (provider != null && oldProvider != null)
						provider.Text = oldProvider.Text;
					UpdateText();
				}
			}
		}

		/// <summary>
		/// Gets or sets the mode for insertion. Use <see cref="IsOverwrite"/> to determine the current mode.
		/// </summary>
		/// <value>The desired insert mode.</value>
		public InsertKeyMode InsertMode
		{
			get { return Properties.Get<InsertKeyMode?>(InsertKeyModeKey) ?? InsertKeyMode.Insert; }
			set { Properties[InsertKeyModeKey] = value; }
		}

		/// <summary>
		/// Gets a value indicating whether typing will overwrite text.
		/// </summary>
		/// <seealso cref="InsertMode"/> 
		/// <value><c>true</c> if text will be overwritten; otherwise, <c>false</c> to insert text.</value>
		public bool IsOverwrite
		{
			get
			{
				if (InsertMode == InsertKeyMode.Overwrite)
					return true;
				var overwrite = SupportsInsert ? Keyboard.IsKeyLocked(Keys.Insert) : ManualOverwriteMode;
				return InsertMode == InsertKeyMode.Toggle && overwrite; 
			}
		}

		/// <summary>
		/// Gets or sets a value indicating that the prompt characters should only be shown when the control has focus.
		/// </summary>
		/// <value><c>true</c> if to show the prompt only when focussed; otherwise, <c>false</c>.</value>
		[Obsolete("Since 2.5.1, Use ShowPromptMode instead")]
		public bool ShowPromptOnFocus
		{
			get => ShowPromptMode == ShowPromptMode.OnFocus;
			set => ShowPromptMode = value ? ShowPromptMode.OnFocus : ShowPromptMode.Always;
		}

		/// <summary>
		/// Gets or sets the mode for when the input prompts should be shown
		/// </summary>
		public ShowPromptMode ShowPromptMode
		{
			get => Properties.Get<ShowPromptMode>(ShowPromptModeKey);
			set
			{ 
				if (Properties.TrySet(ShowPromptModeKey, value))
				{
					UpdateText();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating that the placeholder should be shown when the mask is empty and the control does
		/// not have focus.
		/// </summary>
		/// <value><c>true</c> to show the placeholder when its value is empty; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool ShowPlaceholderWhenEmpty
		{
			get { return Properties.Get<bool?>(ShowPlaceholderWhenEmptyKey) ?? true; }
			set
			{ 
				if (ShowPlaceholderWhenEmpty != value)
				{
					Properties[ShowPlaceholderWhenEmptyKey] = value;
					UpdateText();
				}
			}
		}

		int IsUpdatingText
		{
			get => Properties.Get<int>(IsUpdatingTextKey);
			set => Properties.Set(IsUpdatingTextKey, value);
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="MaskedTextBox"/> class.
		/// </summary>
		public MaskedTextBox()
		{
			HandleEvent(TextChangingEvent);
			HandleEvent(TextChangedEvent);
			HandleEvent(KeyDownEvent);
			HandleEvent(GotFocusEvent);
			HandleEvent(LostFocusEvent);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MaskedTextBox"/> class with the specified masked text provider.
		/// </summary>
		/// <param name="provider">Masked text provider to specify the format of the mask.</param>
		public MaskedTextBox(IMaskedTextProvider provider)
			: this()
		{
			if (provider == null)
				throw new ArgumentNullException(nameof(provider));
			this.provider = provider;
			UpdateText();
		}

		/// <summary>
		/// Updates the text to the display text from the provider.
		/// </summary>
		/// <remarks>
		/// Call this in a subclass when you want to update the text based on the state of the control.
		/// When the <see cref="IMaskedTextProvider.IsEmpty"/> is true, it will set the text to null to show the placeholder text.
		/// 
		/// Override this to perform other actions before or after the text of the control is updated.
		/// </remarks>
		protected virtual void UpdateText()
		{
			if (provider == null)
				return;
			IsUpdatingText++;
			var hasFocus = HasFocus;
			if (!hasFocus && ShowPlaceholderWhenEmpty && provider.IsEmpty && !string.IsNullOrEmpty(PlaceholderText))
				base.Text = null;
			else if ((hasFocus && ShowPromptMode == ShowPromptMode.OnFocus) || ShowPromptMode == ShowPromptMode.Always)
				base.Text = provider.DisplayText;
			else
				base.Text = provider.Text;
			IsUpdatingText--;
		}

		/// <summary>
		/// Raises the <see cref="Control.LoadComplete"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			UpdateText();
		}

		/// <summary>
		/// Raises the <see cref="Control.GotFocus"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			if (ShowPromptMode == ShowPromptMode.OnFocus || ShowPlaceholderWhenEmpty)
				UpdateText();
		}

		/// <summary>
		/// Raises the <see cref="Control.LostFocus"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			if (ShowPromptMode == ShowPromptMode.OnFocus || ShowPlaceholderWhenEmpty)
				UpdateText();
		}

		/// <summary>
		/// Raises the <see cref="TextControl.TextChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected override void OnTextChanged(EventArgs e)
		{
			// handle undo/redo and drag/drop which doesn't always get a TextChanging event.
			if (IsUpdatingText == 0 && provider != null)
			{
				provider.Text = base.Text;
				UpdateText();
			}

			base.OnTextChanged(e);
		}

		/// <summary>
		/// Raises the <see cref="TextBox.TextChanging"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected override void OnTextChanging(TextChangingEventArgs e)
		{
			base.OnTextChanging(e);
			if (e.Cancel || ReadOnly || !Enabled || !e.FromUser)
				return;
			var sel = e.Range;
			var pos = sel.Start;
			var len = sel.Length();
			var overwrite = IsOverwrite;
			if (provider == null)
			{
				// with no provider, still have some functionality
				if (e.Text.Length > 0)
				{
					if (overwrite && len == 0)
					{
						var text = Text;
						if (sel.Start < text.Length)
							text = text.Remove(sel.Start, Math.Min(text.Length - sel.Start, e.Text.Length));
						text = text.Insert(sel.Start, e.Text);
						Text = text;
					}
					else
						SelectedText = e.Text;
					CaretIndex = pos + e.Text.Length;
					e.Cancel = true;
				}
				return;
			}

			if (len > 0)
			{
				var tempPos = pos;
				if (overwrite)
					provider.Clear(ref tempPos, len, true);
				else
					provider.Delete(ref tempPos, len, true);
			}

			foreach (char ch in e.Text)
			{
				if (overwrite)
					provider.Replace(ch, ref pos);
				else
					provider.Insert(ch, ref pos);
			}

			UpdateText();
			CaretIndex = pos;
			e.Cancel = true;
		}

		/// <summary>
		/// Raises the <see cref="Control.KeyDown"/> event.
		/// </summary>
		/// <param name="e">Key event arguments</param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.Handled || ReadOnly || !Enabled)
				return;
			switch (e.KeyData)
			{
				case Keys.Delete:
				case Keys.Backspace:
					if (provider == null)
						return;
					// override default delete/backspace behaviour so we can skip past literals
					var sel = Selection;
					var pos = sel.Start;
					var len = sel.Length();
					var forward = len > 0 || e.KeyData == Keys.Delete;
					len = Math.Max(1, len);
					bool changed;
					if (IsOverwrite)
						changed = provider.Clear(ref pos, len, forward);
					else
						changed = provider.Delete(ref pos, len, forward);

					if (changed)
					{
						Text = provider.DisplayText;
						CaretIndex = pos;
					}
					e.Handled = true;
					break;
				case Keys.Insert:
					if (!SupportsInsert && InsertMode == InsertKeyMode.Toggle)
					{
						ManualOverwriteMode = !ManualOverwriteMode;
						e.Handled = true;
					}
					break;
			}
		}

		/// <summary>
		/// Gets or sets the text of the control including any mask characters.
		/// </summary>
		/// <value>The text content of the control.</value>
		public override string Text
		{
			get
			{
				return provider != null ? provider.Text : base.Text;
			}
			set
			{
				if (provider != null)
				{
					provider.Text = value;
					UpdateText();
				}
				else
					base.Text = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the mask is completed.
		/// </summary>
		/// <value><c>true</c> if mask completed; otherwise, <c>false</c>.</value>
		public bool MaskCompleted => provider?.MaskCompleted == true;
	}
}

