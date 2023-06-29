using Eto.Android.Forms;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android
{
	internal class MessageBoxHandler : WidgetHandler<Widget>, MessageBox.IHandler
	{
		public System.String Text
		{
			get;
			set;
		}

		public System.String Caption
		{
			get;
			set;
		}

		public MessageBoxType Type
		{
			get;
			set;
		}

		public MessageBoxButtons Buttons
		{
			get;
			set;
		}

		public MessageBoxDefaultButton DefaultButton
		{
			get;
			set;
		}

		public DialogResult ShowDialog(Control parent)
		{
#if DEBUG
			ShowDialogWithCallback(r => { }, parent);
			return DialogResult.Ok;
#endif
			throw new NotSupportedException("Android platform supports only async dialogs. Use ShowAsync()");
		}

		public void ShowDialogWithCallback(Action<DialogResult> callback, Control parent)
		{
			var Builder = CreateDialog(callback);
			Builder.Show();
		}

		private aa.AlertDialog.Builder CreateDialog(Action<DialogResult> callback)
		{
			var Handler = (ApplicationHandler)Application.Instance.Handler;
			var Builder = new aa.AlertDialog.Builder(Handler.TopActivity);
			Builder.SetTitle(Caption);
			Builder.SetCancelable(false);
			Builder.SetMessage(Text);

			if (this.Type == MessageBoxType.Warning || Type == MessageBoxType.Error)
				Builder.SetIcon(global::Android.Resource.Drawable.IcDialogAlert);
			
			if (this.Type == MessageBoxType.Information)
				Builder.SetIcon(global::Android.Resource.Drawable.IcDialogInfo);

			if (this.Type == MessageBoxType.Question)
				Builder.SetIcon(global::Android.Resource.Drawable.IcMenuHelp);

			if(Buttons == MessageBoxButtons.OK || Buttons == MessageBoxButtons.OKCancel)
				Builder.SetPositiveButton("OK", (s, e) => callback(DialogResult.Ok));

			if(Buttons == MessageBoxButtons.YesNo || Buttons == MessageBoxButtons.YesNoCancel)
				Builder.SetPositiveButton("Yes", (s, e) => callback(DialogResult.Yes));

			if(Buttons == MessageBoxButtons.YesNo || Buttons == MessageBoxButtons.YesNoCancel)
				Builder.SetNegativeButton("No", (s, e) => callback(DialogResult.No));

			if(Buttons == MessageBoxButtons.OKCancel)
				Builder.SetNegativeButton("Cancel", (s, e) => callback(DialogResult.Cancel));

			if(Buttons == MessageBoxButtons.YesNoCancel)
				Builder.SetNeutralButton("Cancel", (s, e) => callback(DialogResult.Cancel));

			return Builder;
		}
	}
}
