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

		public Palette()
		{
			argb = new List<uint>();
		}
		
		public Palette(int capacity)
			: base(capacity)
		{
			argb = new List<uint>(capacity);
		}

		public static Palette GetEgaPalette()
		{
			Palette pal = new Palette(64);
			pal.Add (Color.FromArgb(0, 0, 0));
			pal.Add (Color.FromArgb(0, 0, 168));
			pal.Add (Color.FromArgb(0, 168, 0));
			pal.Add (Color.FromArgb(0, 168, 168));
			pal.Add (Color.FromArgb(168, 0, 0));
			pal.Add (Color.FromArgb(168, 0, 168));
			pal.Add (Color.FromArgb(168, 168, 0));
			pal.Add (Color.FromArgb(168, 168, 168));
			pal.Add (Color.FromArgb(0, 0, 84));
			pal.Add (Color.FromArgb(0, 0, 252));
			pal.Add (Color.FromArgb(0, 168, 84));
			pal.Add (Color.FromArgb(0, 168, 252));
			pal.Add (Color.FromArgb(168, 0, 84));
			pal.Add (Color.FromArgb(168, 0, 252));
			pal.Add (Color.FromArgb(168, 168, 84));
			pal.Add (Color.FromArgb(168, 168, 252));
			pal.Add (Color.FromArgb(0, 84, 0));
			pal.Add (Color.FromArgb(0, 84, 168));
			pal.Add (Color.FromArgb(0, 252, 0));
			pal.Add (Color.FromArgb(0, 252, 168));
			pal.Add (Color.FromArgb(168, 84, 0));
			pal.Add (Color.FromArgb(168, 84, 168));
			pal.Add (Color.FromArgb(168, 252, 0));
			pal.Add (Color.FromArgb(168, 252, 168));
			pal.Add (Color.FromArgb(0, 84, 84));
			pal.Add (Color.FromArgb(0, 84, 252));
			pal.Add (Color.FromArgb(0, 252, 84));
			pal.Add (Color.FromArgb(0, 252, 252));
			pal.Add (Color.FromArgb(168, 84, 84));
			pal.Add (Color.FromArgb(168, 84, 252));
			pal.Add (Color.FromArgb(168, 252, 84));
			pal.Add (Color.FromArgb(168, 252, 252));
			pal.Add (Color.FromArgb(84, 0, 0));
			pal.Add (Color.FromArgb(84, 0, 168));
			pal.Add (Color.FromArgb(84, 168, 0));
			pal.Add (Color.FromArgb(84, 168, 168));
			pal.Add (Color.FromArgb(252, 0, 0));
			pal.Add (Color.FromArgb(252, 0, 168));
			pal.Add (Color.FromArgb(252, 168, 0));
			pal.Add (Color.FromArgb(252, 168, 168));
			pal.Add (Color.FromArgb(84, 0, 84));
			pal.Add (Color.FromArgb(84, 0, 252));
			pal.Add (Color.FromArgb(84, 168, 84));
			pal.Add (Color.FromArgb(84, 168, 252));
			pal.Add (Color.FromArgb(252, 0, 84));
			pal.Add (Color.FromArgb(252, 0, 252));
			pal.Add (Color.FromArgb(252, 168, 84));
			pal.Add (Color.FromArgb(252, 168, 252));
			pal.Add (Color.FromArgb(84, 84, 0));
			pal.Add (Color.FromArgb(84, 84, 168));
			pal.Add (Color.FromArgb(84, 252, 0));
			pal.Add (Color.FromArgb(84, 252, 168));
			pal.Add (Color.FromArgb(252, 84, 0));
			pal.Add (Color.FromArgb(252, 84, 168));
			pal.Add (Color.FromArgb(252, 252, 0));
			pal.Add (Color.FromArgb(252, 252, 168));
			pal.Add (Color.FromArgb(84, 84, 84));
			pal.Add (Color.FromArgb(84, 84, 252));
			pal.Add (Color.FromArgb(84, 252, 84));
			pal.Add (Color.FromArgb(84, 252, 252));
			pal.Add (Color.FromArgb(252, 84, 84));
			pal.Add (Color.FromArgb(252, 84, 252));
			pal.Add (Color.FromArgb(252, 252, 84));
			pal.Add (Color.FromArgb(252, 252, 252));
			return pal;
		}

		public static Palette GetDefaultPalette()
		{
			Palette pal = new Palette(16);
			pal.Add (Color.FromArgb(0,		0,		0));
			pal.Add (Color.FromArgb(0,		0,		171));
			pal.Add (Color.FromArgb(0,		171,	0));
			pal.Add (Color.FromArgb(0,		171,	171));
			pal.Add (Color.FromArgb(171,	0,		0));
			pal.Add (Color.FromArgb(171,	0,		171));
			pal.Add (Color.FromArgb(171,	87,		0));
			pal.Add (Color.FromArgb(171,	171,	171));
			pal.Add (Color.FromArgb(87,		87,		87));
			pal.Add (Color.FromArgb(87,		87,		255));
			pal.Add (Color.FromArgb(87,		255,	87));
			pal.Add (Color.FromArgb(87,		255,	255));
			pal.Add (Color.FromArgb(255,	87,		87));
			pal.Add (Color.FromArgb(255,	87,		255));
			pal.Add (Color.FromArgb(255,	255,	87));
			pal.Add (Color.FromArgb(255,	255,	255));
			return pal;
		}

		public static Palette FromEGA(Palette palEGA)
		{
			if (palEGA.Count != 64) throw new Exception("source palette is not an EGA palette");
			Palette output = new Palette(EGAColors.Length);
			for (int i=0; i<EGAColors.Length; i++)
			{
				output[i] = palEGA[EGAColors[i]];
			}
			return output;
		}

		public void Save(BinaryWriter bw, int shift = 2)
		{
			for (int i=0; i<this.Count; i++)
			{
				var c = this[i];
				bw.Write ((byte)(c.R >> shift));
				bw.Write ((byte)(c.G >> shift));
				bw.Write ((byte)(c.B >> shift));
			}
		}

		public void Load(BinaryReader br, int size)
		{
			Load(br, size, 2);
		
		}
		
		public void Load(BinaryReader br, int size, int shift)
		{
			Clear ();
			this.Capacity = size;
			for (int i=0; i<size; i++)
			{
				int red = (br.ReadByte() << shift) & 0xff;
				int green = (br.ReadByte() << shift) & 0xff;
				int blue = (br.ReadByte() << shift) & 0xff;
				var c = Color.FromArgb(red, green, blue);
				InnerList.Add (c);
				argb.Add (c.ToArgb());
			}
			OnChanged(EventArgs.Empty);
		}

		public static UInt32 GenerateRGBColor(Color c)
		{
			return (UInt32)((c.A << 24) + (c.R << 16) + (c.G << 8) + c.B);
		}

		public uint GetRGBColor(int index)
		{
			return argb[index];
		}
		
		public override Color this[int index] {
			get {
				return base[index];
			}
			set {
				base[index] = value;
				argb[index] = value.ToArgb();
			}
		}
		
		public override void Add (Color item)
		{
			argb.Add (item.ToArgb ());
			base.Add (item);
		}
		
		public override void AddRange (IEnumerable<Color> collection)
		{
			argb.AddRange (collection.Select(r => r.ToArgb ()));
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
		
		public int FindAddColour(Color colour)
		{
			var index = argb.IndexOf (colour.ToArgb ());
			if (index != -1) return index;
			
			this.Add (colour);
			return this.Count - 1;
		}
		
		public override int GetHashCode ()
		{
			int code = 0;
			for (int i = 0; i < argb.Count; i++) {
				code ^= argb[i].GetHashCode();
			}
			return code;
		}
		
		public override bool Equals (object obj)
		{
			var p = obj as Palette;
			if (obj == null) return false;
			if (p.Count != this.Count) return false;
			for (int i = 0; i < p.Count; i++) {
				if (p.argb[i] != this.argb[i]) return false;
			}
			return true;
		}

		public Palette Clone ()
		{
			var p = new Palette();
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
