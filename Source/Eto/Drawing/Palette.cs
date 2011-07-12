using System;
using System.IO;
using System.Collections.Generic;

namespace Eto.Drawing
{
	public class Palette
	{
		public static readonly int[] EGAColors = new int[] {0,1,2,3,4,5,20,7,56,57,58,59,60,61,62,63};

		List<Color> paletteList;
		List<uint> argb;

		public event EventHandler<EventArgs> Changed;
		
		protected virtual void OnChanged(EventArgs e)
		{
			if (Changed != null) Changed(this, EventArgs.Empty);
		}

		public Palette()
			: this(16)
		{
		}
		public Palette(int size)
		{
			paletteList = new List<Color>(size);
			argb = new List<uint>(size);
			var col = Color.Black;
			var argbcol = col.ToArgb ();
			for (int i=0; i<size; i++)
			{
				paletteList.Add (col);
				argb.Add (argbcol);
			}
		}

		public static Palette GetEgaPalette()
		{
			Palette pal = new Palette(64);
			pal[0] = Color.FromArgb(0, 0, 0);
			pal[1] = Color.FromArgb(0, 0, 168);
			pal[2] = Color.FromArgb(0, 168, 0);
			pal[3] = Color.FromArgb(0, 168, 168);
			pal[4] = Color.FromArgb(168, 0, 0);
			pal[5] = Color.FromArgb(168, 0, 168);
			pal[6] = Color.FromArgb(168, 168, 0);
			pal[7] = Color.FromArgb(168, 168, 168);
			pal[8] = Color.FromArgb(0, 0, 84);
			pal[9] = Color.FromArgb(0, 0, 252);
			pal[10] = Color.FromArgb(0, 168, 84);
			pal[11] = Color.FromArgb(0, 168, 252);
			pal[12] = Color.FromArgb(168, 0, 84);
			pal[13] = Color.FromArgb(168, 0, 252);
			pal[14] = Color.FromArgb(168, 168, 84);
			pal[15] = Color.FromArgb(168, 168, 252);
			pal[16] = Color.FromArgb(0, 84, 0);
			pal[17] = Color.FromArgb(0, 84, 168);
			pal[18] = Color.FromArgb(0, 252, 0);
			pal[19] = Color.FromArgb(0, 252, 168);
			pal[20] = Color.FromArgb(168, 84, 0);
			pal[21] = Color.FromArgb(168, 84, 168);
			pal[22] = Color.FromArgb(168, 252, 0);
			pal[23] = Color.FromArgb(168, 252, 168);
			pal[24] = Color.FromArgb(0, 84, 84);
			pal[25] = Color.FromArgb(0, 84, 252);
			pal[26] = Color.FromArgb(0, 252, 84);
			pal[27] = Color.FromArgb(0, 252, 252);
			pal[28] = Color.FromArgb(168, 84, 84);
			pal[29] = Color.FromArgb(168, 84, 252);
			pal[30] = Color.FromArgb(168, 252, 84);
			pal[31] = Color.FromArgb(168, 252, 252);
			pal[32] = Color.FromArgb(84, 0, 0);
			pal[33] = Color.FromArgb(84, 0, 168);
			pal[34] = Color.FromArgb(84, 168, 0);
			pal[35] = Color.FromArgb(84, 168, 168);
			pal[36] = Color.FromArgb(252, 0, 0);
			pal[37] = Color.FromArgb(252, 0, 168);
			pal[38] = Color.FromArgb(252, 168, 0);
			pal[39] = Color.FromArgb(252, 168, 168);
			pal[40] = Color.FromArgb(84, 0, 84);
			pal[41] = Color.FromArgb(84, 0, 252);
			pal[42] = Color.FromArgb(84, 168, 84);
			pal[43] = Color.FromArgb(84, 168, 252);
			pal[44] = Color.FromArgb(252, 0, 84);
			pal[45] = Color.FromArgb(252, 0, 252);
			pal[46] = Color.FromArgb(252, 168, 84);
			pal[47] = Color.FromArgb(252, 168, 252);
			pal[48] = Color.FromArgb(84, 84, 0);
			pal[49] = Color.FromArgb(84, 84, 168);
			pal[50] = Color.FromArgb(84, 252, 0);
			pal[51] = Color.FromArgb(84, 252, 168);
			pal[52] = Color.FromArgb(252, 84, 0);
			pal[53] = Color.FromArgb(252, 84, 168);
			pal[54] = Color.FromArgb(252, 252, 0);
			pal[55] = Color.FromArgb(252, 252, 168);
			pal[56] = Color.FromArgb(84, 84, 84);
			pal[57] = Color.FromArgb(84, 84, 252);
			pal[58] = Color.FromArgb(84, 252, 84);
			pal[59] = Color.FromArgb(84, 252, 252);
			pal[60] = Color.FromArgb(252, 84, 84);
			pal[61] = Color.FromArgb(252, 84, 252);
			pal[62] = Color.FromArgb(252, 252, 84);
			pal[63] = Color.FromArgb(252, 252, 252);
			return pal;
		}

		public static Palette GetDefaultPalette()
		{
			Palette pal = new Palette(16);
			pal[0] = Color.FromArgb(0,		0,		0);
			pal[1] = Color.FromArgb(0,		0,		171);
			pal[2] = Color.FromArgb(0,		171,	0);
			pal[3] = Color.FromArgb(0,		171,	171);
			pal[4] = Color.FromArgb(171,	0,		0);
			pal[5] = Color.FromArgb(171,	0,		171);
			pal[6] = Color.FromArgb(171,	87,		0);
			pal[7] = Color.FromArgb(171,	171,	171);
			pal[8] = Color.FromArgb(87,		87,		87);
			pal[9] = Color.FromArgb(87,		87,		255);
			pal[10] = Color.FromArgb(87,	255,	87);
			pal[11] = Color.FromArgb(87,	255,	255);
			pal[12] = Color.FromArgb(255,	87,		87);
			pal[13] = Color.FromArgb(255,	87,		255);
			pal[14] = Color.FromArgb(255,	255,	87);
			pal[15] = Color.FromArgb(255,	255,	255);
			return pal;
		}

		public static Palette FromEGA(Palette palEGA)
		{
			if (palEGA.Size != 64) throw new Exception("source palette is not an EGA palette");
			Palette output = new Palette(EGAColors.Length);
			for (int i=0; i<EGAColors.Length; i++)
			{
				output[i] = palEGA[EGAColors[i]];
			}
			return output;
		}

		public void Save(BinaryWriter bw, int shift = 2)
		{
			for (int i=0; i<this.Size; i++)
			{
				var c = paletteList[i];
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
			paletteList = new List<Color>(size);
			argb = new List<uint>(size);
			for (int i=0; i<size; i++)
			{
				int red = (br.ReadByte() << shift) & 0xff;
				int green = (br.ReadByte() << shift) & 0xff;
				int blue = (br.ReadByte() << shift) & 0xff;
				Color c = Color.FromArgb(red, green, blue);
				paletteList.Add (c);
				argb.Add (c.ToArgb());
			}
			if (Changed != null) Changed(this, EventArgs.Empty);
		}

		public int Size
		{
			get { return paletteList.Count; }
		}

		public static UInt32 GenerateRGBColor(Color c)
		{
			return (UInt32)((c.A << 24) + (c.R << 16) + (c.G << 8) + c.B);
		}

		public int Find(uint argb)
		{
			for (int i=0; i<Size; i++)
			{
				if (this.argb[i] == argb) return i;
			}
			return -1;
		}

		public uint GetRGBColor(int index)
		{
			return argb[index];
		}

		public Color this[int index]
		{
			get { return paletteList[index]; }
			set 
			{
				paletteList[index] = value;
				argb[index] = value.ToArgb();
				OnChanged(EventArgs.Empty);
			}
		}
		
		public int FindAddColour(Color colour)
		{
			var argbcol = colour.ToArgb();
			for (int i=0; i<this.Size; i++) {
				if (argb[i] == argbcol) return i;
			}
			
			paletteList.Add (colour);
			argb.Add (argbcol);
			OnChanged(EventArgs.Empty);
			return paletteList.Count - 1;
		}
		
		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
		
		public override bool Equals (object obj)
		{
			var p = obj as Palette;
			if (obj == null) return false;
			if (p.Size != this.Size) return false;
			for (int i = 0; i < p.Size; i++) {
				if (p[i] != this[i]) return false;
			}
			return true;
		}
	}
}
