using System;

namespace Eto.Wpf
{
	/// <summary>
	/// Non-public WPF control types, exposed so a theme can reference them from XAML with
	/// <c>{x:Static}</c> where <c>{x:Type}</c> cannot.
	/// </summary>
	/// <remarks>
	/// WPF matches an implicit style on the element's EXACT type and never walks up to a base type,
	/// so the menus WPF builds for itself out of internal ContextMenu / MenuItem subclasses - most
	/// visibly the editing context menu you get by right-clicking a TextBox - skip a theme's
	/// <c>TargetType="ContextMenu"</c> / <c>"MenuItem"</c> styles and come up in system chrome. A
	/// theme fixes that by keying the same style under the derived type as well (see
	/// themes/palette/EtoMenuItem.xaml); a style whose TargetType is a base class of the element it
	/// is applied to is valid, so no other change is needed.
	/// <para>
	/// A key of <see langword="null"/> can't go in a ResourceDictionary, so a name that a future
	/// .NET release renames or removes resolves to <see cref="UnresolvedType"/> instead - a
	/// resource entry nothing ever looks up - rather than failing to load the whole dictionary.
	/// </para>
	/// </remarks>
	public static class WpfInternalTypes
	{
		/// <summary>
		/// Stands in for a type that could not be resolved. See the remarks on
		/// <see cref="WpfInternalTypes"/>.
		/// </summary>
		public sealed class UnresolvedType
		{
		}

		// Note this deliberately avoids Type.GetType("Ns.Type, PresentationFramework"): a partial
		// assembly name resolves on .NET but returns null on .NET Framework, and Eto.Wpf targets both.
		static Type Find(string typeName) =>
			typeof(sw.FrameworkElement).Assembly.GetType(typeName, false) ?? typeof(UnresolvedType);

		/// <summary>The context menu of a TextBox / RichTextBox and friends: Cut, Copy, Paste, …</summary>
		public static Type EditorContextMenu { get; } = Find("System.Windows.Documents.TextEditorContextMenu+EditorContextMenu");

		/// <summary>An item of the <see cref="EditorContextMenu"/>, including spelling suggestions.</summary>
		public static Type EditorMenuItem { get; } = Find("System.Windows.Documents.TextEditorContextMenu+EditorMenuItem");

		/// <summary>The IME reconversion item of the <see cref="EditorContextMenu"/>.</summary>
		public static Type ReconversionMenuItem { get; } = Find("System.Windows.Documents.TextEditorContextMenu+ReconversionMenuItem");

		/// <summary>The context menu of a DocumentViewer.</summary>
		public static Type ViewerContextMenu { get; } = Find("MS.Internal.Documents.DocumentGridContextMenu+ViewerContextMenu");

		/// <summary>An item of the <see cref="ViewerContextMenu"/>.</summary>
		public static Type ViewerMenuItem { get; } = Find("MS.Internal.Documents.DocumentGridContextMenu+EditorMenuItem");
	}
}
