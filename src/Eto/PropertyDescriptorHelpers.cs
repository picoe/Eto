namespace Eto;

static class PropertyDescriptorHelpers
{
	public static T GetCustomAttribute<T>(this PropertyDescriptor descriptor) where T : Attribute
	{
		return descriptor.Attributes.OfType<T>().FirstOrDefault();
	}
}
