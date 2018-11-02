using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Eto.Wpf.CustomControls.FontDialog
{
	/// <summary>
	/// Interaction logic for FontChooser.xaml
	/// </summary>
	/// <remarks>
	/// Initially from https://github.com/Microsoft/WPF-Samples, MIT license.
	/// </remarks>
	public partial class FontChooser : Window
    {
        #region Private fields and types

        ICollection<FontFamily> _familyCollection;          // see FamilyCollection property
        string _defaultSampleText;
        string _previewSampleText;
        string _pointsText;

		bool _populated;
        bool _updatePending;                                // indicates a call to OnUpdate is scheduled
        bool _familyListValid;                              // indicates the list of font families is valid
        bool _typefaceListValid;                            // indicates the list of typefaces is valid
        bool _typefaceListSelectionValid;                   // indicates the current selection in the typeface list is valid
        bool _previewValid;                                 // indicates the preview control is valid
        Dictionary<TabItem, TabState> _tabDictionary;       // state and logic for each tab
        DependencyProperty _currentFeature;
        TypographyFeaturePage _currentFeaturePage;

        static readonly double[] CommonlyUsedFontSizes = {
            3.0,    4.0,   5.0,   6.0,   6.5,
            7.0,    7.5,   8.0,   8.5,   9.0,
            9.5,   10.0,  10.5,  11.0,  11.5,
            12.0,  12.5,  13.0,  13.5,  14.0,
            15.0,  16.0,  17.0,  18.0,  19.0,
            20.0,  22.0,  24.0,  26.0,  28.0,  30.0,  32.0,  34.0,  36.0,  38.0,
            40.0,  44.0,  48.0,  52.0,  56.0,  60.0,  64.0,  68.0,  72.0,  76.0,
            80.0,  88.0,  96.0, 104.0, 112.0, 120.0, 128.0, 136.0, 144.0
        };

        // Specialized metadata object for font chooser dependency properties
        class FontPropertyMetadata : FrameworkPropertyMetadata
        {
            public readonly DependencyProperty TargetProperty;

            public FontPropertyMetadata(
                object defaultValue,
                PropertyChangedCallback changeCallback,
                DependencyProperty targetProperty
                )
                : base(defaultValue, changeCallback)
            {
                TargetProperty = targetProperty;
            }
        }

        // Specialized metadata object for typographic font chooser properties
        class TypographicPropertyMetadata : FontPropertyMetadata
        {
            public TypographicPropertyMetadata(object defaultValue, DependencyProperty targetProperty, TypographyFeaturePage featurePage, string sampleTextTag)
                : base(defaultValue, _callback, targetProperty)
            {
                FeaturePage = featurePage;
                SampleTextTag = sampleTextTag;
            }

            public readonly TypographyFeaturePage FeaturePage;
            public readonly string SampleTextTag;

            static readonly PropertyChangedCallback _callback = new PropertyChangedCallback(
                FontChooser.TypographicPropertyChangedCallback
                );
        }

        // Object used to initialize the right-hand side of the typographic properties tab
        class TypographyFeaturePage
        {
            public TypographyFeaturePage(Item[] items)
            {
                Items = items;
            }

            public TypographyFeaturePage(Type enumType)
            {
                string[] names = Enum.GetNames(enumType);
                Array values = Enum.GetValues(enumType);

                Items = new Item[names.Length];

                for (int i = 0; i < names.Length; ++i)
                {
                    Items[i] = new Item(names[i], values.GetValue(i));
                }
            }

            public static readonly TypographyFeaturePage BooleanFeaturePage = new TypographyFeaturePage(
                new Item[] { 
                    new Item("Disabled", false), 
                    new Item("Enabled", true) 
                    }
                );

            public static readonly TypographyFeaturePage IntegerFeaturePage = new TypographyFeaturePage(
                new Item[] { 
                    new Item("_0", 0), 
                    new Item("_1", 1), 
                    new Item("_2", 2), 
                    new Item("_3", 3), 
                    new Item("_4", 4), 
                    new Item("_5", 5), 
                    new Item("_6", 6), 
                    new Item("_7", 7), 
                    new Item("_8", 8), 
                    new Item("_9", 9) 
                    }
                );

            public struct Item
            {
                public Item(string tag, object value)
                {
                    Tag = tag;
                    Value = value;
                }
                public readonly string Tag;
                public readonly object Value;
            }

            public readonly Item[] Items;
        }

        delegate void UpdateCallback();

        // Encapsulates the state and initialization logic of a tab control item.
        class TabState
        {
            public TabState(UpdateCallback initMethod)
            {
                InitializeTab = initMethod;
            }

            public bool IsValid;
            public readonly UpdateCallback InitializeTab;
        }

        #endregion

        #region Construction and initialization

        public FontChooser()
        {
			InitializeComponent();
			Loaded += FontChooser_Loaded;
        }

		void FontChooser_Loaded (object sender, RoutedEventArgs e)
		{
			SizeToContent = System.Windows.SizeToContent.Manual;
			selectionControls.Height = double.NaN;
			tabControl.Height = tabControl.Width = double.NaN;
			preview.Height = double.NaN;
			_previewSampleText = _defaultSampleText = previewTextBox.Text;
			_pointsText = typefaceNameRun.Text;

			// Hook up events for the font family list and associated text box.
			fontFamilyTextBox.SelectionChanged += new RoutedEventHandler (fontFamilyTextBox_SelectionChanged);
			fontFamilyTextBox.TextChanged += new TextChangedEventHandler (fontFamilyTextBox_TextChanged);
			fontFamilyTextBox.PreviewKeyDown += new KeyEventHandler (fontFamilyTextBox_PreviewKeyDown);
			fontFamilyList.SelectionChanged += new SelectionChangedEventHandler (fontFamilyList_SelectionChanged);

			// Hook up events for the typeface list.
			typefaceList.SelectionChanged += new SelectionChangedEventHandler (typefaceList_SelectionChanged);

			// Hook up events for the font size list and associated text box.
			sizeTextBox.TextChanged += new TextChangedEventHandler (sizeTextBox_TextChanged);
			sizeTextBox.PreviewKeyDown += new KeyEventHandler (sizeTextBox_PreviewKeyDown);
			sizeList.SelectionChanged += new SelectionChangedEventHandler (sizeList_SelectionChanged);

			// Hook up events for text decoration check boxes.
			var textDecorationEventHandler = new RoutedEventHandler (textDecorationCheckStateChanged);
			underlineCheckBox.Checked += textDecorationEventHandler;
			underlineCheckBox.Unchecked += textDecorationEventHandler;
			baselineCheckBox.Checked += textDecorationEventHandler;
			baselineCheckBox.Unchecked += textDecorationEventHandler;
			strikethroughCheckBox.Checked += textDecorationEventHandler;
			strikethroughCheckBox.Unchecked += textDecorationEventHandler;
			overlineCheckBox.Checked += textDecorationEventHandler;
			overlineCheckBox.Unchecked += textDecorationEventHandler;

			// Initialize the dictionary that maps tab control items to handler objects.
			_tabDictionary = new Dictionary<TabItem, TabState> (tabControl.Items.Count);
			_tabDictionary.Add (samplesTab, new TabState (new UpdateCallback (InitializeSamplesTab)));
			_tabDictionary.Add (typographyTab, new TabState (new UpdateCallback (InitializeTypographyTab)));
			_tabDictionary.Add (descriptiveTextTab, new TabState (new UpdateCallback (InitializeDescriptiveTextTab)));

			// Hook up events for the tab control.
			tabControl.SelectionChanged += new SelectionChangedEventHandler (tabControl_SelectionChanged);

			// Initialize the list of font sizes and select the nearest size.
			foreach (double value in CommonlyUsedFontSizes) {
				sizeList.Items.Add (new FontSizeListItem (value));
			}
			OnSelectedFontSizeChanged (SelectedFontSize);

			// Initialize the font family list and the current family.
			if (!_familyListValid) {
				InitializeFontFamilyList ();
				_familyListValid = true;
				OnSelectedFontFamilyChanged (SelectedFontFamily);
			}

			// Schedule background updates.
			_populated = true;
			ScheduleUpdate ();
		}

        #endregion

        #region Event handlers

        void OnOKButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        void OnCancelButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        int _fontFamilyTextBoxSelectionStart;

        void fontFamilyTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _fontFamilyTextBoxSelectionStart = fontFamilyTextBox.SelectionStart;
        }

        void fontFamilyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = fontFamilyTextBox.Text;

            // Update the current list item.
            if (SelectFontFamilyListItem(text) == null)
            {
                // The text does not exactly match a family name so consider applying auto-complete behavior.
                // However, only do so if the following conditions are met:
                //   (1)  The user is typing more text rather than deleting (i.e., the new text length is
                //        greater than the most recent selection start index), and
                //   (2)  The caret is at the end of the text box.
                if (text.Length > _fontFamilyTextBoxSelectionStart 
                    && fontFamilyTextBox.SelectionStart == text.Length)
                {
                    // Get the current list item, which should be the nearest match for the text.
                    var item = fontFamilyList.Items.CurrentItem as FontFamilyListItem;
                    if (item != null)
                    {
                        // Does the text box text match the beginning of the family name?
                        string familyDisplayName = item.ToString();
                        if (string.Compare(text, 0, familyDisplayName, 0, text.Length, true, CultureInfo.CurrentCulture) == 0)
                        {
                            // Set the text box text to the complete family name and select the part not typed in.
                            fontFamilyTextBox.Text = familyDisplayName;
                            fontFamilyTextBox.SelectionStart = text.Length;
                            fontFamilyTextBox.SelectionLength = familyDisplayName.Length - text.Length;
                        }
                    }
                }
            }
        }

        void sizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double sizeInPoints;
            if (double.TryParse(sizeTextBox.Text, out sizeInPoints))
            {
                double sizeInPixels = FontSizeListItem.PointsToPixels(sizeInPoints);
                if (!FontSizeListItem.FuzzyEqual(sizeInPixels, SelectedFontSize))
                {
                    SelectedFontSize = sizeInPixels;
                }
            }
        }

        void fontFamilyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            OnComboBoxPreviewKeyDown(fontFamilyTextBox, fontFamilyList, e);
        }

        void sizeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            OnComboBoxPreviewKeyDown(sizeTextBox, sizeList, e);
        }

        void fontFamilyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = fontFamilyList.SelectedItem as FontFamilyListItem;
            if (item != null)
            {
                SelectedFontFamily = item.FontFamily;
            }
        }

        void sizeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = sizeList.SelectedItem as FontSizeListItem;
            if (item != null)
            {
                SelectedFontSize = item.SizeInPixels;
            }
        }

        void typefaceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = typefaceList.SelectedItem as TypefaceListItem;
            if (item != null)
            {
                SelectedFontWeight = item.FontWeight;
                SelectedFontStyle = item.FontStyle;
                SelectedFontStretch = item.FontStretch;
            }
        }

        void textDecorationCheckStateChanged(object sender, RoutedEventArgs e)
        {
            var textDecorations = new TextDecorationCollection();

            if (underlineCheckBox.IsChecked.Value)
            {
                textDecorations.Add(TextDecorations.Underline[0]);
            }
            if (baselineCheckBox.IsChecked.Value)
            {
                textDecorations.Add(TextDecorations.Baseline[0]);
            }
            if (strikethroughCheckBox.IsChecked.Value)
            {
                textDecorations.Add(TextDecorations.Strikethrough[0]);
            }
            if (overlineCheckBox.IsChecked.Value)
            {
                textDecorations.Add(TextDecorations.OverLine[0]);
            }

            textDecorations.Freeze();
            SelectedTextDecorations = textDecorations;
        }

        void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabState tab = CurrentTabState;
            if (tab != null && !tab.IsValid)
            {
                tab.InitializeTab();
                tab.IsValid = true;
            }
        }

        void featureList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeTypographyTab();
        }

        #endregion

        #region Public properties and methods

        /// <summary>
        /// Collection of font families to display in the font family list. By default this is Fonts.SystemFontFamilies,
        /// but a client could set this to another collection returned by Fonts.GetFontFamilies, e.g., a collection of
        /// application-defined fonts.
        /// </summary>
        public ICollection<FontFamily> FontFamilyCollection
        {
            get
            {
				return _familyCollection ?? Fonts.SystemFontFamilies;
            }

            set
            {
                if (value != _familyCollection)
                {
                    _familyCollection = value;
                    InvalidateFontFamilyList();
                }
            }
        }

        /// <summary>
        /// Sets the font chooser selection properties to match the properites of the specified object.
        /// </summary>
        public void SetPropertiesFromObject(DependencyObject obj)
        {
            foreach (DependencyProperty property in _chooserProperties)
            {
                var metadata = property.GetMetadata(typeof(FontChooser)) as FontPropertyMetadata;
                if (metadata != null)
                {
                    SetValue(property, obj.GetValue(metadata.TargetProperty));
                }
            }
        }

        /// <summary>
        /// Sets the properites of the specified object to match the font chooser selection properties.
        /// </summary>
        public void ApplyPropertiesToObject(DependencyObject obj)
        {
            foreach (DependencyProperty property in _chooserProperties)
            {
                var metadata = property.GetMetadata(typeof(FontChooser)) as FontPropertyMetadata;
                if (metadata != null)
                {
                    obj.SetValue(metadata.TargetProperty, GetValue(property));
                }
            }
        }

        void ApplyPropertiesToObjectExcept(DependencyObject obj, DependencyProperty except)
        {
            foreach (DependencyProperty property in _chooserProperties)
            {
                if (property != except)
                {
                    var metadata = property.GetMetadata(typeof(FontChooser)) as FontPropertyMetadata;
                    if (metadata != null)
                    {
                        obj.SetValue(metadata.TargetProperty, GetValue(property));
                    }
                }
            }
        }

        /// <summary>
        /// Sample text used in the preview box and family and typeface samples tab.
        /// </summary>
        public string PreviewSampleText
        {
            get 
            { 
                return _previewSampleText; 
            }

            set 
            {
                string newValue = string.IsNullOrEmpty(value) ? _defaultSampleText : value;
                if (newValue != _previewSampleText)
                {
                    _previewSampleText = newValue;

                    // Update the preview text box.
                    previewTextBox.Text = newValue;

                    // The preview sample text is also used in the family and typeface samples tab.
                    InvalidateTab(samplesTab);
                }
            }
        }

        #endregion

        #region Dependency properties for typographic features

        public static readonly DependencyProperty StandardLigaturesProperty = RegisterTypographicProperty(Typography.StandardLigaturesProperty);
        public bool StandardLigatures
        {
            get { return (bool)GetValue(StandardLigaturesProperty); }
            set { SetValue(StandardLigaturesProperty, value); }
        }

        public static readonly DependencyProperty ContextualLigaturesProperty = RegisterTypographicProperty(Typography.ContextualLigaturesProperty);
        public bool ContextualLigatures
        {
            get { return (bool)GetValue(ContextualLigaturesProperty); }
            set { SetValue(ContextualLigaturesProperty, value); }
        }

        public static readonly DependencyProperty DiscretionaryLigaturesProperty = RegisterTypographicProperty(Typography.DiscretionaryLigaturesProperty);
        public bool DiscretionaryLigatures
        {
            get { return (bool)GetValue(DiscretionaryLigaturesProperty); }
            set { SetValue(DiscretionaryLigaturesProperty, value); }
        }

        public static readonly DependencyProperty HistoricalLigaturesProperty = RegisterTypographicProperty(Typography.HistoricalLigaturesProperty);
        public bool HistoricalLigatures
        {
            get { return (bool)GetValue(HistoricalLigaturesProperty); }
            set { SetValue(HistoricalLigaturesProperty, value); }
        }

        public static readonly DependencyProperty ContextualAlternatesProperty = RegisterTypographicProperty(Typography.ContextualAlternatesProperty);
        public bool ContextualAlternates
        {
            get { return (bool)GetValue(ContextualAlternatesProperty); }
            set { SetValue(ContextualAlternatesProperty, value); }
        }

        public static readonly DependencyProperty HistoricalFormsProperty = RegisterTypographicProperty(Typography.HistoricalFormsProperty);
        public bool HistoricalForms
        {
            get { return (bool)GetValue(HistoricalFormsProperty); }
            set { SetValue(HistoricalFormsProperty, value); }
        }

        public static readonly DependencyProperty KerningProperty = RegisterTypographicProperty(Typography.KerningProperty);
        public bool Kerning
        {
            get { return (bool)GetValue(KerningProperty); }
            set { SetValue(KerningProperty, value); }
        }

        public static readonly DependencyProperty CapitalSpacingProperty = RegisterTypographicProperty(Typography.CapitalSpacingProperty);
        public bool CapitalSpacing
        {
            get { return (bool)GetValue(CapitalSpacingProperty); }
            set { SetValue(CapitalSpacingProperty, value); }
        }

        public static readonly DependencyProperty CaseSensitiveFormsProperty = RegisterTypographicProperty(Typography.CaseSensitiveFormsProperty);
        public bool CaseSensitiveForms
        {
            get { return (bool)GetValue(CaseSensitiveFormsProperty); }
            set { SetValue(CaseSensitiveFormsProperty, value); }
        }

        public static readonly DependencyProperty StylisticSet1Property = RegisterTypographicProperty(Typography.StylisticSet1Property);
        public bool StylisticSet1
        {
            get { return (bool)GetValue(StylisticSet1Property); }
            set { SetValue(StylisticSet1Property, value); }
        }

        public static readonly DependencyProperty StylisticSet2Property = RegisterTypographicProperty(Typography.StylisticSet2Property);
        public bool StylisticSet2
        {
            get { return (bool)GetValue(StylisticSet2Property); }
            set { SetValue(StylisticSet2Property, value); }
        }

        public static readonly DependencyProperty StylisticSet3Property = RegisterTypographicProperty(Typography.StylisticSet3Property);
        public bool StylisticSet3
        {
            get { return (bool)GetValue(StylisticSet3Property); }
            set { SetValue(StylisticSet3Property, value); }
        }

        public static readonly DependencyProperty StylisticSet4Property = RegisterTypographicProperty(Typography.StylisticSet4Property);
        public bool StylisticSet4
        {
            get { return (bool)GetValue(StylisticSet4Property); }
            set { SetValue(StylisticSet4Property, value); }
        }

        public static readonly DependencyProperty StylisticSet5Property = RegisterTypographicProperty(Typography.StylisticSet5Property);
        public bool StylisticSet5
        {
            get { return (bool)GetValue(StylisticSet5Property); }
            set { SetValue(StylisticSet5Property, value); }
        }

        public static readonly DependencyProperty StylisticSet6Property = RegisterTypographicProperty(Typography.StylisticSet6Property);
        public bool StylisticSet6
        {
            get { return (bool)GetValue(StylisticSet6Property); }
            set { SetValue(StylisticSet6Property, value); }
        }

        public static readonly DependencyProperty StylisticSet7Property = RegisterTypographicProperty(Typography.StylisticSet7Property);
        public bool StylisticSet7
        {
            get { return (bool)GetValue(StylisticSet7Property); }
            set { SetValue(StylisticSet7Property, value); }
        }

        public static readonly DependencyProperty StylisticSet8Property = RegisterTypographicProperty(Typography.StylisticSet8Property);
        public bool StylisticSet8
        {
            get { return (bool)GetValue(StylisticSet8Property); }
            set { SetValue(StylisticSet8Property, value); }
        }

        public static readonly DependencyProperty StylisticSet9Property = RegisterTypographicProperty(Typography.StylisticSet9Property);
        public bool StylisticSet9
        {
            get { return (bool)GetValue(StylisticSet9Property); }
            set { SetValue(StylisticSet9Property, value); }
        }

        public static readonly DependencyProperty StylisticSet10Property = RegisterTypographicProperty(Typography.StylisticSet10Property);
        public bool StylisticSet10
        {
            get { return (bool)GetValue(StylisticSet10Property); }
            set { SetValue(StylisticSet10Property, value); }
        }

        public static readonly DependencyProperty StylisticSet11Property = RegisterTypographicProperty(Typography.StylisticSet11Property);
        public bool StylisticSet11
        {
            get { return (bool)GetValue(StylisticSet11Property); }
            set { SetValue(StylisticSet11Property, value); }
        }

        public static readonly DependencyProperty StylisticSet12Property = RegisterTypographicProperty(Typography.StylisticSet12Property);
        public bool StylisticSet12
        {
            get { return (bool)GetValue(StylisticSet12Property); }
            set { SetValue(StylisticSet12Property, value); }
        }

        public static readonly DependencyProperty StylisticSet13Property = RegisterTypographicProperty(Typography.StylisticSet13Property);
        public bool StylisticSet13
        {
            get { return (bool)GetValue(StylisticSet13Property); }
            set { SetValue(StylisticSet13Property, value); }
        }

        public static readonly DependencyProperty StylisticSet14Property = RegisterTypographicProperty(Typography.StylisticSet14Property);
        public bool StylisticSet14
        {
            get { return (bool)GetValue(StylisticSet14Property); }
            set { SetValue(StylisticSet14Property, value); }
        }

        public static readonly DependencyProperty StylisticSet15Property = RegisterTypographicProperty(Typography.StylisticSet15Property);
        public bool StylisticSet15
        {
            get { return (bool)GetValue(StylisticSet15Property); }
            set { SetValue(StylisticSet15Property, value); }
        }

        public static readonly DependencyProperty StylisticSet16Property = RegisterTypographicProperty(Typography.StylisticSet16Property);
        public bool StylisticSet16
        {
            get { return (bool)GetValue(StylisticSet16Property); }
            set { SetValue(StylisticSet16Property, value); }
        }

        public static readonly DependencyProperty StylisticSet17Property = RegisterTypographicProperty(Typography.StylisticSet17Property);
        public bool StylisticSet17
        {
            get { return (bool)GetValue(StylisticSet17Property); }
            set { SetValue(StylisticSet17Property, value); }
        }

        public static readonly DependencyProperty StylisticSet18Property = RegisterTypographicProperty(Typography.StylisticSet18Property);
        public bool StylisticSet18
        {
            get { return (bool)GetValue(StylisticSet18Property); }
            set { SetValue(StylisticSet18Property, value); }
        }

        public static readonly DependencyProperty StylisticSet19Property = RegisterTypographicProperty(Typography.StylisticSet19Property);
        public bool StylisticSet19
        {
            get { return (bool)GetValue(StylisticSet19Property); }
            set { SetValue(StylisticSet19Property, value); }
        }

        public static readonly DependencyProperty StylisticSet20Property = RegisterTypographicProperty(Typography.StylisticSet20Property);
        public bool StylisticSet20
        {
            get { return (bool)GetValue(StylisticSet20Property); }
            set { SetValue(StylisticSet20Property, value); }
        }

        public static readonly DependencyProperty SlashedZeroProperty = RegisterTypographicProperty(Typography.SlashedZeroProperty, "Digits");
        public bool SlashedZero
        {
            get { return (bool)GetValue(SlashedZeroProperty); }
            set { SetValue(SlashedZeroProperty, value); }
        }

        public static readonly DependencyProperty MathematicalGreekProperty = RegisterTypographicProperty(Typography.MathematicalGreekProperty);
        public bool MathematicalGreek
        {
            get { return (bool)GetValue(MathematicalGreekProperty); }
            set { SetValue(MathematicalGreekProperty, value); }
        }

        public static readonly DependencyProperty EastAsianExpertFormsProperty = RegisterTypographicProperty(Typography.EastAsianExpertFormsProperty);
        public bool EastAsianExpertForms
        {
            get { return (bool)GetValue(EastAsianExpertFormsProperty); }
            set { SetValue(EastAsianExpertFormsProperty, value); }
        }

        public static readonly DependencyProperty FractionProperty = RegisterTypographicProperty(Typography.FractionProperty, "OneHalf");
        public FontFraction Fraction
        {
            get { return (FontFraction)GetValue(FractionProperty); }
            set { SetValue(FractionProperty, value); }
        }

        public static readonly DependencyProperty VariantsProperty = RegisterTypographicProperty(Typography.VariantsProperty);
        public FontVariants Variants
        {
            get { return (FontVariants)GetValue(VariantsProperty); }
            set { SetValue(VariantsProperty, value); }
        }

        public static readonly DependencyProperty CapitalsProperty = RegisterTypographicProperty(Typography.CapitalsProperty);
        public FontCapitals Capitals
        {
            get { return (FontCapitals)GetValue(CapitalsProperty); }
            set { SetValue(CapitalsProperty, value); }
        }

        public static readonly DependencyProperty NumeralStyleProperty = RegisterTypographicProperty(Typography.NumeralStyleProperty, "Digits");
        public FontNumeralStyle NumeralStyle
        {
            get { return (FontNumeralStyle)GetValue(NumeralStyleProperty); }
            set { SetValue(NumeralStyleProperty, value); }
        }

        public static readonly DependencyProperty NumeralAlignmentProperty = RegisterTypographicProperty(Typography.NumeralAlignmentProperty, "Digits");
        public FontNumeralAlignment NumeralAlignment
        {
            get { return (FontNumeralAlignment)GetValue(NumeralAlignmentProperty); }
            set { SetValue(NumeralAlignmentProperty, value); }
        }

        public static readonly DependencyProperty EastAsianWidthsProperty = RegisterTypographicProperty(Typography.EastAsianWidthsProperty);
        public FontEastAsianWidths EastAsianWidths
        {
            get { return (FontEastAsianWidths)GetValue(EastAsianWidthsProperty); }
            set { SetValue(EastAsianWidthsProperty, value); }
        }

        public static readonly DependencyProperty EastAsianLanguageProperty = RegisterTypographicProperty(Typography.EastAsianLanguageProperty);
        public FontEastAsianLanguage EastAsianLanguage
        {
            get { return (FontEastAsianLanguage)GetValue(EastAsianLanguageProperty); }
            set { SetValue(EastAsianLanguageProperty, value); }
        }

        public static readonly DependencyProperty AnnotationAlternatesProperty = RegisterTypographicProperty(Typography.AnnotationAlternatesProperty);
        public int AnnotationAlternates
        {
            get { return (int)GetValue(AnnotationAlternatesProperty); }
            set { SetValue(AnnotationAlternatesProperty, value); }
        }

        public static readonly DependencyProperty StandardSwashesProperty = RegisterTypographicProperty(Typography.StandardSwashesProperty);
        public int StandardSwashes
        {
            get { return (int)GetValue(StandardSwashesProperty); }
            set { SetValue(StandardSwashesProperty, value); }
        }

        public static readonly DependencyProperty ContextualSwashesProperty = RegisterTypographicProperty(Typography.ContextualSwashesProperty);
        public int ContextualSwashes
        {
            get { return (int)GetValue(ContextualSwashesProperty); }
            set { SetValue(ContextualSwashesProperty, value); }
        }

        public static readonly DependencyProperty StylisticAlternatesProperty = RegisterTypographicProperty(Typography.StylisticAlternatesProperty);
        public int StylisticAlternates
        {
            get { return (int)GetValue(StylisticAlternatesProperty); }
            set { SetValue(StylisticAlternatesProperty, value); }
        }

        static void TypographicPropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var chooser = obj as FontChooser;
            if (chooser != null)
            {
                chooser.InvalidatePreview();
            }
        }

        #endregion

        #region Other dependency properties

        public static readonly DependencyProperty SelectedFontFamilyProperty = RegisterFontProperty(
            "SelectedFontFamily",
            TextBlock.FontFamilyProperty, 
            new PropertyChangedCallback(SelectedFontFamilyChangedCallback)
            );
        public FontFamily SelectedFontFamily
        {
            get { return GetValue(SelectedFontFamilyProperty) as FontFamily; }
            set { SetValue(SelectedFontFamilyProperty, value); }
        }
        static void SelectedFontFamilyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((FontChooser)obj).OnSelectedFontFamilyChanged(e.NewValue as FontFamily);
        }

        public static readonly DependencyProperty SelectedFontWeightProperty = RegisterFontProperty(
            "SelectedFontWeight",
            TextBlock.FontWeightProperty,
            new PropertyChangedCallback(SelectedTypefaceChangedCallback)
            );
        public FontWeight SelectedFontWeight
        {
            get { return (FontWeight)GetValue(SelectedFontWeightProperty); }
            set { SetValue(SelectedFontWeightProperty, value); }
        }

        public static readonly DependencyProperty SelectedFontStyleProperty = RegisterFontProperty(
            "SelectedFontStyle",
            TextBlock.FontStyleProperty,
            new PropertyChangedCallback(SelectedTypefaceChangedCallback)
            );
        public FontStyle SelectedFontStyle
        {
            get { return (FontStyle)GetValue(SelectedFontStyleProperty); }
            set { SetValue(SelectedFontStyleProperty, value); }
        }

        public static readonly DependencyProperty SelectedFontStretchProperty = RegisterFontProperty(
           "SelectedFontStretch",
           TextBlock.FontStretchProperty,
           new PropertyChangedCallback(SelectedTypefaceChangedCallback)
           );
        public FontStretch SelectedFontStretch
        {
            get { return (FontStretch)GetValue(SelectedFontStretchProperty); }
            set { SetValue(SelectedFontStretchProperty, value); }
        }

        static void SelectedTypefaceChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((FontChooser)obj).InvalidateTypefaceListSelection();
        }

        public static readonly DependencyProperty SelectedFontSizeProperty = RegisterFontProperty(
           "SelectedFontSize",
           TextBlock.FontSizeProperty,
           new PropertyChangedCallback(SelectedFontSizeChangedCallback)
           );
        public double SelectedFontSize
        {
            get { return (double)GetValue(SelectedFontSizeProperty); }
            set { SetValue(SelectedFontSizeProperty, value); }
        }
        static void SelectedFontSizeChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((FontChooser)obj).OnSelectedFontSizeChanged((double)(e.NewValue));
        }

		public double SelectedFontPointSize
		{
			get { return FontSizeListItem.PixelsToPoints (SelectedFontSize); }
			set { SelectedFontSize = FontSizeListItem.PointsToPixels (value); }
		}

        public static readonly DependencyProperty SelectedTextDecorationsProperty = RegisterFontProperty(
           "SelectedTextDecorations",
           TextBlock.TextDecorationsProperty,
           new PropertyChangedCallback(SelectedTextDecorationsChangedCallback)
           );
        public TextDecorationCollection SelectedTextDecorations
        {
            get { return GetValue(SelectedTextDecorationsProperty) as TextDecorationCollection; }
            set { SetValue(SelectedTextDecorationsProperty, value); }
        }
        static void SelectedTextDecorationsChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var chooser = (FontChooser)obj;
            chooser.OnTextDecorationsChanged();
        }

        #endregion

        #region Dependency property helper functions

        // Helper function for registering typographic dependency properties with property-specific sample text.
        static DependencyProperty RegisterTypographicProperty(DependencyProperty targetProperty, string sampleTextTag)
        {
            Type t = targetProperty.PropertyType;

            TypographyFeaturePage featurePage = (t == typeof(bool)) ? TypographyFeaturePage.BooleanFeaturePage :
                                                (t == typeof(int)) ? TypographyFeaturePage.IntegerFeaturePage :
                                                new TypographyFeaturePage(t);

            return DependencyProperty.Register(
                targetProperty.Name,
                t,
                typeof(FontChooser),
                new TypographicPropertyMetadata(
                    targetProperty.DefaultMetadata.DefaultValue,
                    targetProperty,
                    featurePage,
                    sampleTextTag
                    )
                );
        }

        // Helper function for registering typographic dependency properties with default sample text for the type.
        static DependencyProperty RegisterTypographicProperty(DependencyProperty targetProperty)
        {
            return RegisterTypographicProperty(targetProperty, null);
        }

        // Helper function for registering font chooser dependency properties other than typographic properties.
        static DependencyProperty RegisterFontProperty(
            string propertyName,
            DependencyProperty targetProperty,
            PropertyChangedCallback changeCallback
            )
        {
            return DependencyProperty.Register(
                propertyName,
                targetProperty.PropertyType,
                typeof(FontChooser),
                new FontPropertyMetadata(
                    targetProperty.DefaultMetadata.DefaultValue,
                    changeCallback,
                    targetProperty
                    )
                );
        }

        #endregion

        #region Dependency property tables

        // Array of all font chooser dependency properties
        static readonly DependencyProperty[] _chooserProperties = {
            // typography properties
            StandardLigaturesProperty,
            ContextualLigaturesProperty,
            DiscretionaryLigaturesProperty,
            HistoricalLigaturesProperty,
            ContextualAlternatesProperty,
            HistoricalFormsProperty,
            KerningProperty,
            CapitalSpacingProperty,
            CaseSensitiveFormsProperty,
            StylisticSet1Property,
            StylisticSet2Property,
            StylisticSet3Property,
            StylisticSet4Property,
            StylisticSet5Property,
            StylisticSet6Property,
            StylisticSet7Property,
            StylisticSet8Property,
            StylisticSet9Property,
            StylisticSet10Property,
            StylisticSet11Property,
            StylisticSet12Property,
            StylisticSet13Property,
            StylisticSet14Property,
            StylisticSet15Property,
            StylisticSet16Property,
            StylisticSet17Property,
            StylisticSet18Property,
            StylisticSet19Property,
            StylisticSet20Property,
            SlashedZeroProperty,
            MathematicalGreekProperty,
            EastAsianExpertFormsProperty,
            FractionProperty,
            VariantsProperty,
            CapitalsProperty,
            NumeralStyleProperty,
            NumeralAlignmentProperty,
            EastAsianWidthsProperty,
            EastAsianLanguageProperty,
            AnnotationAlternatesProperty,
            StandardSwashesProperty,
            ContextualSwashesProperty,
            StylisticAlternatesProperty,

            // other properties
            SelectedFontFamilyProperty,
            SelectedFontWeightProperty,
            SelectedFontStyleProperty,
            SelectedFontStretchProperty,
            SelectedFontSizeProperty,
            SelectedTextDecorationsProperty
        };

        #endregion

        #region Property change handlers

        // Handle changes to the SelectedFontFamily property
        void OnSelectedFontFamilyChanged(FontFamily family)
        {
            // If the family list is not valid do nothing for now. 
            // We'll be called again after the list is initialized.
            if (_familyListValid)
            {
                // Select the family in the list; this will return null if the family is not in the list.
                FontFamilyListItem item = SelectFontFamilyListItem(family);

                // Set the text box to the family name, if it isn't already.
                string displayName = (item != null) ? item.ToString() : FontFamilyListItem.GetDisplayName(family);
                if (string.Compare(fontFamilyTextBox.Text, displayName, true, CultureInfo.CurrentCulture) != 0)
                {
                    fontFamilyTextBox.Text = displayName;
                }

                // The typeface list is no longer valid; update it in the background to improve responsiveness.
                InvalidateTypefaceList();
            }
        }

        // Handle changes to the SelectedFontSize property
        void OnSelectedFontSizeChanged(double sizeInPixels)
        {
            // Select the list item, if the size is in the list.
            double sizeInPoints = FontSizeListItem.PixelsToPoints(sizeInPixels);
            if (!SelectListItem(sizeList, sizeInPoints))
            {
                sizeList.SelectedIndex = -1;
            }

            // Set the text box contents if it doesn't already match the current size.
            double textBoxValue;
            if (!double.TryParse(sizeTextBox.Text, out textBoxValue) || !FontSizeListItem.FuzzyEqual(textBoxValue, sizeInPoints))
            {
                sizeTextBox.Text = sizeInPoints.ToString();
            }

            // Schedule background updates.
            InvalidateTab(typographyTab);
            InvalidatePreview();
        }

        // Handle changes to any of the text decoration properties.
        void OnTextDecorationsChanged()
        {
            bool underline = false;
            bool baseline = false;
            bool strikethrough = false;
            bool overline = false;

            TextDecorationCollection textDecorations = SelectedTextDecorations;
            if (textDecorations != null)
            {
                foreach (TextDecoration td in textDecorations)
                {
                    switch (td.Location)
                    {
                        case TextDecorationLocation.Underline:
                            underline = true;
                            break;
                        case TextDecorationLocation.Baseline:
                            baseline = true;
                            break;
                        case TextDecorationLocation.Strikethrough:
                            strikethrough = true;
                            break;
                        case TextDecorationLocation.OverLine:
                            overline = true;
                            break;
                    }
                }
            }

            underlineCheckBox.IsChecked = underline;
            baselineCheckBox.IsChecked = baseline;
            strikethroughCheckBox.IsChecked = strikethrough;
            overlineCheckBox.IsChecked = overline;

            // Schedule background updates.
            InvalidateTab(typographyTab);
            InvalidatePreview();
        }

        #endregion

        #region Background update logic

        // Schedule background initialization of the font famiy list.
        void InvalidateFontFamilyList()
        {
            if (_familyListValid)
            {
                InvalidateTypefaceList();

                fontFamilyList.Items.Clear();
                fontFamilyTextBox.Clear();
                _familyListValid = false;

                ScheduleUpdate();
            }
        }

        // Schedule background initialization of the typeface list.
        void InvalidateTypefaceList()
        {
            if (_typefaceListValid)
            {
                typefaceList.Items.Clear();
                _typefaceListValid = false;

                ScheduleUpdate();
            }
        }

        // Schedule background selection of the current typeface list item.
        void InvalidateTypefaceListSelection()
        {
            if (_typefaceListSelectionValid)
            {
                _typefaceListSelectionValid = false;
                ScheduleUpdate();
            }
        }

        // Mark a specific tab as invalid and schedule background initialization if necessary.
        void InvalidateTab(TabItem tab)
        {
            TabState tabState;
			if (_tabDictionary != null && _tabDictionary.TryGetValue (tab, out tabState))
            {
                if (tabState.IsValid)
                {
                    tabState.IsValid = false;

                    if (tabControl.SelectedItem == tab)
                    {
                        ScheduleUpdate();
                    }
                }
            }
        }

        // Mark all the tabs as invalid and schedule background initialization of the current tab.
        void InvalidateTabs()
        {
            foreach (KeyValuePair<TabItem, TabState> item in _tabDictionary)
            {
                item.Value.IsValid = false;
            }

            ScheduleUpdate();
        }

        // Schedule background initialization of the preview control.
        void InvalidatePreview()
        {
            if (_previewValid)
            {
                _previewValid = false;
                ScheduleUpdate();
            }
        }

        // Schedule background initialization.
        void ScheduleUpdate()
        {
            if (_populated && !_updatePending)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new UpdateCallback(OnUpdate));
                _updatePending = true;
            }
        }

        // Dispatcher callback that performs background initialization.
        void OnUpdate()
        {
			if (!_populated)
				return;

            _updatePending = false;

            if (!_familyListValid)
            {
                // Initialize the font family list.
                InitializeFontFamilyList();
                _familyListValid = true;
                OnSelectedFontFamilyChanged(SelectedFontFamily);

                // Defer any other initialization until later.
                ScheduleUpdate();
            }
            else if (!_typefaceListValid)
            {
                // Initialize the typeface list.
                InitializeTypefaceList();
                _typefaceListValid = true;

                // Select the current typeface in the list.
                InitializeTypefaceListSelection();
                _typefaceListSelectionValid = true;

                // Defer any other initialization until later.
                ScheduleUpdate();
            }
            else if (!_typefaceListSelectionValid)
            {
                // Select the current typeface in the list.
                InitializeTypefaceListSelection();
                _typefaceListSelectionValid = true;

                // Defer any other initialization until later.
                ScheduleUpdate();
            }
            else
            {
                // Perform any remaining initialization.
                TabState tab = CurrentTabState;
                if (tab != null && !tab.IsValid)
                {
                    // Initialize the current tab.
                    tab.InitializeTab();
                    tab.IsValid = true;
                }
                if (!_previewValid)
                {
                    // Initialize the preview control.
                    InitializePreview();
                    _previewValid = true;
                }
            }
        }

        #endregion

        #region Content initialization

        void InitializeFontFamilyList()
        {
            ICollection<FontFamily> familyCollection = FontFamilyCollection;
            if (familyCollection != null)
            {
                var items = new FontFamilyListItem[familyCollection.Count];

                int i = 0;

                foreach (FontFamily family in familyCollection)
                {
                    items[i++] = new FontFamilyListItem(family);
                }

                Array.Sort<FontFamilyListItem>(items);

                foreach (FontFamilyListItem item in items)
                {
                    fontFamilyList.Items.Add(item);
                }
            }
        }

        void InitializeTypefaceList()
        {
            FontFamily family = SelectedFontFamily;
            if (family != null)
            {
                ICollection<Typeface> faceCollection = family.GetTypefaces();

				var items = new List<TypefaceListItem>(faceCollection.Count);

                foreach (Typeface face in faceCollection)
                {
					if (!Drawing.FontHandler.ShowSimulatedFonts && (face.IsBoldSimulated || face.IsObliqueSimulated))
						continue;
                    items.Add(new TypefaceListItem(face));
                }

				items.Sort();

                foreach (TypefaceListItem item in items)
                {
                    typefaceList.Items.Add(item);
                }
            }
        }

        void InitializeTypefaceListSelection()
        {
            // If the typeface list is not valid, do nothing for now.
            // We'll be called again after the list is initialized.
            if (_typefaceListValid)
            {
                var typeface = new Typeface(SelectedFontFamily, SelectedFontStyle, SelectedFontWeight, SelectedFontStretch);

                // Select the typeface in the list.
                SelectTypefaceListItem(typeface);

                // Schedule background updates.
                InvalidateTabs();
                InvalidatePreview();
            }
        }

        void InitializeFeatureList()
        {
            var items = new TypographicFeatureListItem[_chooserProperties.Length];

            int count = 0;

            foreach (DependencyProperty property in _chooserProperties)
            {
                if (property.GetMetadata(typeof(FontChooser)) is TypographicPropertyMetadata)
                {
                    string displayName = LookupString(property.Name);
                    items[count++] = new TypographicFeatureListItem(displayName, property);
                }
            }

            Array.Sort(items, 0, count);

            for (int i = 0; i < count; ++i)
            {
                featureList.Items.Add(items[i]);
            }
        }

        static string LookupString(string tag)
        {
			return Properties.FontDialogResources.ResourceManager.GetString (tag, CultureInfo.CurrentUICulture);
        }

        TabState CurrentTabState
        {
            get
            {
				if (tabControl.SelectedItem == null) return null;
				TabState tab;
                return _tabDictionary.TryGetValue(tabControl.SelectedItem as TabItem, out tab) ? tab : null;
            }
        }

        void InitializeSamplesTab()
        {
            FontFamily selectedFamily = SelectedFontFamily;

            var selectedFace = new Typeface(
                selectedFamily,
                SelectedFontStyle,
                SelectedFontWeight,
                SelectedFontStretch
                );

            fontFamilyNameRun.Text = FontFamilyListItem.GetDisplayName(selectedFamily);
            typefaceNameRun.Text = TypefaceListItem.GetDisplayName(selectedFace);

            // Create FontFamily samples document.
            var doc = new FlowDocument();
            foreach (Typeface face in selectedFamily.GetTypefaces())
            {
                var labelPara = new Paragraph(new Run(TypefaceListItem.GetDisplayName(face)));
                labelPara.Margin = new Thickness(0);
                doc.Blocks.Add(labelPara);

                var samplePara = new Paragraph(new Run(_previewSampleText));
                samplePara.FontFamily = selectedFamily;
                samplePara.FontWeight = face.Weight;
                samplePara.FontStyle = face.Style;
                samplePara.FontStretch = face.Stretch;
                samplePara.FontSize = 16.0;
                samplePara.Margin = new Thickness(0, 0, 0, 8);
                doc.Blocks.Add(samplePara);
            }

            fontFamilySamples.Document = doc;

            // Create typeface samples document.
            doc = new FlowDocument();
            foreach (double sizeInPoints in new double[] { 9.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 17.0 })
            {
                string labelText = string.Format("{0} {1}", sizeInPoints, _pointsText);
                var labelPara = new Paragraph(new Run(labelText));
                labelPara.Margin = new Thickness(0);
                doc.Blocks.Add(labelPara);

                var samplePara = new Paragraph(new Run(_previewSampleText));
                samplePara.FontFamily = selectedFamily;
                samplePara.FontWeight = selectedFace.Weight;
                samplePara.FontStyle = selectedFace.Style;
                samplePara.FontStretch = selectedFace.Stretch;
                samplePara.FontSize = FontSizeListItem.PointsToPixels(sizeInPoints);
                samplePara.Margin = new Thickness(0, 0, 0, 8);
                doc.Blocks.Add(samplePara);
            }

            typefaceSamples.Document = doc;
        }

        void InitializeTypographyTab()
        {
            if (featureList.Items.IsEmpty)
            {
                InitializeFeatureList();
                featureList.SelectedIndex = 0;

                featureList.SelectionChanged += new SelectionChangedEventHandler(featureList_SelectionChanged);
            }

            DependencyProperty chooserProperty = null;
            TypographyFeaturePage featurePage = null;

            var listItem = featureList.SelectedItem as TypographicFeatureListItem;
            if (listItem != null)
            {
                var metadata = listItem.ChooserProperty.GetMetadata(typeof(FontChooser)) as TypographicPropertyMetadata;
                if (metadata != null)
                {
                    chooserProperty = listItem.ChooserProperty;
                    featurePage = metadata.FeaturePage;
                }
            }

            InitializeFeaturePage(typographyFeaturePage, chooserProperty, featurePage);
        }

        void InitializeFeaturePage(Grid grid, DependencyProperty chooserProperty, TypographyFeaturePage page)
        {
            if (page == null)
            {
                grid.Children.Clear();
                grid.RowDefinitions.Clear();
            }
            else
            {
                // Get the property value and metadata.
                object value = GetValue(chooserProperty);
                var metadata = (TypographicPropertyMetadata)chooserProperty.GetMetadata(typeof(FontChooser));

                // Look up the sample text.
                string sampleText = (metadata.SampleTextTag != null) ? LookupString(metadata.SampleTextTag) :
                                    _defaultSampleText;

                if (page == _currentFeaturePage)
                {
                    // Update the state of the controls.
                    for (int i = 0; i < page.Items.Length; ++i)
                    {
                        // Check the radio button if it matches the current property value.
                        if (page.Items[i].Value.Equals(value))
                        {
                            var radioButton = (RadioButton)grid.Children[i * 2];
                            radioButton.IsChecked = true;
                        }

                        // Apply properties to the sample text block.
                        var sample = (TextBlock)grid.Children[i * 2 + 1];
                        sample.Text = sampleText;
                        ApplyPropertiesToObjectExcept(sample, chooserProperty);
                        sample.SetValue(metadata.TargetProperty, page.Items[i].Value);
                    }
                }
                else
                {
                    grid.Children.Clear();
                    grid.RowDefinitions.Clear();

                    // Add row definitions.
                    for (int i = 0; i < page.Items.Length; ++i)
                    {
                        var row = new RowDefinition();
                        row.Height = GridLength.Auto;
                        grid.RowDefinitions.Add(row);
                    }

                    // Add the controls.
                    for (int i = 0; i < page.Items.Length; ++i)
                    {
                        string tag = page.Items[i].Tag;
                        var radioContent = new TextBlock(new Run(LookupString(tag)));
                        radioContent.TextWrapping = TextWrapping.Wrap;

                        // Add the radio button.
                        var radioButton = new RadioButton();
                        radioButton.Name = tag;
                        radioButton.Content = radioContent;
                        radioButton.Margin = new Thickness(5.0, 0.0, 0.0, 0.0);
                        radioButton.VerticalAlignment = VerticalAlignment.Center;
                        Grid.SetRow(radioButton, i);
                        grid.Children.Add(radioButton);

                        // Check the radio button if it matches the current property value.
                        if (page.Items[i].Value.Equals(value))
                        {
                            radioButton.IsChecked = true;
                        }

                        // Hook up the event.
						radioButton.Checked += featureRadioButton_Checked;

                        // Add the block with sample text.
                        var sample = new TextBlock(new Run(sampleText));
                        sample.Margin = new Thickness(5.0, 5.0, 5.0, 0.0);
                        sample.TextWrapping = TextWrapping.WrapWithOverflow;
                        ApplyPropertiesToObjectExcept(sample, chooserProperty);
                        sample.SetValue(metadata.TargetProperty, page.Items[i].Value);
                        Grid.SetRow(sample, i);
                        Grid.SetColumn(sample, 1);
                        grid.Children.Add(sample);
                    }

                    // Add borders between rows.
                    for (int i = 0; i < page.Items.Length; ++i)
                    {
                        var border = new Border();
                        border.BorderThickness = new Thickness(0.0, 0.0, 0.0, 1.0);
                        border.BorderBrush = SystemColors.ControlLightBrush;
                        Grid.SetRow(border, i);
                        Grid.SetColumnSpan(border, 2);
                        grid.Children.Add(border);
                    }
                }
            }

            _currentFeature = chooserProperty;
            _currentFeaturePage = page;
        }

        void featureRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (_currentFeature != null && _currentFeaturePage != null)
            {
                string tag = ((RadioButton)sender).Name;

                foreach (TypographyFeaturePage.Item item in _currentFeaturePage.Items)
                {
                    if (item.Tag == tag)
                    {
                        SetValue(_currentFeature, item.Value);
                    }
                }
            }
        }

        static void AddTableRow(TableRowGroup rowGroup, string leftText, string rightText)
        {
            var row = new TableRow();

            row.Cells.Add(new TableCell(new Paragraph(new Run(leftText))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(rightText))));

            rowGroup.Rows.Add(row);
        }

        void AddTableRow(TableRowGroup rowGroup, string leftText, IDictionary<CultureInfo, string> rightStrings)
        {
            string rightText = NameDictionaryExtensions.GetDisplayName(rightStrings);
            AddTableRow(rowGroup, leftText, rightText);
        }

        void InitializeDescriptiveTextTab()
        {
            var selectedTypeface = new Typeface(
                SelectedFontFamily,
                SelectedFontStyle,
                SelectedFontWeight,
                SelectedFontStretch
                );

            GlyphTypeface glyphTypeface;
            if (selectedTypeface.TryGetGlyphTypeface(out glyphTypeface))
            {
                // Create a table with two columns.
                var table = new Table();
                table.CellSpacing = 5;
                var leftColumn = new TableColumn();
                leftColumn.Width = new GridLength(2.0, GridUnitType.Star);
                table.Columns.Add(leftColumn);
                var rightColumn = new TableColumn();
                rightColumn.Width = new GridLength(3.0, GridUnitType.Star);
                table.Columns.Add(rightColumn);

                var rowGroup = new TableRowGroup();
                AddTableRow(rowGroup, "Family:", glyphTypeface.FamilyNames);
                AddTableRow(rowGroup, "Face:", glyphTypeface.FaceNames);
                AddTableRow(rowGroup, "Description:", glyphTypeface.Descriptions);
                AddTableRow(rowGroup, "Version:", glyphTypeface.VersionStrings);
                AddTableRow(rowGroup, "Copyright:", glyphTypeface.Copyrights);
                AddTableRow(rowGroup, "Trademark:", glyphTypeface.Trademarks);
                AddTableRow(rowGroup, "Manufacturer:", glyphTypeface.ManufacturerNames);
                AddTableRow(rowGroup, "Designer:", glyphTypeface.DesignerNames);
                AddTableRow(rowGroup, "Designer URL:", glyphTypeface.DesignerUrls);
                AddTableRow(rowGroup, "Vendor URL:", glyphTypeface.VendorUrls);
                AddTableRow(rowGroup, "Win32 Family:", glyphTypeface.Win32FamilyNames);
                AddTableRow(rowGroup, "Win32 Face:", glyphTypeface.Win32FaceNames);

                try
                {
                    AddTableRow(rowGroup, "Font File URI:", glyphTypeface.FontUri.ToString());
                }
                catch (System.Security.SecurityException)
                {
                    // Font file URI is privileged information; just skip it if we don't have access.
                }

                table.RowGroups.Add(rowGroup);

                fontDescriptionBox.Document = new FlowDocument(table);

                fontLicenseBox.Text = NameDictionaryExtensions.GetDisplayName(glyphTypeface.LicenseDescriptions);
            }
            else
            {
                fontDescriptionBox.Document = new FlowDocument();
                fontLicenseBox.Text = String.Empty;
            }
        }

        void InitializePreview()
        {
            ApplyPropertiesToObject(previewTextBox);
        }

        #endregion

        #region List box helpers

        // Update font family list based on selection.
        // Return list item if there's an exact match, or null if not.
        FontFamilyListItem SelectFontFamilyListItem(string displayName)
		{
			var listItem = fontFamilyList.SelectedItem as FontFamilyListItem;
			if (listItem != null && string.Compare(listItem.ToString(), displayName, true, CultureInfo.CurrentCulture) == 0)
			{
				// Already selected
				return listItem;
			}
			if (SelectListItem(fontFamilyList, displayName))
			{
				// Exact match found
				return fontFamilyList.SelectedItem as FontFamilyListItem;
			}
			// Not in the list
			return null;
		}

        // Update font family list based on selection.
        // Return list item if there's an exact match, or null if not.
        FontFamilyListItem SelectFontFamilyListItem(FontFamily family)
		{
			var listItem = fontFamilyList.SelectedItem as FontFamilyListItem;
			if (listItem != null && listItem.FontFamily.Equals(family))
			{
				// Already selected
				return listItem;
			}
			if (SelectListItem(fontFamilyList, FontFamilyListItem.GetDisplayName(family)))
			{
				// Exact match found
				return fontFamilyList.SelectedItem as FontFamilyListItem;
			}
			// Not in the list
			return null;
		}

        // Update typeface list based on selection.
        // Return list item if there's an exact match, or null if not.
        TypefaceListItem SelectTypefaceListItem(Typeface typeface)
		{
			var listItem = typefaceList.SelectedItem as TypefaceListItem;
			if (listItem != null && listItem.Typeface.Equals(typeface))
			{
				// Already selected
				return listItem;
			}
			if (SelectListItem(typefaceList, new TypefaceListItem(typeface)))
			{
				// Exact match found
				return typefaceList.SelectedItem as TypefaceListItem;
			}
			// Not in list
			return null;
		}

        // Update list based on selection.
        // Return true if there's an exact match, or false if not.
        static bool SelectListItem(ListBox list, object value)
        {
            ItemCollection itemList = list.Items;

            // Perform a binary search for the item.
            int first = 0;
            int limit = itemList.Count;

            while (first < limit)
            {
                int i = first + (limit - first) / 2;
                var item = (IComparable)(itemList[i]);
                int comparison = item.CompareTo(value);
                if (comparison < 0)
                {
                    // Value must be after i
                    first = i + 1;
                }
                else if (comparison > 0)
                {
                    // Value must be before i
                    limit = i;
                }
                else
                {
                    // Exact match; select the item.
                    list.SelectedIndex = i;
                    itemList.MoveCurrentToPosition(i);
                    list.ScrollIntoView(itemList[i]);
                    return true;
                }
            }

            // Not an exact match; move current position to the nearest item but don't select it.
            if (itemList.Count > 0)
            {
                int i = Math.Min(first, itemList.Count - 1);
                itemList.MoveCurrentToPosition(i);
                list.ScrollIntoView(itemList[i]);
            }

            return false;
        }

        // Logic to handle UP and DOWN arrow keys in the text box associated with a list.
        // Behavior is similar to a Win32 combo box.
        void OnComboBoxPreviewKeyDown(TextBox textBox, ListBox listBox, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    // Move up from the current position.
                    MoveListPosition(listBox, -1);
                    e.Handled = true;
                    break;

                case Key.Down:
                    // Move down from the current position, unless the item at the current position is
                    // not already selected in which case select it.
                    if (listBox.Items.CurrentPosition == listBox.SelectedIndex)
                    {
                        MoveListPosition(listBox, +1);
                    }
                    else
                    {
                        MoveListPosition(listBox, 0);
                    }
                    e.Handled = true;
                    break;
            }
        }

        void MoveListPosition(ListBox listBox, int distance)
        {
            int i = listBox.Items.CurrentPosition + distance;
            if (i >= 0 && i < listBox.Items.Count)
            {
                listBox.Items.MoveCurrentToPosition(i);
                listBox.SelectedIndex = i;
                listBox.ScrollIntoView(listBox.Items[i]);
            }
        }

        #endregion
    }
}
