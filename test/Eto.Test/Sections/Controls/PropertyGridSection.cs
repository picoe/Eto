using System;using System.Globalization;using System.ComponentModel;using System.Collections.Generic;
using Eto.Drawing;using Eto.Forms;
using System.Linq;using sc = System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Eto.Test.Sections.Controls{
	[Section("Controls", typeof(PropertyGrid))]
	public class PropertyGridSection : Panel
	{
		public PropertyGridSection()
		{
			var grid = new PropertyGrid();
			grid.SelectedObjects = new[]
			{
				new MyPropertyObject()
			};

			var showCategoriesCheckBox = new CheckBox { Text = "ShowCategories" };
			showCategoriesCheckBox.CheckedBinding.Bind(grid, c => c.ShowCategories);

			var showDescriptionCheckBox = new CheckBox { Text = "ShowDescription" };
			showDescriptionCheckBox.CheckedBinding.Bind(grid, c => c.ShowDescription);

			var layout = new DynamicLayout();
			layout.DefaultSpacing = new Size(4, 4);
			layout.AddSeparateRow(null, showCategoriesCheckBox, showDescriptionCheckBox, null);

			layout.Add(grid, yscale: true);

			Content = layout;
		}
	}

	public class CustomEditor : PropertyGridTypeEditor	{		public override Control CreateControl(CellEventArgs args)		{			return new Label { Text = "Custom Editor!" };		}		public override void PaintCell(CellPaintEventArgs args)		{		}	}

	public enum MyEnum
	{
		FirstEnum,
		SecondEnum,
		ThirdEnum
	}

	[Flags]
	public enum MyFlagsEnum
	{
		None = 0,
		FirstEnum = 1,
		SecondEnum = 2,
		ThirdEnum = 4
	}

	public class MyPropertyObjectConverter : sc.TypeConverter
	{
		public override bool CanConvertTo(sc.ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(sc.ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is MyPropertyObject obj)
				return obj.TextProperty;
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}

	[sc.TypeConverter(typeof(MyPropertyObjectConverter))]
	public class MyPropertyObject
	{
		string _textProperty = "Hello!";
		bool _boolProperty;
		int[] _intArray = { 1, 2, 3 };

		[Display(Name = "Display Property Name", GroupName = "Display Group", Description = "Yeppers, this is a description")]
		public string PropertyWithDisplayAttributeThatShouldNotDisplayTheName { get; set; }

		public string TextProperty
		{
			get => _textProperty;
			set => _textProperty = value;
		}

		[DefaultValue(false)]
		public bool BoolProperty
		{
			get => _boolProperty;
			set => _boolProperty = value;
		}

		[Category("Objects")]
		public MyPropertyObject ObjectProperty { get; set; }

		[Category("Objects")]
		public MyOtherObject ObjectPropertyWithValue { get; set; } = new MyOtherObject();

		[Category("Objects")]
		public MyPropertyObject ReadOnlyObjectPropertyThatsNull { get; }

		[Category("Objects")]
		public MyOtherObject ReadOnlyObjectPropertyThatsNotNull { get; } = new MyOtherObject();

		[Category("Arrays")]
		public int[] IntArray
		{
			get => _intArray;
			set => _intArray = value;
		}

		[Category("Lists")]
		public IList<int> IntArrayAsIListProperty
		{
			get => _intArray;
			set => _intArray = value?.ToArray();
		}

		[Category("Lists")]
		public List<int> ListOfInt { get; set; } = new List<int> { 1, 3, 5 };

		[Category("Lists")]
		public List<string> ListOfString { get; set; } = new List<string> { "First", "Second", "Third" };

		[Category("Lists")]
		public List<MyOtherObject> ListOfObject { get; set; } = new List<MyOtherObject>
		{
			new MyOtherObject { TextProperty = "Hi" },
			new MyOtherObject { TextProperty = "There" }
		};

		[Category("Lists")]
		public IList<int> IListOfInt { get; set; } = new List<int> { 3, 4, 5 };
		[Category("Lists")]
		public IList<int> IListOfIntThatsNull { get; set; }


		[Category("Arrays")]
		public int[][] IntIntArray { get; set; } = new int[][] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 } };

		[DefaultValue("#000000")]
		public Color ColorProperty { get; set; } = Colors.Black;

		public Color? NullableColorProperty { get; set; } = Colors.Black;

		public MyEnum EnumProperty { get; set; }

		public MyEnum? NullableEnumProperty { get; set; }

		public MyFlagsEnum FlagsEnumProperty { get; set; }

		public MyFlagsEnum? NullableFlagsEnumProperty { get; set; }

		[Category("Some Category")]
		public bool SomeOtherProperty { get; set; }

		public bool? NullableBoolProperty { get; set; }

		public DateTime DateTimeProperty { get; set; }

		public DateTime? NullableDateTimeProperty { get; set; }

		[DisplayName("Some Other Name")]
		public string PropertyWithNameThatShouldNotBeShown { get; set; }

		[Editor(typeof(CustomEditor), typeof(PropertyGridTypeEditor))]
		public string PropertyWithEditor { get; set; }

		[Description("This is a description.  It should be shown.")]
		public string PropertyWithDescription { get; set; }

		public bool ReadOnlyBoolean => true;

		public string ReadOnlyString => "You can't edit me!";

		public int ReadOnlyInt => 20;

		[ReadOnly(true)]
		public string ReadOnlyStringWithAttribute { get; set; } = "You can't edit me either!";

		public Color ReadOnlyColor => Colors.Blue;

		[Category("Lists")]
		public List<MyPropertyObject> ReadOnlyObjectListThatsNull { get; }

		[Category("Lists")]
		public List<MyPropertyObject> ReadOnlyObjectListWithValue { get; } = new List<MyPropertyObject>();

		[Category("Lists")]
		public List<int> ReadOnlyIntListWithValue { get; } = new List<int> { 5, 6, 7 };

		[Category("Lists")]
		public IList<float> ReadOnlyFloatIListWithValue { get; } = new List<float> { 3, 4, 5 };

		[Category("Arrays")]
		public int[] ReadOnlyIntArray { get; } = new int[] { 10, 11, 12 };


		[Category("Numbers")]
		public byte ByteValue { get; set; }
		[Category("Numbers")]
		public short ShortValue { get; set; }
		[Category("Numbers")]
		public int IntValue { get; set; }
		[Category("Numbers")]
		public long LongValue { get; set; }
		[Category("Numbers")]
		public decimal DecimalValue { get; set; }
		[Category("Numbers")]
		public float FloatValue { get; set; }
		[Category("Numbers")]
		public double DoubleValue { get; set; }


		[Category("Numbers")]
		public byte? NullableByteValue { get; set; }
		[Category("Numbers")]
		public short? NullableShortValue { get; set; }
		[Category("Numbers")]
		public int? NullableIntValue { get; set; }
		[Category("Numbers")]
		public long? NullableLongValue { get; set; }
		[Category("Numbers")]
		public decimal? NullableDecimalValue { get; set; }
		[Category("Numbers")]
		public float? NullableFloatValue { get; set; }
		[Category("Numbers")]
		public double? NullableDoubleValue { get; set; }


		[Category("Objects")]
		public MyExpandableObject ExpandableObjectNull { get; set; }
		[Category("Objects")]
		public MyExpandableObject ExpandableObject { get; set; } = new MyExpandableObject();
		[Category("Objects")]
		public MyExpandableObject ReadOnlyExpandableObject { get; } = new MyExpandableObject();
		[Category("Objects")]
		public MyExpandableObject ReadOnlyExpandableObjectNull { get; }
	}

	class MyOtherObjectConverter : sc.TypeConverter
	{
		public override bool CanConvertTo(sc.ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(sc.ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is MyOtherObject otherObject && destinationType == typeof(string))
				return otherObject.TextProperty;
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}

	[sc.TypeConverter(typeof(MyOtherObjectConverter))]
	public class MyOtherObject
	{
		string _textProperty;
		bool _boolProperty;
		public string TextProperty
		{
			get => _textProperty;
			set => _textProperty = value;
		}

		public bool BoolProperty
		{
			get => _boolProperty;
			set => _boolProperty = value;
		}

		public int[] IntArrayProperty { get; set; }

		public List<int> IntListProperty { get; set; }
	}

	public class MyExpandableObjectConverter : sc.ExpandableObjectConverter
	{

		public override bool CanConvertTo(sc.ITypeDescriptorContext context, Type destinationType)
		{
			return false;
			//return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(sc.ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return "Hello";
			//return base.ConvertTo(context, culture, value, destinationType);
		}

		public override PropertyDescriptorCollection GetProperties(sc.ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return base.GetProperties(context, value, attributes);
		}
	}

	[sc.TypeConverter(typeof(MyExpandableObjectConverter))]
	public class MyExpandableObject
	{
		string _textProperty;
		bool _boolProperty;
		public string TextProperty
		{
			get => _textProperty;
			set => _textProperty = value;
		}

		public bool BoolProperty
		{
			get => _boolProperty;
			set => _boolProperty = value;
		}

		public int[] IntArrayProperty { get; set; }

		public List<int> IntListProperty { get; set; }
	}}