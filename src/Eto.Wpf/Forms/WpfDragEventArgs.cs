using Eto.Drawing;
using Eto.Forms;
using sw = System.Windows;
using swi = System.Windows.Input;

namespace Eto.Wpf.Forms
{

	class WpfDragEventArgs : DragEventArgs
	{
		string _format;
		string _inner;
		public WpfDragEventArgs(Control source, DataObject data, DragEffects allowedEffects, PointF location, Keys modifiers, MouseButtons buttons, object controlObject = null) 
			: base(source, data, allowedEffects, location, modifiers, buttons, controlObject)
		{
		}

		public override DragEffects Effects
		{
			get => base.Effects;
			set
			{
				base.Effects = value;
				if (_format != null)
					SetDropDescription(_format, _inner);
			}
		}

		public override bool SupportsDropDescription => true;

		public override void SetDropDescription(string format, string inner = null)
		{
			_format = format;
			_inner = inner;
			if (format?.Contains("{0}") == true)
			{
				// when we have %1 in the string, we must double up our percent signs..
				format = format.Replace("%", "%%").Replace("{0}", "%1");
			}
			else
			{
				// can't have %1 in the string, so insert zero width space character inbetween.
				format = format?.Replace("%1", "%\x200b1");
			}

			var data = Data.ToWpf();
			if (sw.DropTargetHelper.IsSupported(data))
				sw.WpfDataObjectExtensions.SetDropDescription(data, (sw.DropImageType)Effects.ToWpf(), format, inner);
		}
	}
}
