using System;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.iOS.Drawing;
using MonoTouch.UIKit;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.OpenGLES;
using MonoTouch.GLKit;
using System.Collections.Generic;
using Eto.Platform.Mac.Forms;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class DrawableHandler : MacDockContainer<DrawableHandler.EtoView, Drawable>, IDrawable
	{
		/*
		[Register("FastLayer")]
		public class FastLayer : CATiledLayer
		{
			[Export("fadeDuration")]
			public static float fadeDuration {
				get { return 0.0f; }
			}
		}*/

		public override UIView ContainerControl { get { return Control; } }

		public class EtoTiledView : EtoView
		{
			[Export("layerClass")]
			public static Class LayerClass()
			{
				return new Class(typeof(CATiledLayer));
			}

			public EtoTiledView()
			{
				var tiledLayer = (CATiledLayer)this.Layer;
				if (UIScreen.MainScreen.RespondsToSelector(new Selector("scale")) && UIScreen.MainScreen.Scale == 2.0f)
				{
					tiledLayer.TileSize = new SD.SizeF(512, 512);
				}
				tiledLayer.LevelsOfDetail = 4;
			}
		}
		// A UITextPosition object represents a position in a text container; in other words, it is
		// an index into the backing string in a text-displaying view.
		//
		// Classes that adopt the UITextInput protocol must create custom UITextPosition objects
		// for representing specific locations within the text managed by the class. The text input
		// system uses both these objects and UITextRange objects for communicating text-layout information.
		//
		// We could use more sophisticated objects, but for demonstration purposes it suffices to wrap integers.
		class IndexedPosition : UITextPosition
		{
			public int Index { get; private set; }

			private IndexedPosition(int index)
			{
				this.Index = index;
			}
			// We need to keep all the IndexedPositions we create accessible from managed code,
			// otherwise the garbage collector will collect them since it won't know that native
			// code has references to them.
			static Dictionary<int, IndexedPosition> indexes = new Dictionary<int, IndexedPosition>();

			public static IndexedPosition GetPosition(int index)
			{
				IndexedPosition result;

				if (!indexes.TryGetValue(index, out result))
				{
					result = new IndexedPosition(index);
					indexes[index] = result;
				}
				return result;
			}
		}
		// A UITextRange object represents a range of characters in a text container; in other words,
		// it identifies a starting index and an ending index in string backing a text-entry object.
		//
		// Classes that adopt the UITextInput protocol must create custom UITextRange objects for
		// representing ranges within the text managed by the class. The starting and ending indexes
		// of the range are represented by UITextPosition objects. The text system uses both UITextRange
		// and UITextPosition objects for communicating text-layout information.
		class IndexedRange : UITextRange
		{
			public NSRange Range { get; private set; }

			private IndexedRange()
			{
			}

			public override UITextPosition start { get { return IndexedPosition.GetPosition(Range.Location); } }

			public override UITextPosition end { get { return IndexedPosition.GetPosition(Range.Location + Range.Length); } }

			public override bool IsEmpty { get { return Range.Length == 0; } }
			// We need to keep all the IndexedRanges we create accessible from managed code,
			// otherwise the garbage collector will collect them since it won't know that native
			// code has references to them.
			private static Dictionary<NSRange, IndexedRange> ranges = new Dictionary<NSRange, IndexedRange>(new NSRangeEqualityComparer());

			public static IndexedRange GetRange(NSRange theRange)
			{
				IndexedRange result;

				if (theRange.Location == NSRange.NotFound)
					return null;

				if (!ranges.TryGetValue(theRange, out result))
				{
					result = new IndexedRange();
					result.Range = new NSRange(theRange.Location, theRange.Length);
				}
				return result;
			}

			class NSRangeEqualityComparer : IEqualityComparer<NSRange>
			{

				#region IEqualityComparer[NSRange] implementation

				public bool Equals(NSRange x, NSRange y)
				{
					return x.Length == y.Length && x.Location == y.Location;
				}

				public int GetHashCode(NSRange obj)
				{
					return obj.Location.GetHashCode() ^ obj.Length.GetHashCode();
				}

				#endregion

			}
		}

		[Adopts("UITextInput")]
		[Adopts("UIKeyInput")]
		[Adopts("UITextInputTraits")]
		[Register("MyView")]
		public class EtoView : UIView
		{
			public override void TouchesBegan(NSSet touches, UIEvent evt)
			{
				var args = Conversions.ConvertMouse(this, touches, evt);
				Handler.Widget.OnMouseDown(args);
				if (!args.Handled)
					base.TouchesBegan(touches, evt);
			}

			public override void TouchesEnded(NSSet touches, UIEvent evt)
			{
				var args = Conversions.ConvertMouse(this, touches, evt);
				Handler.Widget.OnMouseUp(args);
				if (!args.Handled)
					base.TouchesEnded(touches, evt);
			}

			public override void TouchesMoved(NSSet touches, UIEvent evt)
			{
				var args = Conversions.ConvertMouse(this, touches, evt);
				Handler.Widget.OnMouseMove(args);
				if (!args.Handled)
					base.TouchesMoved(touches, evt);
			}

			public EtoView()
			{
				//var tiledLayer = this.Layer as CATiledLayer;
				this.BackgroundColor = UIColor.Clear;
				this.ContentMode = UIViewContentMode.Redraw;
			}

			WeakReference handler;

			public DrawableHandler Handler { get { return (DrawableHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override void Draw(System.Drawing.RectangleF rect)
			{
				//Console.WriteLine ("Drawing {0}, {1}", rect, new System.Diagnostics.StackTrace ());
				Handler.Update(rect.ToEtoRectangle());
			}

			public bool CanFocus { get; set; }

			public override bool CanBecomeFirstResponder
			{
				get { return CanFocus; }
			}

			public override bool BecomeFirstResponder()
			{
				Handler.Widget.OnGotFocus(EventArgs.Empty);
				return base.BecomeFirstResponder();
			}

			public override bool ResignFirstResponder()
			{
				Handler.Widget.OnLostFocus(EventArgs.Empty);
				return base.ResignFirstResponder();
			}

			static IntPtr selFrame = Selector.GetHandle("frame");

			public SD.RectangleF BaseFrame
			{
				get
				{
					SD.RectangleF result;
					Messaging.RectangleF_objc_msgSend_stret(out result, base.Handle, selFrame);
					return result;
				}
			}

			#region Text Handling - could not move this into a superclass because of protocol registration issues.

			NSDictionary markedTextStyle;

			[Export("markedTextStyle")]
			public NSDictionary MarkedTextStyle
			{
				get { return markedTextStyle; }
				set { markedTextStyle = value; }
			}

			UITextInputDelegate inputDelegate;

			[Export("inputDelegate")]
			public UITextInputDelegate InputDelegate
			{
				get { return inputDelegate; }
				set { inputDelegate = value; }
			}

			#region UITextInput - Replacing and Returning Text

			// UITextInput required method - called by text system to get the string for
			// a given range in the text storage
			[Export("textInRange:")]
			string TextInRange(UITextRange range)
			{
				throw new NotImplementedException();
			}
			// UITextInput required method - called by text system to replace the given
			// text storage range with new text
			[Export("replaceRange:withText:")]
			void ReplaceRange(UITextRange range, string text)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region UITextInput - Working with Marked and Selected Text

			// UITextInput selectedTextRange property accessor overrides
			// (access/update underlaying SimpleCoreTextView)
			[Export("selectedTextRange")]
			IndexedRange SelectedTextRange
			{
				get
				{
					return null;
				}
				set
				{
					//throw new NotImplementedException();
				}
			}
			// UITextInput markedTextRange property accessor overrides
			// (access/update underlaying SimpleCoreTextView)
			[Export("markedTextRange")]
			IndexedRange MarkedTextRange
			{
				get { return null; }
			}
			// UITextInput required method - Insert the provided text and marks it to indicate
			// that it is part of an active input session.
			[Export("setMarkedText:selectedRange:")]
			void SetMarkedText(string markedText, NSRange selectedRange)
			{
				throw new NotImplementedException();
			}
			// UITextInput required method - Unmark the currently marked text.
			[Export("unmarkText")]
			void UnmarkText()
			{
				
			}

			#endregion

			#region UITextInput - Computing Text Ranges and Text Positions

			// UITextInput beginningOfDocument property accessor override
			[Export("beginningOfDocument")]
			IndexedPosition BeginningOfDocument
			{
				get
				{
					throw new NotImplementedException();
				}
			}
			// UITextInput endOfDocument property accessor override
			[Export("endOfDocument")]
			IndexedPosition EndOfDocument
			{
				get
				{
					throw new NotImplementedException();
				}
			}
			// UITextInput protocol required method - Return the range between two text positions
			// using our implementation of UITextRange
			[Export("textRangeFromPosition:toPosition:")]
			IndexedRange GetTextRange(UITextPosition fromPosition, UITextPosition toPosition)
			{
				throw new NotImplementedException();
			}
			// UITextInput protocol required method - Returns the text position at a given offset
			// from another text position using our implementation of UITextPosition
			[Export("positionFromPosition:offset:")]
			IndexedPosition GetPosition(UITextPosition position, int offset)
			{
				throw new NotImplementedException();
			}
			// UITextInput protocol required method - Returns the text position at a given offset
			// in a specified direction from another text position using our implementation of
			// UITextPosition.
			[Export("positionFromPosition:inDirection:offset:")]
			IndexedPosition GetPosition(UITextPosition position, UITextLayoutDirection direction, int offset)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region UITextInput - Evaluating Text Positions

			// UITextInput protocol required method - Return how one text position compares to another
			// text position.
			[Export("comparePosition:toPosition:")]
			NSComparisonResult ComparePosition(UITextPosition position, UITextPosition other)
			{
				throw new NotImplementedException();
			}
			// UITextInput protocol required method - Return the number of visible characters
			// between one text position and another text position.
			[Export("offsetFromPosition:toPosition:")]
			int GetOffset(IndexedPosition @from, IndexedPosition toPosition)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region UITextInput - Text Input Delegate and Text Input Tokenizer

			// UITextInput tokenizer property accessor override
			//
			// An input tokenizer is an object that provides information about the granularity
			// of text units by implementing the UITextInputTokenizer protocol.  Standard units
			// of granularity include characters, words, lines, and paragraphs. In most cases,
			// you may lazily create and assign an instance of a subclass of
			// UITextInputStringTokenizer for this purpose, as this sample does. If you require
			// different behavior than this system-provided tokenizer, you can create a custom
			// tokenizer that adopts the UITextInputTokenizer protocol.
			UITextInputTokenizer tokenizer;

			[Export("tokenizer")]
			UITextInputTokenizer Tokenizer
			{
				get { return tokenizer = tokenizer ?? new UITextInputStringTokenizer(this); }
			}

			#endregion

			#region UITextInput - Text Layout, writing direction and position related methods

			// UITextInput protocol method - Return the text position that is at the farthest
			// extent in a given layout direction within a range of text.
			[Export("positionWithinRange:farthestInDirection:")]
			IndexedPosition GetPosition(UITextRange range, UITextLayoutDirection direction)
			{
				throw new NotImplementedException();
			}
			// UITextInput protocol required method - Return a text range from a given text position
			// to its farthest extent in a certain direction of layout.
			[Export("characterRangeByExtendingPosition:inDirection:")]
			IndexedRange GetCharacterRange(UITextPosition position, UITextLayoutDirection direction)
			{
				throw new NotImplementedException();
			}
			// UITextInput protocol required method - Return the base writing direction for
			// a position in the text going in a specified text direction.
			[Export("baseWritingDirectionForPosition:inDirection:")]
			UITextWritingDirection GetBaseWritingDirection(UITextPosition position, UITextStorageDirection direction)
			{
				return UITextWritingDirection.LeftToRight;
			}
			// UITextInput protocol required method - Set the base writing direction for a
			// given range of text in a document.
			[Export("setBaseWritingDirection:forRange:")]
			void SetBaseWritingDirection(UITextWritingDirection writingDirection, UITextRange range)
			{
				// This sample assumes LTR text direction and does not currently support BiDi or RTL.
			}

			#endregion

			#region UITextInput - Geometry methods

			// UITextInput protocol required method - Return the first rectangle that encloses
			// a range of text in a document.
			[Export("firstRectForRange:")]
			RectangleF FirstRect(UITextRange range)
			{
				throw new NotImplementedException();
			}
			// UITextInput protocol required method - Return a rectangle used to draw the caret
			// at a given insertion point.
			[Export("caretRectForPosition:")]
			RectangleF CaretRect(UITextPosition position)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region UITextInput - Hit testing

			// Note that for this sample hit testing methods are not implemented, as there
			// is no implemented mechanic for letting user select text via touches.  There
			// is a wide variety of approaches for this (gestures, drag rects, etc) and
			// any approach chosen will depend greatly on the design of the application.
			// UITextInput protocol required method - Return the position in a document that
			// is closest to a specified point.
			[Export("closestPositionToPoint:")]
			UITextPosition ClosestPosition(PointF point)
			{
				return null;
			}
			// UITextInput protocol required method - Return the position in a document that
			// is closest to a specified point in a given range.
			[Export("closestPositionToPoint:withinRange:")]
			UITextPosition ClosestPosition(PointF point, UITextRange range)
			{
				return null;
			}
			// UITextInput protocol required method - Return the character or range of
			// characters that is at a given point in a document.
			[Export("characterRangeAtPoint:")]
			UITextRange CharacterRange(PointF point)
			{
				// Not implemented in this sample.  Could utilize underlying 
				// SimpleCoreTextView:closestIndexToPoint:point
				return null;
			}

			#endregion

			#region UITextInput - Returning Text Styling Information

			// UITextInput protocol method - Return a dictionary with properties that specify
			// how text is to be style at a certain location in a document.
			[Export("textStylingAtPosition:inDirection:")]
			NSDictionary TextStyling(UITextPosition position, UITextStorageDirection direction)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region UIKeyInput methods

			// UIKeyInput required method - A Boolean value that indicates whether the text-entry
			// objects have any text.
			[Export("hasText")]
			bool HasText
			{
				get { throw new NotImplementedException(); }
			}
			// UIKeyInput required method - Insert a character into the displayed text.
			// Called by the text system when the user has entered simple text
			[Export("insertText:")]
			void InsertText(string text)
			{
				var args = new TextInputEventArgs { Text = text };
				Handler.Widget.OnTextInput(args);
			}
			// UIKeyInput required method - Delete a character from the displayed text.
			// Called by the text system when the user is invoking a delete (e.g. pressing
			// the delete software keyboard key)
			[Export("deleteBackward")]
			void DeleteBackward()
			{
				var args = new TextInputEventArgs { DeleteBackwards = true };
				Handler.Widget.OnTextInput(args);
			}

			#endregion

			#endregion
		}

		public void Create()
		{
			Control = new EtoView { Handler = this };
		}

		public void Create(bool largeCanvas)
		{
			if (largeCanvas)
				Control = new EtoTiledView { Handler = this };
			else
				Control = new EtoView { Handler = this };
		}

		public bool CanFocus
		{
			get { return Control.CanFocus; }
			set { Control.CanFocus = value; }
		}

		public void Update(Rectangle rect)
		{
			var context = UIGraphics.GetCurrentContext();
			if (context != null)
			{
				/*var scale = context.GetCTM().xx;  // .a			// http://developer.apple.com/library/ios/#documentation/GraphicsImaging/Reference/CGAffineTransform/Reference/reference.html#//apple_ref/doc/c_ref/CGAffineTransform
				var tiledLayer = (CATiledLayer)this.Layer;
				var tileSize = tiledLayer.TileSize;
				
			    tileSize.Width /= scale;
			    tileSize.Height /= scale;*/
				//lock (this) {
				//context.TranslateCTM(0, 0);
				//context.ScaleCTM(1, -1);
				//var oldCheck = UIApplication.CheckForIllegalCrossThreadCalls;
				//UIApplication.CheckForIllegalCrossThreadCalls = false;
				
				using (var graphics = new Graphics(Widget.Generator, new GraphicsHandler(Control, context, Control.BaseFrame.Height, true)))
				{
					Widget.OnPaint(new PaintEventArgs(graphics, rect));
				}
				//UIApplication.CheckForIllegalCrossThreadCalls = oldCheck;
				//}
			}
		}
	}
}
