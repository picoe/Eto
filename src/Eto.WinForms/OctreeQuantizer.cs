/* 
  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
  ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
  THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
  PARTICULAR PURPOSE. 
  
    This is sample code and is freely distributable. 
*/ 
namespace ImageManipulation
{
	/// <summary>
	/// Quantize using an Octree
	/// </summary>
	public unsafe class OctreeQuantizer : Quantizer
	{
		/// <summary>
		/// Construct the octree quantizer
		/// </summary>
		/// <remarks>
		/// The Octree quantizer is a two pass algorithm. The initial pass sets up the octree,
		/// the second pass quantizes a color based on the nodes in the tree
		/// </remarks>
		/// <param name="maxColors">The maximum number of colors to return</param>
		/// <param name="maxColorBits">The number of significant bits</param>
		public OctreeQuantizer(int maxColors, int maxColorBits) : base(false)
		{
			if (maxColors > 255)
				throw new ArgumentOutOfRangeException("maxColors", maxColors, "The number of colors should be less than 256");

			if ((maxColorBits < 1) | (maxColorBits > 8))
				throw new ArgumentOutOfRangeException("maxColorBits", maxColorBits, "This should be between 1 and 8");

			// Construct the octree
			_octree = new Octree(maxColorBits);

			_maxColors = maxColors;
		}

		/// <summary>
		/// Process the pixel in the first pass of the algorithm
		/// </summary>
		/// <param name="pixel">The pixel to quantize</param>
		/// <remarks>
		/// This function need only be overridden if your quantize algorithm needs two passes,
		/// such as an Octree quantizer.
		/// </remarks>
		protected override void InitialQuantizePixel(Color32* pixel)
		{
			// Add the color to the octree
			_octree.AddColor(pixel);
		}

		/// <summary>
		/// Override this to process the pixel in the second pass of the algorithm
		/// </summary>
		/// <param name="pixel">The pixel to quantize</param>
		/// <returns>The quantized value</returns>
		protected override byte QuantizePixel(Color32* pixel)
		{
			byte	paletteIndex = (byte)_maxColors;	// The color at [_maxColors] is set to transparent

			// Get the palette index if this non-transparent
			if (pixel->Alpha > 0)
				paletteIndex = (byte)_octree.GetPaletteIndex(pixel);

			return paletteIndex;
		}

		/// <summary>
		/// Retrieve the palette for the quantized image
		/// </summary>
		/// <param name="original">Any old palette, this is overrwritten</param>
		/// <returns>The new color palette</returns>
		protected override sdi.ColorPalette GetPalette(sdi.ColorPalette original)
		{
			// First off convert the octree to _maxColors colors
			ArrayList	palette = _octree.Palletize(_maxColors - 1);

			// Then convert the palette based on those colors
			for (int index = 0; index < palette.Count; index++)
				original.Entries[index] = (sd.Color)palette[index];

			// Add the transparent color
			original.Entries[_maxColors] = sd.Color.FromArgb(0, 0, 0, 0);

			return original;
		}

		/// <summary>
		/// Stores the tree
		/// </summary>
		readonly Octree _octree;
		/// <summary>
		/// Maximum allowed color depth
		/// </summary>
		readonly int _maxColors;

		/// <summary>
		/// Class which does the actual quantization
		/// </summary>
		class Octree
		{
			/// <summary>
			/// Construct the octree
			/// </summary>
			/// <param name="maxColorBits">The maximum number of significant bits in the image</param>
			public Octree(int maxColorBits)
			{
				_maxColorBits = maxColorBits;
				_leafCount = 0;
				_reducibleNodes = new OctreeNode[9];
				_root = new OctreeNode(0, _maxColorBits, this); 
				_previousColor = 0;
				_previousNode = null;
			}

			/// <summary>
			/// Add a given color value to the octree
			/// </summary>
			/// <param name="pixel"></param>
			public void AddColor(Color32* pixel)
			{
				// Check if this request is for the same color as the last
				if (_previousColor == pixel->ARGB)
				{
					// If so, check if I have a previous node setup. This will only ocurr if the first color in the image
					// happens to be black, with an alpha component of zero.
					if (null == _previousNode)
					{
						_previousColor = pixel->ARGB;
						_root.AddColor(pixel, _maxColorBits, 0, this);
					}
					else
						// Just update the previous node
						_previousNode.Increment(pixel);
				}
				else
				{
					_previousColor = pixel->ARGB;
					_root.AddColor(pixel, _maxColorBits, 0, this);
				}
			}

			/// <summary>
			/// Reduce the depth of the tree
			/// </summary>
			public void Reduce()
			{
				int	index;

				// Find the deepest level containing at least one reducible node
				for (index = _maxColorBits - 1; (index > 0) && (null == _reducibleNodes[index]); index--)
				{
				}

				// Reduce the node most recently added to the list at level 'index'
				OctreeNode	node = _reducibleNodes[index];
				_reducibleNodes[index] = node.NextReducible;

				// Decrement the leaf count after reducing the node
				_leafCount -= node.Reduce();

				// And just in case I've reduced the last color to be added, and the next color to
				// be added is the same, invalidate the previousNode...
				_previousNode = null;
			}

			/// <summary>
			/// Get/Set the number of leaves in the tree
			/// </summary>
			public int Leaves
			{
				get { return _leafCount; }
				set { _leafCount = value; }
			}

			/// <summary>
			/// Return the array of reducible nodes
			/// </summary>
			protected OctreeNode[] ReducibleNodes
			{
				get { return _reducibleNodes; }
			}

			/// <summary>
			/// Keep track of the previous node that was quantized
			/// </summary>
			/// <param name="node">The node last quantized</param>
			protected void TrackPrevious(OctreeNode node)
			{
				_previousNode = node;
			}

			/// <summary>
			/// Convert the nodes in the octree to a palette with a maximum of colorCount colors
			/// </summary>
			/// <param name="colorCount">The maximum number of colors</param>
			/// <returns>An arraylist with the palettized colors</returns>
			public ArrayList Palletize(int colorCount)
			{
				while (Leaves > colorCount)
					Reduce();

				// Now palettize the nodes
				var	palette = new ArrayList(Leaves);
				int paletteIndex = 0;
				_root.ConstructPalette(palette, ref paletteIndex);

				// And return the palette
				return palette;
			}

			/// <summary>
			/// Get the palette index for the passed color
			/// </summary>
			/// <param name="pixel"></param>
			/// <returns></returns>
			public int GetPaletteIndex(Color32* pixel)
			{
				return _root.GetPaletteIndex(pixel, 0);
			}

			/// <summary>
			/// Mask used when getting the appropriate pixels for a given node
			/// </summary>
			static readonly int[] mask = { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };
			/// <summary>
			/// The root of the octree
			/// </summary>
			readonly OctreeNode _root;
			/// <summary>
			/// Number of leaves in the tree
			/// </summary>
			int _leafCount;
			/// <summary>
			/// Array of reducible nodes
			/// </summary>
			readonly OctreeNode[]	_reducibleNodes;
			/// <summary>
			/// Maximum number of significant bits in the image
			/// </summary>
			readonly int _maxColorBits;
			/// <summary>
			/// Store the last node quantized
			/// </summary>
			OctreeNode _previousNode;
			/// <summary>
			/// Cache the previous color quantized
			/// </summary>
			int _previousColor;

			/// <summary>
			/// Class which encapsulates each node in the tree
			/// </summary>
			protected class OctreeNode
			{
				/// <summary>
				/// Construct the node
				/// </summary>
				/// <param name="level">The level in the tree = 0 - 7</param>
				/// <param name="colorBits">The number of significant color bits in the image</param>
				/// <param name="octree">The tree to which this node belongs</param>
				public OctreeNode(int level, int colorBits, Octree octree)
				{
					// Construct the new node
					_leaf = (level == colorBits);

					_red = _green = _blue = 0;
					_pixelCount = 0;

					// If a leaf, increment the leaf count
					if (_leaf)
					{
						octree.Leaves++;
						_nextReducible = null;
						_children = null; 
					}
					else
					{
						// Otherwise add this to the reducible nodes
						_nextReducible = octree.ReducibleNodes[level];
						octree.ReducibleNodes[level] = this;
						_children = new OctreeNode[8];
					}
				}

				/// <summary>
				/// Add a color into the tree
				/// </summary>
				/// <param name="pixel">The color</param>
				/// <param name="colorBits">The number of significant color bits</param>
				/// <param name="level">The level in the tree</param>
				/// <param name="octree">The tree to which this node belongs</param>
				public void AddColor(Color32* pixel, int colorBits, int level, Octree octree)
				{
					// Update the color information if this is a leaf
					if (_leaf)
					{
						Increment(pixel);
						// Setup the previous node
						octree.TrackPrevious(this);
					}
					else
					{
						// Go to the next level down in the tree
						int	shift = 7 - level;
						int index = ((pixel->Red & mask[level]) >> (shift - 2)) |
						            ((pixel->Green & mask[level]) >> (shift - 1)) |
						            ((pixel->Blue & mask[level]) >> (shift));

						OctreeNode	child = _children[index];

						if (null == child)
						{
							// Create a new child node & store in the array
							child = new OctreeNode(level + 1, colorBits, octree); 
							_children[index] = child;
						}

						// Add the color to the child node
						child.AddColor(pixel, colorBits, level + 1, octree);
					}

				}

				/// <summary>
				/// Get/Set the next reducible node
				/// </summary>
				public OctreeNode NextReducible
				{
					get { return _nextReducible; }
					set { _nextReducible = value; }
				}

				/// <summary>
				/// Return the child nodes
				/// </summary>
				public OctreeNode[] Children
				{
					get { return _children; }
				}

				/// <summary>
				/// Reduce this node by removing all of its children
				/// </summary>
				/// <returns>The number of leaves removed</returns>
				public int Reduce()
				{
					_red = _green = _blue = 0;
					int	children = 0;

					// Loop through all children and add their information to this node
					for (int index = 0; index < 8; index++)
					{
						if (null != _children[index])
						{
							_red += _children[index]._red;
							_green += _children[index]._green;
							_blue += _children[index]._blue;
							_pixelCount += _children[index]._pixelCount;
							++children;
							_children[index] = null;
						}
					}

					// Now change this to a leaf node
					_leaf = true;

					// Return the number of nodes to decrement the leaf count by
					return (children - 1);
				}

				/// <summary>
				/// Traverse the tree, building up the color palette
				/// </summary>
				/// <param name="palette">The palette</param>
				/// <param name="paletteIndex">The current palette index</param>
				public void ConstructPalette(ArrayList palette, ref int paletteIndex)
				{
					if (_leaf)
					{
						// Consume the next palette index
						_paletteIndex = paletteIndex++;

						// And set the color of the palette entry
						palette.Add(sd.Color.FromArgb(_red / _pixelCount, _green / _pixelCount, _blue / _pixelCount));
					}
					else
					{
						// Loop through children looking for leaves
						for (int index = 0; index < 8; index++)
						{
							if (null != _children[index])
								_children[index].ConstructPalette(palette, ref paletteIndex);
						}
					}
				}

				/// <summary>
				/// Return the palette index for the passed color
				/// </summary>
				public int GetPaletteIndex(Color32* pixel, int level)
				{
					int	paletteIndex = _paletteIndex;

					if (!_leaf)
					{
						int	shift = 7 - level;
						int index = ((pixel->Red & mask[level]) >> (shift - 2)) |
						            ((pixel->Green & mask[level]) >> (shift - 1)) |
						            ((pixel->Blue & mask[level]) >> (shift));

						if (null != _children[index])
							paletteIndex = _children[index].GetPaletteIndex(pixel, level + 1);
						else
							throw new ArgumentNullException(string.Format(CultureInfo.CurrentCulture, "Didn't expect this!"));
					}

					return paletteIndex;
				}

				/// <summary>
				/// Increment the pixel count and add to the color information
				/// </summary>
				public void Increment(Color32* pixel)
				{
					_pixelCount++;
					_red += pixel->Red;
					_green += pixel->Green;
					_blue += pixel->Blue;
				}

				/// <summary>
				/// Flag indicating that this is a leaf node
				/// </summary>
				bool _leaf;
				/// <summary>
				/// Number of pixels in this node
				/// </summary>
				int _pixelCount;
				/// <summary>
				/// Red component
				/// </summary>
				int _red;
				/// <summary>
				/// Green Component
				/// </summary>
				int _green;
				/// <summary>
				/// Blue component
				/// </summary>
				int _blue;
				/// <summary>
				/// Pointers to any child nodes
				/// </summary>
				readonly OctreeNode[]	_children;
				/// <summary>
				/// Pointer to next reducible node
				/// </summary>
				OctreeNode _nextReducible;
				/// <summary>
				/// The index of this node in the palette
				/// </summary>
				int _paletteIndex;
			}
		}
	}
}
