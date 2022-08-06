using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Android.Drawing;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
using au = Android.Util;

namespace Eto.Android.Forms.Controls
{
	internal class DrawableHandler : AndroidControl<DrawableHandler.DrawView, Drawable, Drawable.ICallback>, Drawable.IHandler
	{
		internal class DrawView : av.View, av.GestureDetector.IOnGestureListener
		{
			private DrawableHandler handler;
			
			private aw.Scroller scroller;
			private av.GestureDetector gestureDetector;
			private IOnTouchListener _InnerTouchListener;

			private Padding padding = Padding.Empty;
			private Size scrollSize;
			private Point scrollPosition;

			internal ag.Matrix Transform
			{
				get => _Transform;
				set
				{
					_Transform = value;
					value.Invert(_InverseTransform);
				}
			}

			internal ag.Matrix InverseTransform => _InverseTransform;

			private ag.Matrix _Transform = new ag.Matrix();
			private ag.Matrix _InverseTransform = new ag.Matrix();

			public DrawView(DrawableHandler handler, ac.Context context) : base(context)
			{
				this.handler = handler;
				scroller = new aw.Scroller(Context);
				gestureDetector = new av.GestureDetector(context, this);

				// Try to initialize scrollbars, but ignore failure. This method is marked as 'removed' even though it's actually not,
				// and source code indicates that at API level 21 it ignores the passed parameter. However this could change without warning in
				// future Android versions.
				// See: https://stackoverflow.com/questions/26448771/initializescrollbars-is-undefined
				try
				{
					var TA = Platform.AppContextThemed.ObtainStyledAttributes(new int[0]);
					InitializeScrollbars(TA);
					TA.Recycle();
				}
				catch { }

				ScrollBarStyle = av.ScrollbarStyles.InsideOverlay;
				HorizontalScrollBarEnabled = true;
				VerticalScrollBarEnabled = true;

				SetLayerType(av.LayerType.Hardware, null);
			}

			public override void SetOnTouchListener(IOnTouchListener l)
			{
				_InnerTouchListener = l;
			}

			public override Boolean OnTouchEvent(av.MotionEvent e)
			{
				if (e.Action == av.MotionEventActions.Cancel)
					PostInvalidateOnAnimation();

				if (e.Action == av.MotionEventActions.Up)
					PostInvalidateOnAnimation();

				if (gestureDetector.OnTouchEvent(e))
					return true;

				if (_InnerTouchListener?.OnTouch(this, e) ?? false)
					return true;

				return base.OnTouchEvent(e);
			}

			protected override void OnDraw(ag.Canvas canvas)
			{
				// Android applies a transformation matrix based on scroll and there's no way to prevent it, but Eto Drawable consumers are not expecting that,
				// so we have to apply an inverse transformation to undo it.

				canvas.Save(ag.SaveFlags.Matrix | ag.SaveFlags.Clip);

				try
				{
					// TODO: The double translation could be avoided if we maintained scroll size/position internally in px rather than dp.
					canvas.Translate(ScrollPosition.X, ScrollPosition.Y);
					canvas.Concat(_Transform);
					canvas.Translate(-ScrollPosition.X, -ScrollPosition.Y);

					handler.PaintMe(canvas);
				}

				catch
				{
					// If any error during drawing, just fill with red. Allowing the exception to propogate
					// would crash the app. Since exceptions here should only be drawing-related and not
					// business logic, suppressing exceptions should be acceptable, and better than a crash.
					canvas.DrawRGB(255, 0, 0);
				}

				finally
				{
					// TODO: Actually need to save and restore?
					canvas.Restore();
				}

				OnDrawScrollBars(canvas);
			}

			public Padding Padding
			{
				get => padding;
				set
				{
					if (padding == value)
						return;

					padding = value;
					SetPadding(Platform.DpToPx(value.Left), Platform.DpToPx(value.Top), Platform.DpToPx(value.Right), Platform.DpToPx(value.Bottom));
				}
			}

			public Size ScrollSize
			{
				get { return scrollSize; }
				set
				{
					scrollSize = value;
					AwakenScrollBars();
				}
			}

			public Point ScrollPosition
			{
				get { return scrollPosition; }
				set
				{
					scrollPosition = value;
					ScrollTo((int)value.X, (int)value.Y);
					AwakenScrollBars();
				}
			}

			protected override int ComputeHorizontalScrollExtent() => Platform.PxToDp(Width);

			protected override int ComputeHorizontalScrollOffset() => (int)scrollPosition.X;

			protected override int ComputeHorizontalScrollRange() => (int)scrollSize.Width;

			protected override int ComputeVerticalScrollExtent() => Platform.PxToDp(Height);

			protected override int ComputeVerticalScrollOffset() => (int)scrollPosition.Y;

			protected override int ComputeVerticalScrollRange() => (int)scrollSize.Height;

			public override void ComputeScroll()
			{
				base.ComputeScroll();

				if (scroller.ComputeScrollOffset())
				{
					ScrollPosition = new Point(scroller.CurrX, scroller.CurrY);
					AwakenScrollBars();
				}

				if (!scroller.IsFinished)
					PostInvalidateOnAnimation();
			}

			Boolean av.GestureDetector.IOnGestureListener.OnDown(av.MotionEvent e)
			{
				scroller.ForceFinished(true);
				return true;
			}

			Boolean av.GestureDetector.IOnGestureListener.OnFling(av.MotionEvent e1, av.MotionEvent e2, Single velocityX, Single velocityY)
			{
				var MaxX = Math.Max(0, scrollSize.Width - Platform.PxToDp(Width) + Padding.Horizontal);
				var MaxY = Math.Max(0, scrollSize.Height - Platform.PxToDp(Height) + Padding.Vertical);

				scroller.Fling((int)ScrollPosition.X, (int)ScrollPosition.Y, (int)-velocityX, (int)-velocityY, 0, (int)MaxX, 0, (int)MaxY);

				if (!AwakenScrollBars())
					PostInvalidateOnAnimation();

				return true;
			}

			void av.GestureDetector.IOnGestureListener.OnLongPress(av.MotionEvent e)
			{
			}

			Boolean av.GestureDetector.IOnGestureListener.OnScroll(av.MotionEvent e1, av.MotionEvent e2, Single distanceX, Single distanceY)
			{
				var NewX = ScrollPosition.X + Platform.PxToDp(distanceX);
				var MaxX = scrollSize.Width - Platform.PxToDp(Width) + Padding.Horizontal;

				var NewY = ScrollPosition.Y + Platform.PxToDp(distanceY);
				var MaxY = scrollSize.Height - Platform.PxToDp(Height) + Padding.Vertical;

				ScrollPosition = new Point((Int32)Math.Max(Math.Min(NewX, MaxX), 0), (Int32)Math.Max(Math.Min(NewY, MaxY), 0));

				if (AwakenScrollBars())
					Invalidate();

				return true;
			}

			void av.GestureDetector.IOnGestureListener.OnShowPress(av.MotionEvent e)
			{
			}

			Boolean av.GestureDetector.IOnGestureListener.OnSingleTapUp(av.MotionEvent e)
			{
				// If we started handling the gesture on down we suppressed the usual Eto MouseDown event. If it now
				// turns out that the gesture was just a tap, trigger the missing MouseDown now before MouseUp gets sent,
				// so subscribers that were expecting both are not confused.

				var downMotion = av.MotionEvent.Obtain(e);
				downMotion.Action = av.MotionEventActions.Down;

				var downEvent = new TouchEventArgs(true, downMotion);

				handler.Control_Touch(null, downEvent);

				downMotion.Recycle();

				return false;
			}
		}

		protected override void Control_Touch(object sender, av.View.TouchEventArgs e)
		{
			e.Event.Transform(Control.InverseTransform);
			base.Control_Touch(sender, e);
		}

		private void PaintMe(ag.Canvas canvas)
		{
			var GH = new GraphicsHandler(canvas);
			var G = new Graphics(GH);

			Callback.OnPaint(Widget, new PaintEventArgs(G, ScrollViewport));
		}

		public IMatrix PixelToDeviceMatrix => Control.InverseTransform.ToEto();
		public IMatrix DeviceToPixelMatrix => Control.Transform.ToEto();

		public void Create()
		{
			var Transform = new ag.Matrix();
			var SingleDpSize = Platform.DpToPx(1f);
			Transform.SetScale(SingleDpSize, SingleDpSize);

			Control = new DrawView(this, Platform.AppContextThemed) { Transform = Transform };
		}

		public void Create(Boolean largeCanvas)
		{
			Create();
		}

		public void Update(Rectangle region)
		{
			Control.Invalidate(region.ToAndroid());
		}

		public Boolean SupportsCreateGraphics => true;

		public Graphics CreateGraphics()
		{
			return new Graphics(new GraphicsHandler(new ag.Canvas()));
		}

		public Control Content { get; set; }

		public Padding Padding
		{
			get => Control.Padding;
			set => Control.Padding = value;
		}

		public Size MinimumSize { get; set; }

		public Size ClientSize
		{
			get => Size;
			set => Size = value;
		}

		public bool RecurseToChildren => false;

		public Boolean CanFocus
		{
			get => false;
			set { }
		}

		public override void Focus() 
		{
			if(CanFocus)
				base.Focus();
		}

		public ContextMenu ContextMenu { get; set; }

		public override av.View ContainerControl => Control;

		public Size ScrollSize
		{
			get { return Control.ScrollSize; }
			set
			{
				Control.ScrollSize = value;
				ScrollPosition = ScrollPosition;
			}
		}

		public Point ScrollPosition
		{
			get { return Control.ScrollPosition; }
			set
			{
				if (value.X > ScrollSize.Width - Width)
					value.X = ScrollSize.Width - Width;

				if (value.Y > ScrollSize.Height - Height)
					value.Y = ScrollSize.Height - Height;

				if (value.X < 0)
					value.X = 0;

				if (value.Y < 0)
					value.Y = 0;

				Control.ScrollPosition = value;
			}
		}

		public Rectangle ScrollViewport => new Rectangle(ScrollPosition, ClientSize);
	}
}
