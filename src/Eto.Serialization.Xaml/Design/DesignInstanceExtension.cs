#if PORTABLE
using Portable.Xaml.Markup;
#else
using System.Windows.Markup;
#endif

namespace Eto.Serialization.Xaml.Design
{
	/// <summary>
	/// Creates sample data for the designer, e.g. <c>d:DataContext="{d:DesignInstance local:MyViewModel}"</c>
	/// </summary>
	/// <remarks>
	/// The type also tells the editor which properties <c>{Binding}</c> can use.
	/// Returns null outside of <see cref="Control.IsDesignMode"/>, or when the type has no parameterless constructor.
	/// </remarks>
	[MarkupExtensionReturnType(typeof(object))]
	public class DesignInstanceExtension : MarkupExtension
	{
		/// <summary>
		/// Gets or sets the type of object to create
		/// </summary>
		[ConstructorArgument("type")]
		public Type Type { get; set; }

		/// <summary>
		/// Initializes a new instance of the DesignInstanceExtension class
		/// </summary>
		public DesignInstanceExtension()
		{
		}

		/// <summary>
		/// Initializes a new instance of the DesignInstanceExtension class with the specified type
		/// </summary>
		public DesignInstanceExtension(Type type)
		{
			Type = type;
		}

		/// <inheritdoc/>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (!Control.IsDesignMode || Type == null)
				return null;
			// type-only is still useful for completion, so no default constructor isn't an error
			if (!Type.IsValueType && Type.GetConstructor(Type.EmptyTypes) == null)
				return null;
			return Activator.CreateInstance(Type);
		}
	}
}
