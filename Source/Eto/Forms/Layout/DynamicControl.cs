namespace Eto.Forms
{
	[ContentProperty("Control")]
	public class DynamicControl : DynamicItem
	{
		public override Control Create (DynamicLayout layout)
		{
			return Control;
		}

		public Control Control { get; set; }
	}
}
