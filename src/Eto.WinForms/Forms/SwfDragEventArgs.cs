using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.WinForms.Forms
{
	class SwfDragEventArgs : DragEventArgs
	{
		string _format;
		string _inner;
		public SwfDragEventArgs(Control source, DataObject data, DragEffects allowedEffects, PointF location, Keys modifiers, MouseButtons buttons, object controlObject = null)
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
			var data = Data.ToSwf();
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
			swf.SwfDataObjectExtensions.SetDropDescription(data, (swf.DropImageType)Effects.ToSwf(), format, inner);
		}
	}
}
