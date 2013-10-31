using Eto.Forms;
using System.Collections.Generic;
using Eto.Test.Sections.Controls;
using Eto.Test.Sections.Drawing;

namespace Eto.Test
{
	public static class TestSectionList
	{
		public static IEnumerable<Section> TopNodes()
		{
			yield return new Section("Forms", FormsSection());
			yield return new Section("Behaviors", BehaviorsSection());
			yield return new Section("Drawing", DrawingSection());
			yield return new Section("Controls", ControlSection());
			yield return new Section("Layouts", LayoutsSection());
			yield return new Section("Dialogs", DialogsSection());
			yield return new Section("Printing", PrintingSection());
			yield return new Section("Serialization", SerializationSection());
		}
		
		static IEnumerable<Section> ControlSection()
		{
			yield return new Section<LabelSection> { Text = "Label" };
			yield return new Section<ButtonSection> { Text = "Button" };
			yield return new Section<CheckBoxSection> { Text = "Check Box" };
			yield return new Section<RadioButtonSection> { Text = "Radio Button" };
			yield return new Section<ScrollableSection> { Text = "Scrollable" };
			yield return new Section<TextBoxSection> { Text = "Text Box" };
			yield return new Section<TextAreaSection> { Text = "Text Area" };
			yield return new Section<WebViewSection> { Text = "Web View" };
			yield return new Section<DrawableSection> { Text = "Drawable" };
			yield return new Section<ListBoxSection> { Text = "List Box" };
			yield return new Section<TabControlSection> { Text = "Tab Control" };
			yield return new Section<ThemedTabControlSection> { Text = "Tab Control (Themed)" };
			yield return new Section<TreeGridViewSection> { Text = "Tree Grid View" };
			yield return new Section<TreeViewSection> { Text = "Tree View" };
			yield return new Section<NumericUpDownSection> { Text = "Numeric Up/Down" };
			yield return new Section<DateTimePickerSection> { Text = "Date / Time" };
			yield return new Section<ComboBoxSection> { Text = "Combo Box" };
			yield return new Section<GroupBoxSection> { Text = "Group Box" };
			yield return new Section<SliderSection> { Text = "Slider" };
			yield return new Section<GridViewSection> { Text = "Grid View" };
			yield return new Section<GridCellFormattingSection> { Text = "Grid Cell Formatting" };
			yield return new Section<PasswordBoxSection> { Text = "Password Box" };
			yield return new Section<ProgressBarSection> { Text = "Progress Bar" };
			yield return new Section<KitchenSinkSection> { Text = "Kitchen Sink" };
			yield return new Section<ImageViewSection> { Text = "Image View" };
			yield return new Section<RadioButtonListSection> { Text = "Radio Button List" };
			yield return new Section<SplitterSection> { Text = "Splitter" };
		}
		
		static IEnumerable<Section> DrawingSection()
		{
			yield return new Section<BitmapSection> { Text = "Bitmap" };
			yield return new Section<IndexedBitmapSection> { Text = "Indexed Bitmap" };
			yield return new Section<GraphicsPathSection> { Text = "Graphics Path" };
			yield return new Section<AntialiasSection> { Text = "Antialias" };
			yield return new Section<DrawTextSection> { Text = "Draw Text" };
			yield return new Section<FontsSection> { Text = "Control Fonts" };
			yield return new Section<InterpolationSection> { Text = "Image Interpolation" };
			yield return new Section<PenSection> { Text = "Pens" };
			yield return new Section<PixelOffsetSection> { Text = "Pixel Offset" };
			yield return new Section<TransformSection> { Text = "Transform" };
			yield return new Section<BrushSection> { Text = "Brushes" };
			yield return new Section<TextureBrushesSection> { Text = "Texture Brushes" };
			yield return new Section<TextureBrushesSection2> { Text = "Texture Brushes 2" };
			yield return new Section<ClipSection> { Text = "Clip" };
			yield return new Section<ClearSection> { Text = "Clear" };
			yield return new Section<DirectDrawingSection> { Text = "Direct Drawing" };
			yield return new Section<UnitTestSection> { Text = "Unit Tests" };
		}
		
		static IEnumerable<Section> LayoutsSection()
		{
			yield return new Section("Table Layout", TableLayoutsSection());
			yield return new Section("Scrollable Layout", ScrollableLayoutSection());
			yield return new Section("Pixel Layout", PixelLayoutSection());
		}
		
		static IEnumerable<Section> TableLayoutsSection()
		{
			yield return new Section<Sections.Layouts.TableLayoutSection.RuntimeSection> { Text = "Runtime Creation" };
			yield return new Section<Sections.Layouts.TableLayoutSection.SpacingSection> { Text = "Spacing" };
			yield return new Section<Sections.Layouts.TableLayoutSection.ScalingSection> { Text = "Scaling" };
			yield return new Section<Sections.Layouts.TableLayoutSection.ChildWidthSection> { Text = "Child Width" };
		}
		
		static IEnumerable<Section> ScrollableLayoutSection()
		{
			yield return new Section<Sections.Layouts.ScrollingLayouts.TableLayoutExpansion> { Text = "Table Layout Expansion" };
			yield return new Section<Sections.Layouts.ScrollingLayouts.DockLayoutExpansion> { Text = "Dock Layout Expansion" };
			yield return new Section<Sections.Layouts.ScrollingLayouts.PixelLayoutExpansion> { Text = "Pixel Layout Expansion" };
		}

		static IEnumerable<Section> PixelLayoutSection()
		{
			yield return new Section<Sections.Layouts.PixelLayoutSection.AnchorSection > { Text = "Anchor using PixelLayout" };
		}

		static IEnumerable<Section> DialogsSection()
		{
			yield return new Section<Sections.Dialogs.ColorDialogSection> { Text = "Color Dialog" };
			yield return new Section<Sections.Dialogs.FileDialogSection> { Text = "File Dialog" };
			yield return new Section<Sections.Dialogs.SelectFolderSection> { Text = "Select Folder Dialog" };
			yield return new Section<Sections.Dialogs.CustomDialogSection> { Text = "Custom Dialog" };
			yield return new Section<Sections.Dialogs.FontDialogSection> { Text = "Font Dialog" };
			yield return new Section<Sections.Dialogs.MessageBoxSection> { Text = "Message Box" };
		}
		
		static IEnumerable<Section> SerializationSection()
		{
			yield return new Section<Sections.Serialization.JsonReadSection> { Text = "Json" };
#if XAML
			yield return new Section<Sections.Serialization.XamlReadSection> { Text = "Xaml" };
#endif
		}
		
		static IEnumerable<Section> PrintingSection()
		{
			yield return new Section<Sections.Printing.PrintDialogSection> { Text = "Print Dialog" };
		}
		
		static IEnumerable<Section> BehaviorsSection()
		{
			yield return new Section<Sections.Behaviors.FocusEventsSection> { Text = "Focus Events" };
			yield return new Section<Sections.Behaviors.MouseEventsSection> { Text = "Mouse Events" };
			yield return new Section<Sections.Behaviors.KeyEventsSection> { Text = "Key Events" };
			yield return new Section<Sections.Behaviors.BadgeLabelSection> { Text = "Badge Label" };
			yield return new Section<Sections.Behaviors.ScreenSection> { Text = "Screens" };
			if (Generator.Current.Supports <IMouse> ())
				yield return new Section<Sections.Behaviors.MousePositionSection> { Text = "Mouse Position" };
#if DESKTOP
			yield return new Section<Sections.Behaviors.ContextMenuSection> { Text = "Context Menu" };
			yield return new Section<Sections.Behaviors.WindowsSection> { Text = "Windows" };
#endif
		}

		static IEnumerable<Section> FormsSection()
		{
			yield return new Sections.FormsSection.ImageViewFormSection { Text = "ImageView Form" };			
		}
	}
}
