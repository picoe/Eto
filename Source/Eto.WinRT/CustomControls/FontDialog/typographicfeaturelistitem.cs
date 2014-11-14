#if TODO_XAML
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Globalization;

namespace Eto.WinRT.CustomControls.FontDialog
{
    class TypographicFeatureListItem : TextBlock, IComparable
    {
        readonly string _displayName;
        readonly DependencyProperty _chooserProperty;

        public TypographicFeatureListItem(string displayName, DependencyProperty chooserProperty)
        {
            _displayName = displayName;
            _chooserProperty = chooserProperty;
            Text = displayName;
        }

        public DependencyProperty ChooserProperty
        {
            get { return _chooserProperty; }
        }

        public override string ToString()
        {
            return _displayName;
        }

        int IComparable.CompareTo(object obj)
        {
            return string.Compare(_displayName, obj.ToString(), true, CultureInfo.CurrentCulture);
        }
    }
}
#endif