using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms.Controls
{
	public class ProgressBarHandler : AndroidControl<aw.ProgressBar, ProgressBar, ProgressBar.ICallback>, ProgressBar.IHandler
	{
		public override av.View ContainerControl { get { return Control; } }

		public ProgressBarHandler()
		{
			Control = new aw.ProgressBar(Platform.AppContextThemed, null, global::Android.Resource.Attribute.ProgressBarStyleHorizontal)
			{
				Indeterminate = false,
			};
		}

		public override bool Enabled
		{
			get => Control.Enabled;
			set => Control.Enabled = value;
		}

		public int MaxValue
		{
			get => Control.Max;
			set => Control.Max = value;
		}

		public int MinValue
		{
			get => Control.Min;
			set => Control.Min = value;
		}

		public int Value
		{
			get => Control.Progress;
			set => Control.Progress = value;
		}

		public bool Indeterminate
		{ 
			get => Control.Indeterminate;
			set => Control.Indeterminate = value;
		}
	}
}
