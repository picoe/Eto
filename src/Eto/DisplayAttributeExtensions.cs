using System.ComponentModel.DataAnnotations;

namespace Eto;

class BaseAttributeWrapper<T>
	where T : Attribute
{
	public static T Get(Type type) => type.GetTypeInfo().GetCustomAttribute<T>();

	public static T Get(PropertyDescriptor descriptor) => descriptor.Attributes.OfType<T>().FirstOrDefault();

	public static T Get(IPropertyDescriptor descriptor) => descriptor.GetCustomAttribute<T>();	}

class EditorAttributeWrapper : BaseAttributeWrapper<EditorAttribute> { }

class DisplayAttributeWrapper : BaseAttributeWrapper<DisplayAttribute> { }