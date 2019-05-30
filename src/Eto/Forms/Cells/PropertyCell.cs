using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows.Input;
using System.Reflection;

namespace Eto.Forms
{
	/// <summary>
	/// Base type for a <see cref="PropertyCell"/> to display cell contents for a particular type.
	/// </summary>
	public abstract class PropertyCellType
	{
		/// <summary>
		/// Gets the identifier of this type.
		/// </summary>
		/// <remarks>
		/// This is used to cache the cell content so it can be reused for performance reasons.
		/// You must ensure that all your cell types have unique identifiers, otherwise the incorrect content control
		/// will be used in the grid.
		/// </remarks>
		/// <value>The identifier for this type.</value>
		public abstract string Identifier { get; }

		/// <summary>
		/// Determines whether this instance can be used to display the specified <paramref name="itemType"/>.
		/// </summary>
		/// <returns><c>true</c> if this instance can display the specified itemType; otherwise, <c>false</c>.</returns>
		/// <param name="itemType">Item type that is retrieved using <see cref="PropertyCell.TypeBinding"/>.</param>
		public abstract bool CanDisplay(object itemType);

		/// <summary>
		/// Creates the content control for the cell.
		/// </summary>
		/// <remarks>
		/// The control returned may be reused for other cells, so it is ideal to use MVVM data binding using BindDataContext() 
		/// methods of your controls.
		/// This should return the same control for each row, otherwise the incorrect control may be shown on certain cells.
		/// </remarks>
		/// <param name="args">Cell arguments.</param>
		public abstract Control OnCreate(CellEventArgs args);

		/// <summary>
		/// Configures the content control for the given cell information.
		/// </summary>
		/// <remarks>
		/// When the DataContext changes on a cell, this will be called to configure the cell.
		/// 
		/// You are only required to override this when you are not using MVVM data binding with your controls created by
		/// <see cref="OnCreate"/>.
		/// </remarks>
		/// <param name="args">Cell arguments</param>
		/// <param name="control">Content control that was previously created with the OnCreate method.</param>
		public virtual void OnConfigure(CellEventArgs args, Control control)
		{
		}

		/// <summary>
		/// Paints the cell when <see cref="CustomCell.SupportsControlView"/> is false.
		/// </summary>
		/// <remarks>
		/// For platforms like GTK and WinForms which don't support using a custom control per cell, this will be called
		/// to paint the content of the cell when it is not in edit mode.
		/// </remarks>
		/// <param name="args">Cell paint arguments.</param>
		public abstract void OnPaint(CellPaintEventArgs args);
	}

	/// <summary>
	/// Base property cell type for a given type
	/// </summary>
	/// <remarks>
	/// This is useful when displaying a cell with data of a particular type.  It will be used for any row that returns
	/// a type with <see cref="PropertyCell.TypeBinding"/> that is compatible with the specified <typeparamref name="T"/> type.
	/// </remarks>
	/// <typeparam name="T">Type of data that this cell type can show</typeparam>
	public abstract class PropertyCellType<T> : PropertyCellType
	{
		/// <summary>
		/// Gets the identifier of this type.
		/// </summary>
		/// <remarks>This is used to cache the cell content so it can be reused for performance reasons.
		/// You must ensure that all your cell types have unique identifiers, otherwise the incorrect content control
		/// will be used in the grid.</remarks>
		/// <value>The identifier for this type.</value>
		public override string Identifier { get { return typeof(T).FullName; } }

		/// <summary>
		/// Gets or sets the item binding to get/set the value of the cell from the model.
		/// </summary>
		/// <value>The item binding.</value>
		public IndirectBinding<T> ItemBinding { get; set; }

		/// <summary>
		/// Determines whether this instance can display the specified itemType.
		/// </summary>
		/// <returns><c>true</c> if this instance can display the specified itemType; otherwise, <c>false</c>.</returns>
		/// <param name="itemType">Item type.</param>
		public override bool CanDisplay(object itemType)
		{
			var type = itemType as Type;
			if (type != null)
			{
				if (typeof(T).IsAssignableFrom(type))
					return true;
				var underlyingType = Nullable.GetUnderlyingType(typeof(T));
				return underlyingType != null && underlyingType.IsAssignableFrom(type);
			}
			return false;
		}
	}

	/// <summary>
	/// Property cell type to edit boolean values using a check box and a True/False label.
	/// </summary>
	public class PropertyCellTypeBoolean : PropertyCellType<bool?>
	{
		/// <summary>
		/// Gets or sets a binding to indicate that the check box should allow three state (null).
		/// </summary>
		public IndirectBinding<bool> ItemThreeStateBinding { get; set; }

		/// <summary>
		/// Creates the content control for the cell.
		/// </summary>
		/// <remarks>
		/// The control returned may be reused for other cells, so it is ideal to use MVVM data binding using
		/// BindDataContext() 
		/// methods of your controls.
		/// This should return the same control for each row, otherwise the incorrect control may be shown on certain cells.
		/// </remarks>
		/// <param name="args">Cell arguments.</param>
		public override Control OnCreate(CellEventArgs args)
		{
			if (ItemBinding == null)
				return null;
			var checkBox = new CheckBox();
			checkBox.CheckedBinding.BindDataContext(ItemBinding);
			if (ItemThreeStateBinding != null)
				checkBox.BindDataContext(c => c.ThreeState, ItemThreeStateBinding);
			var label = new Label { Wrap = WrapMode.None, VerticalAlignment = VerticalAlignment.Center };
			label.MouseDoubleClick += (sender, e) => checkBox.Checked = !checkBox.Checked;
			label.Bind(c => c.TextColor, args, a => a.CellTextColor);
			label.TextBinding.Bind(checkBox, Binding.Property((CheckBox c) => c.Checked).Convert(r => Convert.ToString(r)));
			return new TableLayout
			{
				Spacing = new Size(5, 0),
				Rows = { new TableRow(new TableCell(label, true), checkBox) } 
			};
		}

		/// <summary>
		/// Paints the cell when <see cref="CustomCell.SupportsControlView"/> is false.
		/// </summary>
		/// <remarks>
		/// For platforms like GTK and WinForms which don't support using a custom control per cell, this will be called
		/// to paint the content of the cell when it is not in edit mode.
		/// </remarks>
		/// <param name="args">Cell paint arguments.</param>
		public override void OnPaint(CellPaintEventArgs args)
		{
			if (ItemBinding == null)
				return;
			var value = ItemBinding.GetValue(args.Item);
			args.DrawCenteredText(value?.ToString());
		}
	}

	/// <summary>
	/// Property cell type to edit string values.
	/// </summary>
	public class PropertyCellTypeString : PropertyCellType<string>
	{
		/// <summary>
		/// Creates the content control for the cell.
		/// </summary>
		/// <remarks>The control returned may be reused for other cells, so it is ideal to use MVVM data binding using
		/// BindDataContext() 
		/// methods of your controls.
		/// This should return the same control for each row, otherwise the incorrect control may be shown on certain cells.</remarks>
		/// <param name="args">Cell arguments.</param>
		public override Control OnCreate(CellEventArgs args)
		{
			if (ItemBinding == null)
				return null;
			var textBox = new TextBox { ShowBorder = false };
			textBox.BackgroundColor = Colors.Transparent;
			var colorBinding = textBox.Bind(c => c.TextColor, args, a => a.CellTextColor);
			textBox.GotFocus += (sender, e) =>
			{
				colorBinding.Mode = DualBindingMode.Manual;
				textBox.BackgroundColor = SystemColors.ControlBackground;
				textBox.TextColor = SystemColors.ControlText;
			};
			textBox.LostFocus += (sender, e) =>
			{
				textBox.BackgroundColor = Colors.Transparent;
				colorBinding.Mode = DualBindingMode.TwoWay;
				colorBinding.Update();
			};
			textBox.TextBinding.BindDataContext(ItemBinding);
			return textBox;
		}

		/// <summary>
		/// Paints the cell when <see cref="CustomCell.SupportsControlView"/> is false.
		/// </summary>
		/// <remarks>
		/// For platforms like GTK and WinForms which don't support using a custom control per cell, this will be called
		/// to paint the content of the cell when it is not in edit mode.
		/// </remarks>
		/// <param name="args">Cell paint arguments.</param>
		public override void OnPaint(CellPaintEventArgs args)
		{
			if (ItemBinding == null)
				return;
			var value = ItemBinding.GetValue(args.Item);
			args.DrawCenteredText(value);
		}
	}


	/// <summary>
	/// Property cell type to edit int values.
	/// </summary>
	public class PropertyCellTypeNumber<T> : PropertyCellType<T>
	{
		/// <summary>
		/// Creates the content control for the cell.
		/// </summary>
		/// <remarks>The control returned may be reused for other cells, so it is ideal to use MVVM data binding using
		/// BindDataContext() 
		/// methods of your controls.
		/// This should return the same control for each row, otherwise the incorrect control may be shown on certain cells.</remarks>
		/// <param name="args">Cell arguments.</param>
		public override Control OnCreate(CellEventArgs args)
		{
			if (ItemBinding == null)
				return null;
			var textBox = new NumericMaskedTextBox<T> { ShowBorder = false };
			textBox.BackgroundColor = Colors.Transparent;
			var colorBinding = textBox.Bind(c => c.TextColor, args, a => a.CellTextColor);
			textBox.GotFocus += (sender, e) =>
			{
				colorBinding.Mode = DualBindingMode.Manual;
				textBox.BackgroundColor = SystemColors.ControlBackground;
				textBox.TextColor = SystemColors.ControlText;
			};
			textBox.LostFocus += (sender, e) =>
			{
				textBox.BackgroundColor = Colors.Transparent;
				colorBinding.Mode = DualBindingMode.TwoWay;
				colorBinding.Update();
			};
			textBox.ValueBinding.BindDataContext(ItemBinding);
			return textBox;
		}

		/// <summary>
		/// Paints the cell when <see cref="CustomCell.SupportsControlView"/> is false.
		/// </summary>
		/// <remarks>
		/// For platforms like GTK and WinForms which don't support using a custom control per cell, this will be called
		/// to paint the content of the cell when it is not in edit mode.
		/// </remarks>
		/// <param name="args">Cell paint arguments.</param>
		public override void OnPaint(CellPaintEventArgs args)
		{
			if (ItemBinding == null)
				return;
			var value = ItemBinding.GetValue(args.Item);
			args.DrawCenteredText(Convert.ToString(value));
		}
	}

	/// <summary>
	/// Property cell type to edit a color value with a color picker and optional hex masked value.
	/// </summary>
	public class PropertyCellTypeColor : PropertyCellType<Color>
	{
		/// <summary>
		/// Gets or sets a value indicating whether to show the hex value
		/// </summary>
		/// <value><c>true</c> if show hex; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool ShowHex { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to show the alpha component of the color.
		/// </summary>
		/// <value><c>true</c> to show the alpha value; otherwise, <c>false</c>.</value>
		public bool ShowAlpha { get; set; }

		/// <summary>
		/// Gets or sets a value indicating the hex value is editable by the user.
		/// </summary>
		/// <value><c>true</c> if the user can edit the hex value; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool HexEditable { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.PropertyCellTypeColor"/> class.
		/// </summary>
		public PropertyCellTypeColor()
		{
			ShowHex = true;
			HexEditable = true;
		}

		/// <summary>
		/// Creates the content control for the cell.
		/// </summary>
		/// <remarks>
		/// The control returned may be reused for other cells, so it is ideal to use MVVM data binding using
		/// BindDataContext() 
		/// methods of your controls.
		/// This should return the same control for each row, otherwise the incorrect control may be shown on certain cells.
		/// </remarks>
		/// <param name="args">Cell arguments.</param>
		public override Control OnCreate(CellEventArgs args)
		{
			if (ItemBinding == null)
				return null;
			var picker = new ColorPicker();
			picker.ValueBinding.BindDataContext(ItemBinding);

			if (!ShowHex)
				return picker;

			var mask = ShowAlpha ? "\\#>AAAAAAAA" : "\\#>AAAAAA";

			Control value;

			if (HexEditable)
			{
				var textBox = new MaskedTextBox
				{
					Provider = new FixedMaskedTextProvider(mask),
					InsertMode = InsertKeyMode.Overwrite,
					BackgroundColor = Colors.Transparent,
					ShowBorder = false
				};
				textBox.TextChanging += (sender, e) =>
				{
					e.Cancel = e.FromUser && !e.Text.ToCharArray().Select(r => r.ToString()).All("0123456789abcdefABCDEF".Contains);
				};
				var colorBinding = textBox.Bind(c => c.TextColor, args, a => a.CellTextColor);
				textBox.TextBinding.Bind(picker, Binding.Property((ColorPicker p) => p.Value).Convert(v => v.ToHex(ShowAlpha), v =>
				{
					Color c;
					if (!Color.TryParse(v, out c) && !ShowAlpha)
						c = Colors.Black;
					return c;
				}));

				textBox.GotFocus += (sender, e) =>
				{
					colorBinding.Mode = DualBindingMode.Manual;
					textBox.BackgroundColor = SystemColors.ControlBackground;
					textBox.TextColor = SystemColors.ControlText;
				};
				textBox.LostFocus += (sender, e) =>
				{
					textBox.BackgroundColor = Colors.Transparent;
					colorBinding.Mode = DualBindingMode.TwoWay;
					colorBinding.Update();
				};
				value = textBox;
			}
			else
			{
				var label = new Label();
				label.Bind(c => c.TextColor, args, a => a.CellTextColor);
				label.TextBinding.Bind(picker, Binding.Property((ColorPicker p) => p.Value).Convert(v => v.ToHex(ShowAlpha), v =>
				{
					Color c;
					Color.TryParse(v, out c);
					return c;
				}));
				value = label;
			}

			return new StackLayout
			{
				Orientation = Orientation.Horizontal,
				VerticalContentAlignment = VerticalAlignment.Stretch,
				Items = { new StackLayoutItem(value, expand: true), picker }
			};
		}

		/// <summary>
		/// Paints the cell when <see cref="CustomCell.SupportsControlView"/> is false.
		/// </summary>
		/// <remarks>
		/// For platforms like GTK and WinForms which don't support using a custom control per cell, this will be called
		/// to paint the content of the cell when it is not in edit mode.
		/// </remarks>
		/// <param name="args">Cell paint arguments.</param>
		public override void OnPaint(CellPaintEventArgs args)
		{
			if (ItemBinding == null)
				return;
			var value = ItemBinding.GetValue(args.Item);

			const int size = 35;
			var rect = args.ClipRectangle;
			rect.Right -= size;
			args.DrawCenteredText(value.ToHex(ShowAlpha), rect: rect);

			rect = args.ClipRectangle;
			rect.Left = rect.Right - size;
			args.Graphics.FillRectangle(value, rect);
		}
	}

	/// <summary>
	/// Property cell type to edit a cell value using a date/time picker.
	/// </summary>
	public class PropertyCellTypeDateTime : PropertyCellType<DateTime?>
	{
		/// <summary>
		/// Gets or sets the mode to use for the picker.
		/// </summary>
		/// <value>The picker mode.</value>
		[DefaultValue(DateTimePickerMode.Date)]
		public DateTimePickerMode Mode { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyCellTypeDateTime"/> class.
		/// </summary>
		public PropertyCellTypeDateTime()
		{
			Mode = DateTimePickerMode.Date;
		}

		/// <summary>
		/// Creates the content control for the cell.
		/// </summary>
		/// <remarks>
		/// The control returned may be reused for other cells, so it is ideal to use MVVM data binding using
		/// BindDataContext() 
		/// methods of your controls.
		/// This should return the same control for each row, otherwise the incorrect control may be shown on certain cells.
		/// </remarks>
		/// <param name="args">Cell arguments.</param>
		public override Control OnCreate(CellEventArgs args)
		{
			if (ItemBinding == null)
				return null;
			var dateTimePicker = new DateTimePicker
			{ 
				ShowBorder = false,
				Mode = Mode,
				BackgroundColor = Colors.Transparent
			};
			dateTimePicker.Bind(c => c.TextColor, args, a => a.CellTextColor);
			dateTimePicker.ValueBinding.BindDataContext(ItemBinding);
			return dateTimePicker;
		}

		/// <summary>
		/// Paints the cell when <see cref="CustomCell.SupportsControlView"/> is false.
		/// </summary>
		/// <remarks>
		/// For platforms like GTK and WinForms which don't support using a custom control per cell, this will be called
		/// to paint the content of the cell when it is not in edit mode.
		/// </remarks>
		/// <param name="args">Cell paint arguments.</param>
		public override void OnPaint(CellPaintEventArgs args)
		{
			if (ItemBinding == null)
				return;
			var value = ItemBinding.GetValue(args.Item);
			args.DrawCenteredText(string.Format("{0:d}", value));
		}
	}

	/// <summary>
	/// Property cell type to edit an enum value using an <see cref="EnumDropDown{T}"/>.
	/// </summary>
	/// <seealso cref="PropertyCellTypeEnum"/>
	public class PropertyCellTypeEnum<T> : PropertyCellType<T>
		where T: struct
	{
		/// <summary>
		/// Creates the content control for the cell.
		/// </summary>
		/// <remarks>
		/// The control returned may be reused for other cells, so it is ideal to use MVVM data binding using
		/// BindDataContext() 
		/// methods of your controls.
		/// This should return the same control for each row, otherwise the incorrect control may be shown on certain cells.
		/// </remarks>
		/// <param name="args">Cell arguments.</param>
		public override Control OnCreate(CellEventArgs args)
		{
			if (ItemBinding == null)
				return null;
			var enumDropDown = new EnumDropDown<T>();
			enumDropDown.ShowBorder = false;
			enumDropDown.SelectedValueBinding.BindDataContext(ItemBinding);
			return enumDropDown;
		}

		/// <summary>
		/// Paints the cell when <see cref="CustomCell.SupportsControlView"/> is false.
		/// </summary>
		/// <remarks>
		/// For platforms like GTK and WinForms which don't support using a custom control per cell, this will be called
		/// to paint the content of the cell when it is not in edit mode.
		/// </remarks>
		/// <param name="args">Cell paint arguments.</param>
		public override void OnPaint(CellPaintEventArgs args)
		{
			if (ItemBinding == null)
				return;
			var value = ItemBinding.GetValue(args.Item);
			args.DrawCenteredText(Convert.ToString(value));
		}
	}

	/// <summary>
	/// Property cell type to edit any type of number
	/// </summary>
	public class PropertyCellTypeNumber : PropertyCellType<object>
	{
		/// <summary>
		/// Gets the identifier for this cell type
		/// </summary>
		public override string Identifier => "PropertyCellTypeNumber";

		/// <summary>
		/// Gets or sets the item binding to get/set the type of the numeric value
		/// </summary>
		/// <remarks>
		/// Use this to specify what numeric type should be used to create the NumericMaskedTextBox.
		/// Otherwise, the type of the current value will be used.  This is only needed when the value can be null.
		/// </remarks>
		/// <value>The item type binding.</value>
		public IndirectBinding<Type> ItemTypeBinding { get; set; }

		/// <summary>
		/// Determines whether this instance can display the specified itemType.
		/// </summary>
		/// <returns><c>true</c> if this instance can display the specified itemType; otherwise, <c>false</c>.</returns>
		/// <param name="itemType">Item type.</param>
		public override bool CanDisplay(object itemType)
		{
			var type = itemType as Type;
			if (type == null)
				return false;
			type = Nullable.GetUnderlyingType(type) ?? type;
			return type == typeof(sbyte)
				|| type == typeof(short)
				|| type == typeof(int)
				|| type == typeof(long)
				|| type == typeof(byte)
				|| type == typeof(ushort)
				|| type == typeof(uint)
				|| type == typeof(ulong)
				|| type == typeof(float)
				|| type == typeof(double)
				|| type == typeof(decimal);
		}

		/// <summary>
		/// Creates the content control for the cell.
		/// </summary>
		/// <remarks>
		/// The control returned may be reused for other cells, so it is ideal to use MVVM data binding using BindDataContext() 
		/// methods of your controls.
		/// This should return the same control for each row, otherwise the incorrect control may be shown on certain cells.
		/// </remarks>
		/// <param name="args">Cell arguments.</param>
		public override Control OnCreate(CellEventArgs args)
		{
			if (ItemBinding == null)
				return null;
			return new Panel { Content = CreateNumericTextBox(args, null) };
		}

		/// <summary>
		/// Configures the content control for the given cell information.
		/// </summary>
		/// <remarks>
		/// When the DataContext changes on a cell, this will be called to configure the cell.
		/// 
		/// You are only required to override this when you are not using MVVM data binding with your controls created by
		/// <see cref="OnCreate"/>.
		/// </remarks>
		/// <param name="args">Cell arguments</param>
		/// <param name="control">Content control that was previously created with the OnCreate method.</param>
		public override void OnConfigure(CellEventArgs args, Control control)
		{
			base.OnConfigure(args, control);

			if (control is Panel panel)
			{
				var content = panel.Content;
				var newContent = CreateNumericTextBox(args, content);
				if (!ReferenceEquals(content, newContent))
					panel.Content = newContent;
			}
		}

		Control CreateNumericTextBox(CellEventArgs args, Control current)
		{
			var type = ItemTypeBinding?.GetValue(args.Item)
				?? ItemBinding?.GetValue(args.Item)?.GetType();
			if (type == null)
				return null;
			var controlType = typeof(NumericMaskedTextBox<>).MakeGenericType(type);
			if (controlType.IsInstanceOfType(current))
				return current;

			var textBox = Activator.CreateInstance(controlType) as TextBox;

			textBox.ShowBorder = false;
			textBox.BackgroundColor = Colors.Transparent;
			var colorBinding = textBox.Bind(c => c.TextColor, args, a => a.CellTextColor);
			textBox.GotFocus += (sender, e) =>
			{
				colorBinding.Mode = DualBindingMode.Manual;
				textBox.BackgroundColor = SystemColors.ControlBackground;
				textBox.TextColor = SystemColors.ControlText;
			};
			textBox.LostFocus += (sender, e) =>
			{
				textBox.BackgroundColor = Colors.Transparent;
				colorBinding.Mode = DualBindingMode.TwoWay;
				colorBinding.Update();
			};

			var binding = ItemBinding.BindingOfType(typeof(object), type);
			textBox.BindDataContextProperty(nameof(NumericMaskedTextBox<int>.ValueBinding), type, binding);

			return textBox;
		}

		/// <summary>
		/// Paints the cell when <see cref="CustomCell.SupportsControlView"/> is false.
		/// </summary>
		/// <remarks>
		/// For platforms like GTK and WinForms which don't support using a custom control per cell, this will be called
		/// to paint the content of the cell when it is not in edit mode.
		/// </remarks>
		/// <param name="args">Cell paint arguments.</param>
		public override void OnPaint(CellPaintEventArgs args)
		{
			if (ItemBinding == null)
				return;
			var value = ItemBinding.GetValue(args.Item);
			args.DrawCenteredText(Convert.ToString(value));
		}
	}

	/// <summary>
	/// Property cell type to display any type of enumeration
	/// </summary>
	/// <seealso cref="PropertyCellTypeEnum{T}"/>
	public class PropertyCellTypeEnum : PropertyCellType<object>
	{
		/// <summary>
		/// Gets the identifier for this cell type
		/// </summary>
		public override string Identifier => "PropertyCellTypeEnum";

		/// <summary>
		/// Gets or sets the item binding to get/set the type of the numeric value
		/// </summary>
		/// <remarks>
		/// Use this to specify what enum type should be used to create the EnumDropDown.
		/// Otherwise, the type of the current value will be used.  This is only needed when the value can be null.
		/// </remarks>
		/// <value>The item type binding.</value>
		public IndirectBinding<Type> ItemTypeBinding { get; set; }

		/// <summary>
		/// Determines whether this instance can display the specified itemType.
		/// </summary>
		/// <returns><c>true</c> if this instance can display the specified itemType; otherwise, <c>false</c>.</returns>
		/// <param name="itemType">Item type.</param>
		public override bool CanDisplay(object itemType)
		{
			return (itemType as Type)?.IsEnum() == true;
		}

		/// <summary>
		/// Creates the content control for the cell.
		/// </summary>
		/// <remarks>
		/// The control returned may be reused for other cells, so it is ideal to use MVVM data binding using
		/// BindDataContext() 
		/// methods of your controls.
		/// This should return the same control for each row, otherwise the incorrect control may be shown on certain cells.
		/// </remarks>
		/// <param name="args">Cell arguments.</param>
		public override Control OnCreate(CellEventArgs args)
		{
			return new Panel { Content = CreateDropDown(args, null) };
		}

		/// <summary>
		/// Configures the content control for the given cell information.
		/// </summary>
		/// <remarks>
		/// When the DataContext changes on a cell, this will be called to configure the cell.
		/// 
		/// You are only required to override this when you are not using MVVM data binding with your controls created by
		/// <see cref="OnCreate"/>.
		/// </remarks>
		/// <param name="args">Cell arguments</param>
		/// <param name="control">Content control that was previously created with the OnCreate method.</param>
		public override void OnConfigure(CellEventArgs args, Control control)
		{
			base.OnConfigure(args, control);
			if (control is Panel panel)
			{
				var content = panel.Content;
				var newContent = CreateDropDown(args, content);
				if (!ReferenceEquals(content, newContent))
					panel.Content = newContent;
			}
		}

		Control CreateDropDown(CellEventArgs args, Control current)
		{
			var type = ItemTypeBinding?.GetValue(args.Item) ?? ItemBinding?.GetValue(args.Item).GetType();
			if (type == null || !type.IsEnum())
				return null;
			var enumType = typeof(EnumDropDown<>).MakeGenericType(type);
			if (enumType.IsInstanceOfType(current))
				return current;
			var enumDropDown = Activator.CreateInstance(enumType) as Control;
			enumType.GetRuntimeProperty("ShowBorder")?.SetValue(enumDropDown, false);


			var binding = ItemBinding.BindingOfType(typeof(object), type);
			enumDropDown.BindDataContextProperty(nameof(EnumDropDown<Orientation>.SelectedValueBinding), type, binding);

			return enumDropDown;
		}

		/// <summary>
		/// Paints the cell when <see cref="CustomCell.SupportsControlView"/> is false.
		/// </summary>
		/// <remarks>
		/// For platforms like GTK and WinForms which don't support using a custom control per cell, this will be called
		/// to paint the content of the cell when it is not in edit mode.
		/// </remarks>
		/// <param name="args">Cell paint arguments.</param>
		public override void OnPaint(CellPaintEventArgs args)
		{
			if (ItemBinding == null)
				return;
			var value = ItemBinding.GetValue(args.Item);
			args.DrawCenteredText(Convert.ToString(value));
		}
	}

	/// <summary>
	/// Property cell type drop down.
	/// </summary>
	public class PropertyCellTypeDropDown : PropertyCellType<object>
	{
		/// <summary>
		/// Gets or sets the binding to get the items for the drop down.
		/// </summary>
		/// <remarks>
		/// If all rows have the same items, you can use a delegate binding to return a single instance of an items enumeration.
		/// </remarks>
		/// <value>The items binding.</value>
		public IndirectBinding<IEnumerable<object>> ItemsBinding { get; set; }

		/// <summary>
		/// Gets or sets the binding to get the text value for the drop down.
		/// </summary>
		/// <value>The binding to get the drop down item text.</value>
		public IndirectBinding<string> ItemTextBinding { get; set; }

		/// <summary>
		/// Gets or sets the binding to get the key value for the drop down.
		/// </summary>
		/// <value>The binding to get the drop down item key.</value>
		public IndirectBinding<string> ItemKeyBinding { get; set; }

		/// <summary>
		/// Creates the content control for the cell.
		/// </summary>
		/// <remarks>
		/// The control returned may be reused for other cells, so it is ideal to use MVVM data binding using
		/// BindDataContext() 
		/// methods of your controls.
		/// This should return the same control for each row, otherwise the incorrect control may be shown on certain cells.
		/// </remarks>
		/// <param name="args">Cell arguments.</param>
		public override Control OnCreate(CellEventArgs args)
		{
			if (ItemBinding == null)
				return null;
			var dropDown = new DropDown();
			dropDown.ShowBorder = false;
			dropDown.ItemTextBinding = ItemTextBinding;
			dropDown.ItemKeyBinding = ItemKeyBinding;
			if (ItemsBinding != null)
				dropDown.BindDataContext(c => c.DataStore, ItemsBinding);
			dropDown.SelectedValueBinding.BindDataContext(ItemBinding);
			return dropDown;
		}

		/// <summary>
		/// Paints the cell when <see cref="CustomCell.SupportsControlView"/> is false.
		/// </summary>
		/// <remarks>
		/// For platforms like GTK and WinForms which don't support using a custom control per cell, this will be called
		/// to paint the content of the cell when it is not in edit mode.
		/// </remarks>
		/// <param name="args">Cell paint arguments.</param>
		public override void OnPaint(CellPaintEventArgs args)
		{
			if (ItemBinding == null)
				return;
			var value = ItemBinding.GetValue(args.Item);
			args.DrawCenteredText(Convert.ToString(value));
		}
	}

	/// <summary>
	/// A custom cell implementation that implements a generic method of showing different types of controls on a per row basis.
	/// </summary>
	/// <remarks>
	/// This is ideal for things like showing a property grid to edit various properties of an object.
	/// 
	/// The way this works is by using the <see cref="TypeBinding"/> of each row to determine which <see cref="PropertyCellType"/>
	/// to use from the <see cref="Types"/> collection.  It calls <see cref="PropertyCellType.CanDisplay"/> on each to find
	/// a type that can display based on the value returned.
	/// 
	/// Each <see cref="PropertyCellType"/> added to the cell must have a unique value for <see cref="PropertyCellType.Identifier"/>
	/// to ensure the correct control is shown in the cell.  The identifier is used to cache the content controls generated by the
	/// PropertyCellType when displaying the same value for other rows while scrolling.
	/// 
	/// Ideally, the content control generated from the <see cref="PropertyCellType"/> should use MVVM BindDataContext() methods
	/// for its children so that they are updated automatically when reused for different controls.  This is not a requirement, however
	/// you must then implement/override the <see cref="PropertyCellType.OnConfigure"/> to update your control when the item/row is changed.
	/// </remarks>
	public class PropertyCell : CustomCell
	{
		List<PropertyCellType> types = new List<PropertyCellType>();

		/// <summary>
		/// Initializes a new instance of the property cell
		/// </summary>
		public PropertyCell()
		{
			TypeBinding = Binding.Property((object o) => (object)o.GetType());
		}

		/// <summary>
		/// Gets or sets the binding to get the type for the 
		/// </summary>
		/// <value>The type binding.</value>
		public IIndirectBinding<object> TypeBinding { get; set; }

		/// <summary>
		/// Gets the list of types to use for each cell, in order of preference.
		/// </summary>
		/// <remarks>
		/// When attempting to use a type for a cell, the types collection is scanned in order for the first PropertyCellType
		/// that returns true for <see cref="PropertyCellType.CanDisplay"/>.
		/// </remarks>
		/// <value>The list of types this property cell supports.</value>
		public IList<PropertyCellType> Types
		{
			get { return types; }
		}

		PropertyCellType GetType(object item)
		{
			if (TypeBinding == null)
				return null;
			var typeId = TypeBinding.GetValue(item);
			return types.FirstOrDefault(r => r.CanDisplay(typeId));
		}

		/// <summary>
		/// Creates an instance of the control for a cell.
		/// </summary>
		/// <param name="args">Arguments when creating the cell to get the row, item and state.</param>
		/// <returns>The control to display in the cell.</returns>
		protected override Control OnCreateCell(CellEventArgs args)
		{
			var type = GetType(args.Item);
			if (type != null)
				return type.OnCreate(args);
			return base.OnCreateCell(args);
		}

		/// <summary>
		/// Configures an existing cell when it is reused for a different row or the data changes.
		/// </summary>
		/// <remarks>This should set up your control your cell content to be reused. If null, the DataContext of your control will be
		/// set to the row model instance.
		/// 
		/// Typically if you use MVVM data binding, you do not need to override the standard behaviour.</remarks>
		/// <param name="args">Arguments for the cell</param>
		/// <param name="control">Existing control to configure for the new cell and/or data</param>
		protected override void OnConfigureCell(CellEventArgs args, Control control)
		{
			base.OnConfigureCell(args, control);

			var type = GetType(args.Item);
			if (type != null)
			{
				type.OnConfigure(args, control);
			}
		}

		/// <summary>
		/// Gets the identifier of the cell based on its content.
		/// </summary>
		/// <remarks>When you have different controls on a per-row level, each variation must have an identifier string
		/// to allow the framework to cache the different types of cells to provide good performance.
		/// 
		/// This hooks into standard cell caching mechanisms in certain platforms, such as on the Mac.</remarks>
		/// <param name="args">Arguments for the cell</param>
		/// <value>The identifier for the cell.</value>
		protected override string OnGetIdentifier(CellEventArgs args)
		{
			var type = GetType(args.Item);
			if (type != null)
				return type.Identifier;
			return base.OnGetIdentifier(args);
		}


		/// <summary>
		/// Raises the <see cref="PropertyCellType.OnPaint"/> event.
		/// </summary>
		/// <param name="args">Cell paint arguments.</param>
		protected override void OnPaint(CellPaintEventArgs args)
		{
			var type = GetType(args.Item);
			if (type != null)
				type.OnPaint(args);
			base.OnPaint(args);
		}
	}
}

