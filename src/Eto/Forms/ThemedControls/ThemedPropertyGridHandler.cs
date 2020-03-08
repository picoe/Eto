using System;
using Eto.Drawing;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using sc = System.ComponentModel;
#if !NETSTANDARD1_0
using System.ComponentModel.DataAnnotations;
#endif

namespace Eto.Forms.ThemedControls
{
    /// <summary>
    /// Themed handler for the <see cref="PropertyGrid"/> control.
    /// </summary>
    public class ThemedPropertyGridHandler : ThemedControlHandler<ThemedPropertyGrid, PropertyGrid, PropertyGrid.ICallback>, PropertyGrid.IHandler
    {
        /// <summary>
        /// Gets or sets the selected object for the grid to edit
        /// </summary>
        public object SelectedObject
        {
            get => Control.SelectedObject;
            set => Control.SelectedObject = value;
        }

        /// <summary>
        /// Gets or sets the selected objects for the grid to edit
        /// </summary>
        /// <remarks>
        /// Only common properties (with the same name and type) will be shown.
        /// </remarks>
        public IEnumerable<object> SelectedObjects
        {
            get => Control.SelectedObjects;
            set => Control.SelectedObjects = value;
        }

        /// <summary>
        /// Gets or sets a value indicating that the categories should be shown
        /// </summary>
        public bool ShowCategories
        {
            get => Control.ShowCategories;
            set => Control.ShowCategories = value;
        }

        /// <summary>
        /// Gets or sets a value indicating that the description panel should be shown
        /// </summary>
        /// <remarks>
        /// The description panel shows the name and description of the selected property
        /// </remarks>
        public bool ShowDescription
        {
            get => Control.ShowDescription;
            set => Control.ShowDescription = value;
        }

        /// <summary>
        /// Refreshes the grid with new values from the selected object(s)
        /// </summary>
        public void Refresh() => Control.Refresh();

        /// <summary>
        /// Creates the control for this handler
        /// </summary>
        /// <returns>The control instance</returns>
        protected override ThemedPropertyGrid CreateControl() => new ThemedPropertyGrid();

		/// <summary>
		/// Attaches the specified event to the platform-specific control
		/// </summary>
		/// <remarks>Implementors should override this method to handle any events that the widget
		/// supports. Ensure to call the base class' implementation if the event is not
		/// one the specific widget supports, so the base class' events can be handled as well.</remarks>
		/// <param name="id">Identifier of the event</param>
		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case PropertyGrid.PropertyValueChangedEvent:
					Control.PropertyValueChanged += Control_PropertyValueChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void Control_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
		{
			Callback.OnPropertyValueChanged(Widget, e);
		}
	}

    /// <summary>
    /// Implementation of the PropertyGrid using the TreeGridView and PropertyCell
    /// </summary>
    public class ThemedPropertyGrid : Panel, IValueTypeWrapperHost
    {
        List<object> _selectedObjects;
        IComparer<IPropertyDescriptor> _propertySort = Comparer<IPropertyDescriptor>.Create((x, y) => string.Compare(x.Name, y.Name, StringComparison.CurrentCulture));
        IComparer<string> _categorySort = Comparer<string>.Default;
        bool _showCategories = true;
        bool _showDescription = true;
        TreeGridView _tree;
        PropertyCell _propertyCell;
        ObservableCollection<PropertyCellType> _customTypes;

        static Font s_DefaultFont = SystemFonts.Default();
        static Font s_BoldFont = SystemFonts.Bold();

        /// <summary>
        /// Event to handle when a property value has changed
        /// </summary>
        public event EventHandler<PropertyValueChangedEventArgs> PropertyValueChanged;

        /// <summary>
        /// Creates a cell value binding for a particular type when you want to add your own cell types to <see cref="PropertyCellTypes"/>
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <returns>A binding to use for custom cell types</returns>
        public IndirectBinding<T> CreateCellValueBinding<T>() => Binding.Property<T>(nameof(PropertyItem.Value));

        /// <summary>
        /// Gets a collection of custom property cell types to use as well as the built-in types.
        /// </summary>
        public IList<PropertyCellType> PropertyCellTypes => _customTypes ?? (_customTypes = CreateCustomTypes());


        ObservableCollection<PropertyCellType> CreateCustomTypes()
        {
            _customTypes = new ObservableCollection<PropertyCellType>();
            _customTypes.CollectionChanged += customTypes_CollectionChanged;
            return _customTypes;

        }

        private void customTypes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetupTypes();
        }

        void SetupTypes()
        {
            if (_propertyCell == null)
                return;

            _propertyCell.Types.Clear();
            var itemTypeBinding = Binding.Property((PropertyItem p) => p.PropertyType);

            if (_customTypes != null)
            {
                for (int i = 0; i < _customTypes.Count; i++)
                {
                    _propertyCell.Types.Add(_customTypes[i]);
                }
            }

            _propertyCell.Types.Add(new PropertyCellTypeArray());
            _propertyCell.Types.Add(new PropertyCellTypeCustom());
            _propertyCell.Types.Add(new PropertyCellTypeReadOnly());
            _propertyCell.Types.Add(Setup(new PropertyCellTypeColor()));
            _propertyCell.Types.Add(Setup(new PropertyCellTypeString()));
            _propertyCell.Types.Add(Setup(new PropertyCellTypeNumber { ItemTypeBinding = itemTypeBinding }));
            _propertyCell.Types.Add(Setup(new PropertyCellTypeBoolean { ItemThreeStateBinding = Binding.Property((PropertyItem p) => p.IsNullable) }));
            _propertyCell.Types.Add(Setup(new PropertyCellTypeDateTime()));
            _propertyCell.Types.Add(Setup(new PropertyCellTypeEnum { ItemTypeBinding = itemTypeBinding }));
            _propertyCell.Types.Add(new PropertyCellTypeObject());
        }

        /// <summary>
        /// Gets or sets the selected objects for the grid to edit
        /// </summary>
        /// <remarks>
        /// Only common properties (with the same name and type) will be shown.
        /// </remarks>
        public IEnumerable<object> SelectedObjects
        {
            get => _selectedObjects?.Select(UnwrapValueType);
            set
            {
                _selectedObjects = WrapValueType(value);
                UpdateProperties();
            }
        }

        object UnwrapValueType(object value)
        {
            if (value is IValueTypeWrapper wrapper)
                return _selectedObjects[(int)wrapper.Key];
            return value;
        }

        List<object> WrapValueType(IEnumerable<object> values)
        {
            if (values == null)
                return null;
            var newValues = new List<object>();
            var index = 0;
            foreach (var value in values)
            {
                newValues.Add(WrapValueType(value, index++));
            }
            return newValues;
        }

        object WrapValueType(object value, int index)
        {
            if (value == null)
                return null;
            // can't mutate it, so we wrap!
            return ValueTypeWrapper.Wrap(this, index, value);
        }

        /// <summary>
        /// Gets or sets a value indicating that value types should use their default to determine if the property is changed (shown as bold)
        /// </summary>
        public bool UseValueTypeDefaults { get; set; } = true;

        /// <summary>
        /// Gets or sets the selected object 
        /// </summary>
        public object SelectedObject
        {
            get => _selectedObjects?.FirstOrDefault();
            set => SelectedObjects = value != null ? new[] { value } : null;
        }

        /// <summary>
        /// Event to handle when the <see cref="ShowCategories"/> property has changed.
        /// </summary>
        public event EventHandler<EventArgs> ShowCategoriesChanged;

        /// <summary>
        /// Gets or sets a value indicating that the categories should be shown
        /// </summary>
        [DefaultValue(true)]
        public bool ShowCategories
        {
            get => _showCategories;
            set
            {
                if (_showCategories != value)
                {
                    _showCategories = value;
                    UpdateProperties();
                    ShowCategoriesChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating that the description panel should be shown
        /// </summary>
        /// <remarks>
        /// The description panel shows the name and description of the selected property
        /// </remarks>
        [DefaultValue(true)]
        public bool ShowDescription
        {
            get => _showDescription;
            set
            {
                if (_showDescription != value)
                {
                    _showDescription = value;
                    CreateLayout();
                }
            }
        }

        IComparer<IPropertyDescriptor> PropertySort
        {
            get => _propertySort;
            set
            {
                _propertySort = value;
                UpdateProperties();
            }
        }

        IComparer<string> CategorySort
        {
            get => _categorySort;
            set
            {
                _categorySort = value;
                UpdateProperties();
            }
        }

        interface IItem
        {
            string Name { get; }
            Color TitleTextColor { get; }
            Font TitleFont { get; }
        }

        class CategoryItem : TreeGridItem, IItem
        {
            public string Name { get; set; }

            public Color TitleTextColor => SystemColors.ControlText;

            public Font TitleFont => s_BoldFont;
        }

        class PropertyItem : TreeGridItem, INotifyPropertyChanged, ITreeGridStore<ITreeGridItem>, IItem
        {
            ThemedPropertyGrid _grid;
            string _description;
            bool? _isReadOnly;
            object _propertyCellType;
            object _defaultValue;
            string _name;
            Lazy<string> _dataTypeText;
            Lazy<Type> _elementType;
            Lazy<PropertyGridTypeEditor> _editor;
            bool? _expandable;
            Lazy<sc.TypeConverter> _converter;
            bool _childrenInitialized;

            public event PropertyChangedEventHandler PropertyChanged;

            void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

            public IPropertyDescriptor Property { get; set; }

            public object GetPropertyValue(object instance)
            {
                if (Parent is PropertyItem propertyItem)
                {
                    instance = propertyItem.GetPropertyValue(instance);
                }
                var prop = GetProperty(instance);
                if (prop == null)
                    return DefaultValue;
                return prop.GetValue(instance);
            }

            public bool SetPropertyValue(object instance, object value)
            {
                if (Parent is PropertyItem propertyItem)
                {
                    instance = propertyItem.GetPropertyValue(instance);
                }
                var prop = GetProperty(instance);
                if (prop == null || prop.IsReadOnly)
                    return false;

                prop.SetValue(instance, value);

                _childrenInitialized = false;

                return true;
            }

            public IPropertyDescriptor GetProperty(object instance)
            {
                if (instance == null)
                    return null;

                // cache per type?
                var instanceType = instance.GetType();
                var prop = Property;
                if (!prop.ComponentType.GetTypeInfo().IsAssignableFrom(instanceType.GetTypeInfo()))
                {
                    prop = EtoTypeDescriptor.GetProperty(instanceType, prop.Name);
                }
                return prop;
            }

            public string Name => _name ?? (_name = GetName());
            public string Description => _description ?? (_description = GetDescription());

            public bool IsReadOnly => _isReadOnly ?? (_isReadOnly = GetIsReadOnly()) ?? false;

            public object DefaultValue => _defaultValue ?? (_defaultValue = GetDefaultValue());

            public object PropertyCellType => _propertyCellType ?? (_propertyCellType = GetPropertyCellType());

            public Type ElementType => (_elementType ?? (_elementType = new Lazy<Type>(GetElementType))).Value;

            public Type PropertyType => Property.PropertyType;

            public string ReadOnlyValue => GetDisplayString();

            public bool IsNullable => PropertyType.GetTypeInfo().IsClass || Nullable.GetUnderlyingType(PropertyType) != null;

            public PropertyGridTypeEditor Editor => (_editor ?? (_editor = new Lazy<PropertyGridTypeEditor>(CreateEditor))).Value;

            PropertyGridTypeEditor CreateEditor()
            {
                var editorType = GetEditorType();
                if (editorType == null)
                    return null;
                return Activator.CreateInstance(editorType) as PropertyGridTypeEditor;
            }

            Type GetEditorType()
            {
                var editor = EditorAttributeWrapper.Get(Property) ?? EditorAttributeWrapper.Get(PropertyType);
                if (editor == null || editor.EditorBaseTypeName != typeof(PropertyGridTypeEditor).AssemblyQualifiedName)
                    return null;
                return Type.GetType(editor.EditorTypeName);
            }

            object GetPropertyCellType()
            {
                if (GetEditorType() != null)
                    return typeof(PropertyCellTypeCustom);

                if (IsReadOnly)
                    return typeof(PropertyCellTypeReadOnly);

                var type = PropertyType;
                if (type == typeof(object))
                {
                    // use current value for type
                    type = Value?.GetType() ?? type;
                }
                return type;
            }

            public sc.TypeConverter Converter => (_converter ?? (_converter = new Lazy<sc.TypeConverter>(GetConverter))).Value;

            public string DisplayText => GetDisplayString();

            string GetDisplayString()
            {
                return Converter?.ConvertToString(Value) ?? Convert.ToString(Value);
            }

            sc.TypeConverter GetConverter()
            {
                return sc.TypeDescriptor.GetConverter(PropertyType);
            }

            Type GetElementType()
            {
                if (PropertyType.HasElementType)
                    return PropertyType.GetElementType();
                else
                {
                    var interfaces = PropertyType.GetTypeInfo().ImplementedInterfaces;
                    foreach (var i in interfaces)
                    {
                        if (i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                            && i.GenericTypeArguments.Length == 1)
                        {
                            return i.GenericTypeArguments[0];
                        }
                    }
                }

                return null;
            }

            string GetName()
            {
                return DisplayAttributeWrapper.Get(Property)?.GetName()
                    ?? Property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
                    ?? Property.Name;
            }

            public PropertyItem(ThemedPropertyGrid grid, PropertyInfo property)
                : this(grid, new PropertyInfoDescriptor(property))
            {
            }

            public PropertyItem(ThemedPropertyGrid grid, IPropertyDescriptor property)
            {
                _grid = grid;
                Property = property;
            }

            public bool HasValue => Value != null;

            public object Value
            {
                get => _grid.GetValue(this);
                set
                {
                    _grid.SetValue(this, value);
                    OnPropertyChanged(nameof(DisplayFont));
                    _dataTypeText = null;
                    OnPropertyChanged(nameof(DataTypeText));
                    OnPropertyChanged(nameof(DisplayText));
                }
            }

            public override bool Expandable => _expandable ?? (_expandable = GetIsExpandable()).Value && HasValue;

            public string DataTypeText => (_dataTypeText ?? (_dataTypeText = new Lazy<string>(GetDataTypeText))).Value;

            string GetDataTypeText()
            {
                if (!HasValue)
                    return null;
                if (PropertyType.IsArray)
                    return string.Format(Application.Instance.Localize(_grid, "{0} Array"), PropertyType.Name);
                if (typeof(IList).GetTypeInfo().IsAssignableFrom(PropertyType.GetTypeInfo()))
                    return Application.Instance.Localize(_grid, "(Collection)");

                return null;
            }

            static MethodInfo s_GetPropertiesSupportedMethod = typeof(sc.TypeConverter).GetRuntimeMethod("GetPropertiesSupported", new Type[0]);
            static MethodInfo s_GetPropertiesMethod = typeof(sc.TypeConverter).GetRuntimeMethod("GetProperties", new Type[] { typeof(object) });

            public bool GetIsExpandable()
            {
                if (s_GetPropertiesSupportedMethod == null || Converter == null)
                    return false;
                return (bool)s_GetPropertiesSupportedMethod.Invoke(Converter, null);
            }

            public bool CanSetCollection
            {
                get
                {
                    if (PropertyType.IsArray)
                        return !IsReadOnly;

                    return !IsReadOnly || HasValue;
                }
            }

            public bool IsDefaultValue => Equals(Value, DefaultValue);

            public Font DisplayFont => IsReadOnly || IsDefaultValue
                ? s_DefaultFont
                : s_BoldFont;

            public Color DisplayTextColor
            {
                get
                {
                    if (IsReadOnly)
                        return IsSelected ? new Color(SystemColors.HighlightText, 0.8f) : SystemColors.DisabledText;
                    else
                        return IsSelected ? SystemColors.HighlightText : SystemColors.ControlText;
                }
            }

            object GetDefaultValue()
            {
                var attr = Property.GetCustomAttribute<DefaultValueAttribute>();
                if (attr != null)
                {
                    var val = attr.Value;
                    if (val != null)
                    {
                        var tc = sc.TypeDescriptor.GetConverter(PropertyType);
                        if (tc != null && tc.CanConvertFrom(val.GetType()))
                            val = tc.ConvertFrom(val);
                    }
                    return val;
                }
                if (_grid.UseValueTypeDefaults && PropertyType.GetTypeInfo().IsValueType)
                {
                    // is it nullable?  return null for default.
                    if (IsNullable)
                        return null;
                    return Activator.CreateInstance(PropertyType);
                }
                return null;
            }

            string GetDescription()
            {
                return DisplayAttributeWrapper.Get(Property)?.GetDescription()
                    ?? Property.GetCustomAttribute<DescriptionAttribute>()?.Description;
            }

            bool IsArray => PropertyType.IsArray;

            bool IsCollection
            {
                get
                {
                    if (typeof(IList).GetTypeInfo().IsAssignableFrom(PropertyType.GetTypeInfo()))
                        return true;
                    return false;
                }
            }

            bool GetIsReadOnly()
            {
                var isReadOnlyAttribute = Property.GetCustomAttribute<ReadOnlyAttribute>();
                if (isReadOnlyAttribute != null)
                    return isReadOnlyAttribute.IsReadOnly;

                if (!Property.IsReadOnly)
                    return false;

                // can't use object editor for string
                if (PropertyType == typeof(string))
                    return true;

                // array editor
                if (IsCollection && !IsArray)
                    return !HasValue;

                var typeInfo = PropertyType.GetTypeInfo();

                // we can use the object editor to edit a read-only property
                if (!(typeInfo.IsValueType || typeInfo.IsArray || typeInfo.IsInterface))
                    return !HasValue;

                return true;
            }

            public object CreateNewValue()
            {
                if (PropertyType.GetConstructor(new Type[0]) == null)
                    return null;
                return Activator.CreateInstance(PropertyType);
            }

            public void SetCollection(IEnumerable<object> newCollection)
            {
                if (PropertyType.IsArray)
                {
                    // it's an array, we need to replace the whole thing
                    var coll = newCollection.ToList();
                    var newArray = Array.CreateInstance(ElementType, coll.Count);
                    int index = 0;
                    for (int i = 0; i < coll.Count; i++)
                    {
                        newArray.SetValue(coll[i], index++);
                    }
                    Value = newArray;
                }

                var value = Value;
                bool setValue = false;
                if (value == null)
                {
                    value = CreateNewValue();
                    setValue = true;
                }

                if (value is IList saveList && !saveList.IsFixedSize)
                {
                    saveList.Clear();

                    foreach (var item in newCollection)
                    {
                        saveList.Add(item);
                    }

                    if (setValue)
                    {
                        Value = value;
                    }
                }

            }

            void EnsureChildren()
            {
                if (_childrenInitialized || s_GetPropertiesMethod == null || Converter == null || !HasValue || !PropertyDescriptorDescriptor.IsSupported)
                    return;

                _childrenInitialized = true;
                Children.Clear();
                var properties = (IList)s_GetPropertiesMethod.Invoke(Converter, new object[] { Value });
                if (properties == null)
                    return;

                var descriptors = new List<IPropertyDescriptor>();
                for (int i = 0; i < properties.Count; i++)
                {
					var descriptor = EtoTypeDescriptor.Get(properties[i]);
					if (descriptor != null)
						descriptors.Add(descriptor);
                }

                if (_grid.PropertySort != null)
                    descriptors.Sort(_grid.PropertySort);

                for (int i = 0; i < descriptors.Count; i++)
                {
                    Children.Add(new PropertyItem(_grid, descriptors[i]));
                }
            }

            int IDataStore<ITreeGridItem>.Count
            {
                get
                {
                    EnsureChildren();
                    return base.Count;
                }
            }

            bool _isSelected;
            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    if (_isSelected != value)
                    {
                        _isSelected = value;
                        OnPropertyChanged();
                        OnPropertyChanged(nameof(DisplayTextColor));
                    }
                }
            }

            public Color TitleTextColor => DisplayTextColor;

            public Font TitleFont => s_DefaultFont;
        }

        private void SetValue(PropertyItem propertyItem, object value)
        {
            if (_selectedObjects == null)
                return;

            var wasUpdated = false;
            for (int i = 0; i < _selectedObjects.Count; i++)
            {
                object obj = _selectedObjects[i];
                var oldValue = propertyItem.GetPropertyValue(obj);
                if (propertyItem.SetPropertyValue(obj, value))
                {
                    wasUpdated = true;
                    PropertyValueChanged?.Invoke(this, new PropertyValueChangedEventArgs(propertyItem.Name, oldValue, obj));
                }
            }
            if (wasUpdated && propertyItem.Expandable)
                _tree.ReloadItem(propertyItem, true);
        }

        private object GetValue(PropertyItem propertyItem)
        {
            if (_selectedObjects?.Count > 0)
            {
                return propertyItem?.GetPropertyValue(_selectedObjects[0]);
            }
            return propertyItem.DefaultValue;
        }

        PropertyCellType<T> Setup<T>(PropertyCellType<T> cell)
        {
            cell.ItemBinding = CreateCellValueBinding<T>();
            return cell;
        }

        class CollectionEditorDialog : Dialog<bool>
        {
            public CollectionEditor Editor { get; }

            static Size? s_defaultSize;

            public CollectionEditorDialog(CollectionEditor editor)
            {
                Editor = editor;
                Resizable = true;
                Padding = 6;
                MinimumSize = new Size(200, 200);
                ClientSize = new Size(600, 400);
                if (s_defaultSize != null)
                    Size = s_defaultSize.Value;

                var cancelButton = new Button { Text = Application.Instance.Localize(this, "Cancel") };
                cancelButton.Click += CancelButton_Click;
                var okButton = new Button { Text = Application.Instance.Localize(this, Platform.IsMac ? "Apply" : "OK") };
                okButton.Click += OkButton_Click;
                DefaultButton = okButton;
                AbortButton = cancelButton;

                if (editor.ControlObject is ThemedCollectionEditor themedCollectionEditor)
                {
                    var buttons = new TableLayout
                    {
                        Padding = 4,
                        Spacing = new Size(6, 0),
                        Rows = { null, new TableRow(cancelButton, okButton), null }
                    };
                    themedCollectionEditor.ExtraContent = buttons;
                    Content = editor;
                }
                else
                {
                    Content = editor;
                    PositiveButtons.Add(okButton);
                    NegativeButtons.Add(cancelButton);
                }
            }

            protected override void OnClosing(CancelEventArgs e)
            {
                base.OnClosing(e);
                s_defaultSize = RestoreBounds.Size;
            }

            private void OkButton_Click(object sender, EventArgs e) => Close(true);

            private void CancelButton_Click(object sender, EventArgs e) => Close(false);
        }


        class PropertyCellTypeArray : PropertyCellType
        {
            public override string Identifier => "PropertyCellTypeArray";

            public override bool CanDisplay(object itemType)
            {
                var type = itemType as Type;
                if (type == null)
                    return false;
                if (type.IsArray)
                    return true;
                if (typeof(IList).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                    return true;
                return false;
            }

            public override Control OnCreate(CellEventArgs args)
            {
                var label = new Label();
                label.VerticalAlignment = VerticalAlignment.Center;
                label.TextBinding.BindDataContext((PropertyItem p) => p.DataTypeText);
                var button = new Button { MinimumSize = Size.Empty, Text = "…" };
                button.BindDataContext(c => c.Enabled, (PropertyItem p) => p.CanSetCollection);
                button.Click += (sender, e) =>
                {
                    var b = sender as Button;
                    if (b.DataContext is PropertyItem pi)
                    {
                        var elementType = pi.ElementType;
                        var value = pi.Value ?? pi.CreateNewValue();
                        var collection = value as IEnumerable<object>;
                        if (collection == null && value is IList list)
                            collection = list.OfType<object>();

                        var editor = new CollectionEditor();
                        editor.ElementType = pi.ElementType;
                        editor.DataStore = collection;
                        var editorDialog = new CollectionEditorDialog(editor);
                        editorDialog.Title = $"{pi.ElementType.Name} Collection Editor";

                        if (editorDialog.ShowModal(button))
                        {
                            pi.SetCollection(editor.DataStore);

                        }
                    }
                };
                return new TableLayout(new TableRow(new TableCell(label, true), button));
            }

            public override void OnPaint(CellPaintEventArgs args)
            {
                if (args.Item is PropertyItem pi)
                {
                    pi.IsSelected = args.IsSelected;
                    var color = pi.DisplayTextColor;
                    var font = pi.DisplayFont;
                    args.DrawCenteredText(pi.DataTypeText, color, font);
                }
            }
        }


        class PropertyGridDialog : Dialog<bool>
        {
            public ThemedPropertyGrid Editor { get; }

            static Size? s_defaultSize;

            public PropertyGridDialog(ThemedPropertyGrid editor)
            {
                Editor = editor;
                Resizable = true;
                MinimumSize = new Size(200, 200);
                ClientSize = new Size(400, 400);
                if (s_defaultSize != null)
                    Size = s_defaultSize.Value;

                var cancelButton = new Button { Text = Application.Instance.Localize(this, "Cancel") };
                cancelButton.Click += CancelButton_Click;
                var okButton = new Button { Text = Application.Instance.Localize(this, Platform.IsMac ? "Apply" : "OK") };
                okButton.Click += OkButton_Click;
                DefaultButton = okButton;
                AbortButton = cancelButton;

                var layout = new DynamicLayout();
                layout.DefaultSpacing = new Size(4, 4);
                layout.Add(editor, yscale: true);
                layout.AddSeparateRow(spacing: new Size(6, 0), controls: new[] { null, cancelButton, okButton });
                Padding = 6;
                Content = layout;
            }

            protected override void OnClosing(CancelEventArgs e)
            {
                base.OnClosing(e);
                s_defaultSize = RestoreBounds.Size;
            }

            private void OkButton_Click(object sender, EventArgs e) => Close(true);

            private void CancelButton_Click(object sender, EventArgs e) => Close(false);
        }

        class PropertyCellTypeObject : PropertyCellType
        {
            public override string Identifier => "PropertyCellTypeObject";

            public override bool CanDisplay(object itemType)
            {
                var type = itemType as Type;
                if (type == null)
                    return false;
                var typeInfo = type.GetTypeInfo();
                if (typeInfo.IsValueType || typeInfo.IsArray || typeInfo.IsInterface)
                    return false;
                return true;
            }

            public override Control OnCreate(CellEventArgs args)
            {
                var label = new Label();
                label.VerticalAlignment = VerticalAlignment.Center;
                label.TextBinding.BindDataContext((PropertyItem p) => p.DisplayText);
                var button = new Button { MinimumSize = Size.Empty, Text = "…" };
                button.BindDataContext(c => c.Enabled, Binding.Property((PropertyItem p) => p.IsReadOnly).ToBool(false, true));
                button.BindDataContext(c => c.Visible, Binding.Property((PropertyItem p) => p.Expandable).ToBool(false, true));
                button.Click += (sender, e) =>
                {
                    var b = sender as Button;
                    if (b.DataContext is PropertyItem pi)
                    {
                        var editor = new ThemedPropertyGrid();
                        var value = pi.Value ?? pi.CreateNewValue();
                        editor.SelectedObject = value;
                        var editorDialog = new PropertyGridDialog(editor);
                        editorDialog.Title = $"{pi.PropertyType.Name} Editor";

                        if (editorDialog.ShowModal(button))
                        {
                            if (!pi.HasValue)
                                pi.Value = value;

                        }
                    }
                };
                return new TableLayout(new TableRow(new TableCell(label, true), button));
            }

            public override void OnPaint(CellPaintEventArgs args)
            {
                if (args.Item is PropertyItem pi)
                {
                    pi.IsSelected = args.IsSelected;
                    var color = pi.DisplayTextColor;
                    var font = pi.DisplayFont;
                    args.DrawCenteredText(pi.DisplayText, color, font);
                }
            }
        }

        class PropertyCellTypeReadOnly : PropertyCellType
        {
            public override string Identifier => "PropertyCellTypeReadOnly";

            public override bool CanDisplay(object itemType) => itemType as Type == typeof(PropertyCellTypeReadOnly);

            public override Control OnCreate(CellEventArgs args)
            {
                var control = new TextBox { ShowBorder = false, BackgroundColor = Colors.Transparent, ReadOnly = true };
                control.BindDataContext(c => c.Text, (PropertyItem p) => p.ReadOnlyValue);
                control.BindDataContext(c => c.ToolTip, (PropertyItem p) => p.ReadOnlyValue);
                return control;
            }

            public override void OnConfigure(CellEventArgs args, Control control)
            {
                base.OnConfigure(args, control);
                if (control is TextBox tb)
                {
                    if (args.IsSelected && !Eto.Platform.Instance.IsMac)
                        tb.TextColor = new Color(SystemColors.HighlightText, 0.8f);
                    else
                        tb.TextColor = SystemColors.DisabledText;
                }
            }

            public override void OnPaint(CellPaintEventArgs args)
            {
                if (args.Item is PropertyItem pi)
                {
                    pi.IsSelected = args.IsSelected;
                    var color = pi.DisplayTextColor;
                    var font = pi.DisplayFont;
                    args.DrawCenteredText(pi.ReadOnlyValue, color, font);
                }
            }
        }

        class PropertyCellTypeCustom : PropertyCellType
        {
            public override string Identifier => "PropertyCellTypeCustom";

            public override bool CanDisplay(object itemType) => Equals(itemType, typeof(PropertyCellTypeCustom));

            public override Control OnCreate(CellEventArgs args)
            {
                if (args.Item is PropertyItem propertyItem)
                {
                    return propertyItem.Editor?.CreateControl(args);
                }

                return null;
            }

            public override void OnPaint(CellPaintEventArgs args)
            {
                if (args.Item is PropertyItem propertyItem)
                {
                    propertyItem.Editor?.PaintCell(args);
                }
            }
        }

        class NameCell : CustomCell
        {
            Grid _parent;
            public NameCell(Grid parent)
            {
                _parent = parent;
            }
            protected override Control OnCreateCell(CellEventArgs args)
            {
                var control = new Label();
                control.VerticalAlignment = VerticalAlignment.Center;
                control.TextBinding.BindDataContext(Binding.Delegate((IItem c) => c.Name));
                control.BindDataContext(c => c.TextColor, (IItem m) => m.TitleTextColor);
                control.BindDataContext(c => c.Font, (IItem m) => m.TitleFont);
                args.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(args.IsSelected) && control.DataContext is PropertyItem pi)
                    {
                        pi.IsSelected = args.IsSelected && _parent.HasFocus;
                    }
                };
                return control;
            }

            protected override void OnPaint(CellPaintEventArgs args)
            {
                if (args.Item is IItem item)
                {
                    if (args.Item is PropertyItem pi)
                        pi.IsSelected = args.IsSelected;
                    args.DrawCenteredText(item.Name, item.TitleTextColor, item.TitleFont);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the ThemedPropertyGrid
        /// </summary>
        public ThemedPropertyGrid()
        {
            var orientation = Orientation.Vertical;

            switch (orientation)
            {
                case Orientation.Horizontal:
                    break;
                case Orientation.Vertical:
                    break;
                default:
                    break;
            }

            _tree = new TreeGridView();
            _tree.ShowHeader = false;
            _tree.SizeChanged += tree_SizeChanged;
            _tree.Columns.Add(new GridColumn
            {
                AutoSize = false,
                DataCell = new NameCell(_tree)
            });
            _propertyCell = new GridPropertyCell(this);
            _propertyCell.TypeBinding = Binding.Property((PropertyItem p) => p.PropertyCellType);
            SetupTypes();

            _tree.Columns.Add(new GridColumn
            {
                AutoSize = false,
                Editable = true,
                DataCell = _propertyCell
            });
            CreateLayout();
        }

        void CreateLayout()
        {
            if (ShowDescription)
            {
                var helpTitle = new Label { Font = SystemFonts.Bold() };
                helpTitle.TextBinding.BindDataContext((PropertyItem p) => p.Name);

                var helpText = new Label { Wrap = WrapMode.Word };
                helpText.TextBinding.BindDataContext((PropertyItem p) => p.Description);

                var helpPanel = new TableLayout(helpTitle, helpText);
                helpPanel.Bind(c => c.DataContext, _tree, t => t.SelectedItem);

                var splitter = new Splitter
                {
                    Orientation = Orientation.Vertical,
                    FixedPanel = SplitterFixedPanel.Panel2,
                    RelativePosition = 60,
                    Panel1 = _tree,
                    Panel2 = helpPanel
                };
                Content = splitter;
            }
            else
            {
                Content = _tree;
            }
        }

        class GridPropertyCell : PropertyCell
        {
            private ThemedPropertyGrid _grid;

            public GridPropertyCell(ThemedPropertyGrid grid)
            {
                _grid = grid;
            }

            protected override Control OnCreateCell(CellEventArgs args)
            {
                var ctl = base.OnCreateCell(args);
                if (ctl is Container container)
                {
                    container.Styles.Add<CommonControl>(null, child =>
                    {
                        if (child is Button)
                            return;
                        child.BindDataContext(c => c.Font, (PropertyItem p) => p.DisplayFont);
                    });
                    container.Styles.Add<TextControl>(null, child =>
                    {
                        if (child is Button)
                            return;
                        child.BindDataContext(c => c.TextColor, (PropertyItem p) => p.DisplayTextColor);
                    });
                }
                return ctl;
            }

        }


        private void tree_SizeChanged(object sender, EventArgs e)
        {
            SetColumnWidths();
        }

        private void SetColumnWidths()
        {
            var width = _tree.Size.Width;
            if (_tree.Border != BorderType.None)
                width -= 2;
            if (Platform.IsWinForms || Platform.IsWpf)
                width -= 18; // hack: for scrollbar, should use something like _tree.ClientSize
            width /= 2;
            var columns = _tree.Columns;
            for (int i = 0; i < columns.Count; i++)
            {
                columns[i].Width = width;
            }
        }

        /// <summary>
        /// Refreshes the properties of the grid
        /// </summary>
        public void Refresh()
        {
            UpdateProperties();
        }

        /// <summary>
        /// Called when the grid is loaded onto a form
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateProperties();
        }

        void UpdateProperties()
        {
            if (!Loaded)
                return;
            Task.Run(() =>
            {
                // do this stuff in the background
                var properties = GetCommonProperties();
                var top = new TreeGridItem();
                if (ShowCategories)
                {
                    var categories = SplitIntoCategories(properties);
                    //if (categories.Count > 1)
                    ToTreeWithCategories(top, categories);
                    //else
                    //    ToTreeWithProperties(top, categories.Values.FirstOrDefault());
                }
                else
                {
                    ToTreeWithProperties(top, properties);
                }
                Application.Instance.Invoke(() =>
                {
                    _tree.DataStore = top;

                    // on mac, the width is enlarged by the expanded nodes.. needs to be fixed.
                    SetColumnWidths();
                });
            });
        }

        private void ToTreeWithCategories(TreeGridItem top, Dictionary<string, IList<IPropertyDescriptor>> categories)
        {
            IEnumerable<string> categoryKeys = categories.Keys;
            if (CategorySort != null)
                categoryKeys = categoryKeys.OrderBy(c => c, CategorySort);
            foreach (var category in categoryKeys)
            {
                var categoryItem = new CategoryItem();
                categoryItem.Expanded = true;
                categoryItem.Name = category;

                ToTreeWithProperties(categoryItem, categories[category]);
                top.Children.Add(categoryItem);
            }
        }

        private void ToTreeWithProperties(TreeGridItem categoryItem, IEnumerable<IPropertyDescriptor> properties)
        {
            if (properties == null)
                return;
            if (PropertySort != null)
                properties = properties.OrderBy(c => c, PropertySort);
            foreach (var property in properties)
            {
                var propertyItem = new PropertyItem(this, property);

                categoryItem.Children.Add(propertyItem);
            }
        }

        Dictionary<string, IList<IPropertyDescriptor>> SplitIntoCategories(IEnumerable<IPropertyDescriptor> properties)
        {
            var categories = new Dictionary<string, IList<IPropertyDescriptor>>();
            foreach (var prop in properties)
            {
                string categoryName =
                    DisplayAttributeWrapper.Get(prop)?.GetGroupName()
                    ?? prop.GetCustomAttribute<CategoryAttribute>()?.Category
                    ?? Application.Instance.Localize(this, "Misc");

                if (!categories.TryGetValue(categoryName, out var categoryProperties))
                {
                    categories[categoryName] = categoryProperties = new List<IPropertyDescriptor>();
                }
                categoryProperties.Add(prop);
            }

            return categories;
        }

        List<IPropertyDescriptor> GetTypeProperties(Type type)
        {
            List<IPropertyDescriptor> properties = new List<IPropertyDescriptor>();
            foreach (var prop in EtoTypeDescriptor.GetProperties(type))
            {
                if (!prop.CanRead)
                    continue;

                var browsable = prop.GetCustomAttribute<BrowsableAttribute>();
                if (browsable?.Browsable == false)
                    continue;

                properties.Add(prop);
            }
            return properties;
        }

        IEnumerable<IPropertyDescriptor> GetCommonProperties()
        {
            if (_selectedObjects == null)
                return Enumerable.Empty<IPropertyDescriptor>();
            List<IPropertyDescriptor> properties = null;
            var types = new HashSet<Type>();
            for (int i = 0; i < _selectedObjects.Count; i++)
            {
                object obj = _selectedObjects[i];
                if (obj == null)
                    continue;
                var type = obj.GetType();
                if (types.Contains(type))
                    continue;
                types.Add(type);
                var currentProperties = GetTypeProperties(type);

                if (properties == null)
                    properties = currentProperties;
                else
                {
                    for (int j = properties.Count - 1; j >= 0; j--)
                    {
                        var prop = properties[j];

                        var currentProp = currentProperties.FirstOrDefault(r => r.Name == prop.Name);
                        if (currentProp == null || currentProp.PropertyType != prop.PropertyType)
                            properties.RemoveAt(j);
                    }
                }
            }
            return properties ?? Enumerable.Empty<IPropertyDescriptor>();
        }

        object IValueTypeWrapperHost.GetValue(object key)
        {
            return _selectedObjects[(int)key];
        }

        void IValueTypeWrapperHost.SetValue(object key, object value)
        {
            _selectedObjects[(int)key] = value;
        }
    }
}
