using System;
using System.IO;
using System.Collections.Generic;
using Eto.Collections;
using System.Linq;

namespace Eto.Drawing
{
	public class Palette : BaseList<Color>, ICloneable
	{
		public static readonly int[] EGAColors = new int[] {0,1,2,3,4,5,20,7,56,57,58,59,60,61,62,63};
		List<uint> argb;

		public Palette ()
		{
			argb = new List<uint> ();
		}
		
		public Palette (int capacity)
			: base(capacity)
		{
			argb = new List<uint> (capacity);
		}

		public static Palette GetEgaPalette ()
		{
			var mid = 168f / 255f;
			var low = 84f / 255f;
			var high = 252f / 255f;
			Palette pal = new Palette (64);
			pal.Add (new Color (0, 0, 0));
			pal.Add (new Color (0, 0, mid));
			pal.Add (new Color (0, mid, 0));
			pal.Add (new Color (0, mid, mid));
			pal.Add (new Color (mid, 0, 0));
			pal.Add (new Color (mid, 0, mid));
			pal.Add (new Color (mid, mid, 0));
			pal.Add (new Color (mid, mid, mid));
			pal.Add (new Color (0, 0, low));
			pal.Add (new Color (0, 0, high));
			pal.Add (new Color (0, mid, low));
			pal.Add (new Color (0, mid, high));
			pal.Add (new Color (mid, 0, low));
			pal.Add (new Color (mid, 0, high));
			pal.Add (new Color (mid, mid, low));
			pal.Add (new Color (mid, mid, high));
			pal.Add (new Color (0, low, 0));
			pal.Add (new Color (0, low, mid));
			pal.Add (new Color (0, high, 0));
			pal.Add (new Color (0, high, mid));
			pal.Add (new Color (mid, low, 0));
			pal.Add (new Color (mid, low, mid));
			pal.Add (new Color (mid, high, 0));
			pal.Add (new Color (mid, high, mid));
			pal.Add (new Color (0, low, low));
			pal.Add (new Color (0, low, high));
			pal.Add (new Color (0, high, low));
			pal.Add (new Color (0, high, high));
			pal.Add (new Color (mid, low, low));
			pal.Add (new Color (mid, low, high));
			pal.Add (new Color (mid, high, low));
			pal.Add (new Color (mid, high, high));
			pal.Add (new Color (low, 0, 0));
			pal.Add (new Color (low, 0, mid));
			pal.Add (new Color (low, mid, 0));
			pal.Add (new Color (low, mid, mid));
			pal.Add (new Color (high, 0, 0));
			pal.Add (new Color (high, 0, mid));
			pal.Add (new Color (high, mid, 0));
			pal.Add (new Color (high, mid, mid));
			pal.Add (new Color (low, 0, low));
			pal.Add (new Color (low, 0, high));
			pal.Add (new Color (low, mid, low));
			pal.Add (new Color (low, mid, high));
			pal.Add (new Color (high, 0, low));
			pal.Add (new Color (high, 0, high));
			pal.Add (new Color (high, mid, low));
			pal.Add (new Color (high, mid, high));
			pal.Add (new Color (low, low, 0));
			pal.Add (new Color (low, low, mid));
			pal.Add (new Color (low, high, 0));
			pal.Add (new Color (low, high, mid));
			pal.Add (new Color (high, low, 0));
			pal.Add (new Color (high, low, mid));
			pal.Add (new Color (high, high, 0));
			pal.Add (new Color (high, high, mid));
			pal.Add (new Color (low, low, low));
			pal.Add (new Color (low, low, high));
			pal.Add (new Color (low, high, low));
			pal.Add (new Color (low, high, high));
			pal.Add (new Color (high, low, low));
			pal.Add (new Color (high, low, high));
			pal.Add (new Color (high, high, low));
			pal.Add (new Color (high, high, high));
			return pal;
		}

		public static Palette GetDosPalette ()
		{
			var mid = 171f / 255f;
			var low = 87f / 255f;
			var high = 1f;

			Palette pal = new Palette (16);
			pal.Add (new Color (0, 0, 0));
			pal.Add (new Color (0, 0, mid));
			pal.Add (new Color (0, mid, 0));
			pal.Add (new Color (0, mid, mid));
			pal.Add (new Color (mid, 0, 0));
			pal.Add (new Color (mid, 0, mid));
			pal.Add (new Color (mid, low, 0));
			pal.Add (new Color (mid, mid, mid));
			pal.Add (new Color (low, low, low));
			pal.Add (new Color (low, low, high));
			pal.Add (new Color (low, high, low));
			pal.Add (new Color (low, high, high));
			pal.Add (new Color (high, low, low));
			pal.Add (new Color (high, low, high));
			pal.Add (new Color (high, high, low));
			pal.Add (new Color (high, high, high));
			return pal;
		}

		public static Palette FromEGA (Palette palEGA)
		{
			if (palEGA.Count != 64)
				throw new Exception ("source palette is not an EGA palette");
			Palette output = new Palette (EGAColors.Length);
			for (int i=0; i<EGAColors.Length; i++) {
				output [i] = palEGA [EGAColors [i]];
			}
			return output;
		}

		public void Save (BinaryWriter bw, int shift = 0)
		{
			for (int i=0; i<this.Count; i++) {
				var c = this [i];
				bw.Write ((byte)((int)(c.R * 255) >> shift));
				bw.Write ((byte)((int)(c.G * 255) >> shift));
				bw.Write ((byte)((int)(c.B * 255) >> shift));
			}
		}

		public void Load (BinaryReader br, int size, int shift = 0)
		{
			Clear ();
			this.Capacity = size;
			for (int i=0; i<size; i++) {
				int red = (br.ReadByte () << shift) & 0xff;
				int green = (br.ReadByte () << shift) & 0xff;
				int blue = (br.ReadByte () << shift) & 0xff;
				Add (new Color (red, green, blue));
			}
			OnChanged (EventArgs.Empty);
		}

		public static UInt32 GenerateRGBColor (Color c)
		{
			return (UInt32)(((uint)(c.A * 255) << 24) + ((uint)(c.R * 255) << 16) + ((uint)(c.G * 255) << 8) + (uint)(c.B * 255));
		}

		public uint GetRGBColor (int index)
		{
			return argb [index];
		}
		
		public override Color this [int index] {
			get {
				return base [index];
			}
			set {
				base [index] = value;
				argb [index] = value.ToArgb ();
			}
		}
		
		public override void Add (Color item)
		{
			argb.Add (item.ToArgb ());
			base.Add (item);
		}
		
		public override void AddRange (IEnumerable<Color> collection)
		{
			argb.AddRange (collection.Select (r => r.ToArgb ()));
			base.AddRange (collection);
		}
		
		public override void Insert (int index, Color item)
		{
			argb.Insert (index, item.ToArgb ());
			base.Insert (index, item);
		}
		
		public override bool Remove (Color item)
		{
			bool ret = base.Remove (item);
			ret &= argb.Remove (item.ToArgb ());
			return ret;
		}
		
		public override void RemoveAt (int index)
		{
			base.RemoveAt (index);
			argb.RemoveAt (index);
		}
		
		public override void Clear ()
		{
			base.Clear ();
			argb.Clear ();
		}
		
		public int FindClosest (Color color)
		{
			int closestIndex = 0;
			var colorHsl = new ColorHSL(color);
			var closestDifference = ColorHSL.Distance (colorHsl, new ColorHSL (this [0]));
			for (int i = 1; i < this.Count; i++) {
				var curDifference = ColorHSL.Distance (colorHsl, new ColorHSL (this [i]));
				if (curDifference < closestDifference) {
					closestIndex = i;
					closestDifference = curDifference;
				}
				if (curDifference == 0) 
					break;
			}
			return closestIndex;
		}
		
		public int FindAddColour (Color colour)
		{
			var index = argb.IndexOf (colour.ToArgb ());
			if (index != -1)
				return index;
			
			this.Add (colour);
			return this.Count - 1;
		}
		
		public override int GetHashCode ()
		{
			int code = 0;
			for (int i = 0; i < argb.Count; i++) {
				code ^= argb [i].GetHashCode ();
			}
			return code;
		}
		
		public override bool Equals (object obj)
		{
			var p = obj as Palette;
			if (obj == null)
				return false;
			if (p.Count != this.Count)
				return false;
			for (int i = 0; i < p.Count; i++) {
				if (p.argb [i] != this.argb [i])
					return false;
			}
			return true;
		}

		public Palette Clone ()
		{
			var p = new Palette ();
			p.AddRange (this);
			return p;
		}
		
		#region ICloneable implementation
		
		object ICloneable.Clone ()
		{
			return Clone ();
		}
		
		#endregion
		
		
	}
}
