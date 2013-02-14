using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class FormHandler : WindowHandler<FormHandler.MyForm, Form>, IForm
	{
        public class MyForm : swf.Form
        {
            public bool HideFromAltTab { get; set; }

            public bool ShouldShowWithoutActivation { get; set; }


            protected override bool ShowWithoutActivation
            {
                get
                {
                    return ShouldShowWithoutActivation;
                }
            }

            /// <summary>
            /// Hide the overlay from the Alt-Tab list
            /// </summary>
            protected override swf.CreateParams CreateParams
            {
                get
                {
                    var createParams = 
                        base.CreateParams;
                    
                    // Turn on WS_EX_TOOLWINDOW style bit
                    // if needed.
                    if (HideFromAltTab)
                        createParams.ExStyle |= 0x80;

					// Turn on WS_EX_COMPOSITED. This is needed to
					// make Drawable work correctly as a Container.
					// see http://stackoverflow.com/a/2613272/90291
					createParams.ExStyle |= 0x02000000;  

                    return createParams;
                }
            }

#if Windows
            const int WM_ACTIVATE = 0x0006;
            const int WA_INACTIVE = 0;
        
            private ParentWindowIntercept parentWindowIntercept;

            public Action<swf.Control> DeactivateDelegate { get; set; }

            public MyForm()
            {
                this.Load += (s, e) =>
                {

                    this.parentWindowIntercept =
                        new ParentWindowIntercept(
                            this.Handle)
                            {
                                DeactivateDelegate =
                                    control =>
                                    {
                                        if (this.DeactivateDelegate != null)
                                            this.DeactivateDelegate(
                                                control);
                                    }
                            };
                };
            }

            /// <summary>
            /// Because System.Windows.Form's Deactivate event
            /// does not provide the control being activated
            /// when a deactivate occurs, 
            /// </summary>
            private class ParentWindowIntercept : swf.NativeWindow
            {
                public Action<swf.Control> DeactivateDelegate { get; set; }

                public ParentWindowIntercept(
                    IntPtr hWnd)
                {
                    this.AssignHandle(hWnd);
                }

                protected override void WndProc(ref swf.Message m)
                {
                    if (m.Msg == WM_ACTIVATE)
                    {
                        if ((int)m.WParam == WA_INACTIVE)
                        {
                            IntPtr windowFocusGoingTo = m.LParam;

                            var control =
                                swf.Control.FromHandle(
                                    windowFocusGoingTo);

                            // Call the delegate
                            if (DeactivateDelegate != null)
                                DeactivateDelegate.Invoke(
                                    control);
                        }
                    }

                    base.WndProc(ref m);
                }
            }
#endif

        }

		public FormHandler()
		{
			Control = new MyForm {
				StartPosition = swf.FormStartPosition.CenterParent,
				AutoSize = true,
				Size = sd.Size.Empty,
				MinimumSize = sd.Size.Empty
			};
		}

        public void Show()
		{
			Control.Show();
		}

        public Color TransparencyKey
        {
            get { return Control.TransparencyKey.ToEto(); }
            set { Control.TransparencyKey = value.ToSD(); }
        }


        public bool KeyPreview
        {
            get { return Control.KeyPreview; }
            set { Control.KeyPreview = value; }
        }
    }
}
