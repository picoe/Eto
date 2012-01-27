//-----------------------------------------------------------------------
// <copyright file="QuadTree.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
#if DEBUG_DUMP
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Xml;
#endif

namespace Microsoft.Sample.Controls

{

    /// <summary>
    /// This class efficiently stores and retrieves arbitrarily sized and positioned
    /// objects in a quad-tree data structure.  This can be used to do efficient hit
    /// detection or visiblility checks on objects in a virtualized canvas.
    /// The object does not need to implement any special interface because the Rect Bounds
    /// of those objects is handled as a separate argument to Insert.
    /// </summary>
    public class QuadTree<T> where T : class
    {
        Rect _bounds; // overall bounds we are indexing.
        Quadrant _root;
        IDictionary<T, Quadrant> _table;

        /// <summary>
        /// Each node stored in the tree has a position, width & height.
        /// </summary>
        internal class QuadNode
        {
            Rect _bounds;
            QuadNode _next; // linked in a circular list.
            T _node; // the actual visual object being stored here.

            /// <summary>
            /// Construct new QuadNode to wrap the given node with given bounds
            /// </summary>
            /// <param name="node">The node</param>
            /// <param name="bounds">The bounds of that node</param>
            public QuadNode(T node, Rect bounds)
            {
                _node = node;
                _bounds = bounds;
            }

            /// <summary>
            /// The node
            /// </summary>
            public T Node
            {
                get { return _node; }
                set { _node = value; }
            }

            /// <summary>
            /// The Rect bounds of the node
            /// </summary>
            public Rect Bounds
            {
                get { return _bounds; }
            }

            /// <summary>
            /// QuadNodes form a linked list in the Quadrant.
            /// </summary>
            public QuadNode Next
            {
                get { return _next; }
                set { _next = value; }
            }
        }


        /// <summary>
        /// The canvas is split up into four Quadrants and objects are stored in the quadrant that contains them
        /// and each quadrant is split up into four child Quadrants recurrsively.  Objects that overlap more than
        /// one quadrant are stored in the _nodes list for this Quadrant.
        /// </summary>
        internal class Quadrant
        {
            Quadrant _parent;
            Rect _bounds; // quadrant bounds.

            QuadNode _nodes; // nodes that overlap the sub quadrant boundaries.

            // The quadrant is subdivided when nodes are inserted that are 
            // completely contained within those subdivisions.
            Quadrant _topLeft;
            Quadrant _topRight;
            Quadrant _bottomLeft;
            Quadrant _bottomRight;

#if DEBUG_DUMP
            public void ShowQuadTree(Canvas c)
            {
                Rectangle r = new Rectangle();
                r.Width = _bounds.Width;
                r.Height = _bounds.Height;
                Canvas.SetLeft(r, _bounds.Left);
                Canvas.SetTop(r, _bounds.Top);
                r.Stroke = Brushes.DarkRed;
                r.StrokeThickness = 1;
                r.StrokeDashArray = new DoubleCollection(new double[] { 2.0, 3.0 });
                c.Children.Add(r);

                if (_topLeft != null) _topLeft.ShowQuadTree(c);
                if (_topRight != null) _topRight.ShowQuadTree(c);
                if (_bottomLeft != null) _bottomLeft.ShowQuadTree(c);
                if (_bottomRight != null) _bottomRight.ShowQuadTree(c);
            }

            public void Dump(LogWriter w)
            {
                w.WriteAttribute("Bounds", _bounds.ToString());
                if (_nodes != null)
                {
                    QuadNode n = _nodes;
                    do
                    {
                        n = n.Next; // first node.
                        w.Open("node");
                        w.WriteAttribute("Bounds", n.Bounds.ToString());
                        w.Close();
                    } while (n != _nodes);
                }
                DumpQuadrant("TopLeft", _topLeft, w);
                DumpQuadrant("TopRight", _topRight, w);
                DumpQuadrant("BottomLeft", _bottomLeft, w);
                DumpQuadrant("BottomRight", _bottomRight, w);
            }

            public void DumpQuadrant(string label, Quadrant q, LogWriter w)
            {
                if (q != null)
                {
                    w.Open("Quadrant");
                    w.WriteAttribute("Name", label);
                    q.Dump(w);
                    w.Close();
                }
            }
#endif

            /// <summary>
            /// Construct new Quadrant with a given bounds all nodes stored inside this quadrant
            /// will fit inside this bounds.  
            /// </summary>
            /// <param name="parent">The parent quadrant (if any)</param>
            /// <param name="bounds">The bounds of this quadrant</param>
            public Quadrant(Quadrant parent, Rect bounds)
            {
                _parent = parent;
                Debug.Assert(bounds.Width != 0 && bounds.Height != 0);
                if (bounds.Width == 0 || bounds.Height == 0)                
                {
                    // todo: localize
                    throw new ArgumentException("Bounds of quadrant cannot be zero width or height");
                }
                _bounds = bounds;
            }

            /// <summary>
            /// The parent Quadrant or null if this is the root
            /// </summary>
            internal Quadrant Parent
            {
                get { return _parent; }
            }

            /// <summary>
            /// The bounds of this quadrant
            /// </summary>
            internal Rect Bounds 
            { 
                get { return _bounds; } 
            }

            /// <summary>
            /// Insert the given node
            /// </summary>
            /// <param name="node">The node </param>
            /// <param name="bounds">The bounds of that node</param>
            /// <returns></returns>
            internal Quadrant Insert(T node, Rect bounds)
            {
                Debug.Assert(bounds.Width != 0 && bounds.Height != 0);
                if (bounds.Width == 0 || bounds.Height == 0)
                {
                    // todo: localize
                    throw new ArgumentException("Bounds of quadrant cannot be zero width or height");
                }

                double w = _bounds.Width / 2;
                if (w == 0)
                {
                    w = 1;
                }
                double h = _bounds.Height / 2;
                if (h == 0)
                {
                    h = 1;
                }

                // assumption that the Rect struct is almost as fast as doing the operations
                // manually since Rect is a value type.

                Rect topLeft = new Rect(_bounds.Left, _bounds.Top, w, h);
                Rect topRight = new Rect(_bounds.Left + w, _bounds.Top, w, h);
                Rect bottomLeft = new Rect(_bounds.Left, _bounds.Top + h, w, h);
                Rect bottomRight = new Rect(_bounds.Left + w, _bounds.Top + h, w, h);

                Quadrant child = null;

                // See if any child quadrants completely contain this node.
                if (topLeft.Contains(bounds))
                {
                    if ( _topLeft == null)
                    {
                        _topLeft = new Quadrant(this, topLeft);
                    }
                    child = _topLeft;
                }
                else if (topRight.Contains(bounds))
                {
                    if ( _topRight == null)
                    {
                        _topRight = new Quadrant(this, topRight);
                    }
                    child = _topRight;
                }
                else if (bottomLeft.Contains(bounds))
                {
                    if ( _bottomLeft == null)
                    {
                        _bottomLeft = new Quadrant(this, bottomLeft);
                    }
                    child = _bottomLeft;
                }
                else if (bottomRight.Contains(bounds))
                {
                    if ( _bottomRight == null)
                    {
                        _bottomRight = new Quadrant(this, bottomRight);
                    }
                    child = _bottomRight;
                }

                if (child != null)
                {
                    return child.Insert(node, bounds);
                }
                else
                {
                    QuadNode n = new QuadNode(node, bounds);
                    if (_nodes == null)
                    {
                        n.Next = n;
                    }
                    else
                    {
                        // link up in circular link list.
                        QuadNode x = _nodes;
                        n.Next = x.Next;
                        x.Next = n;
                    }
                    _nodes = n;
                    return this;
                }
            }
            
            /// <summary>
            /// Returns all nodes in this quadrant that intersect the given bounds.
            /// The nodes are returned in pretty much random order as far as the caller is concerned.
            /// </summary>
            /// <param name="nodes">List of nodes found in the given bounds</param>
            /// <param name="bounds">The bounds that contains the nodes you want returned</param>
            internal void GetIntersectingNodes(List<QuadNode> nodes, Rect bounds)
            {
                if (bounds.IsEmpty) return;
                double w = _bounds.Width / 2;
                double h = _bounds.Height / 2;

                // assumption that the Rect struct is almost as fast as doing the operations
                // manually since Rect is a value type.

                Rect topLeft = new Rect(_bounds.Left, _bounds.Top, w, h);
                Rect topRight = new Rect(_bounds.Left + w, _bounds.Top, w, h);
                Rect bottomLeft = new Rect(_bounds.Left, _bounds.Top + h, w, h);
                Rect bottomRight = new Rect(_bounds.Left + w, _bounds.Top + h, w, h);

                // See if any child quadrants completely contain this node.
                if (topLeft.IntersectsWith(bounds) && _topLeft != null)
                {
                    _topLeft.GetIntersectingNodes(nodes, bounds);
                }

                if (topRight.IntersectsWith(bounds) && _topRight != null)
                {
                    _topRight.GetIntersectingNodes(nodes, bounds);
                }

                if (bottomLeft.IntersectsWith(bounds) && _bottomLeft != null)
                {
                    _bottomLeft.GetIntersectingNodes(nodes, bounds);
                }

                if (bottomRight.IntersectsWith(bounds) && _bottomRight != null)
                {
                    _bottomRight.GetIntersectingNodes(nodes, bounds);
                }

                GetIntersectingNodes(_nodes, nodes, bounds);
            }

            /// <summary>
            /// Walk the given linked list of QuadNodes and check them against the given bounds.
            /// Add all nodes that intersect the bounds in to the list.
            /// </summary>
            /// <param name="last">The last QuadNode in a circularly linked list</param>
            /// <param name="nodes">The resulting nodes are added to this list</param>
            /// <param name="bounds">The bounds to test against each node</param>
            static void GetIntersectingNodes(QuadNode last, List<QuadNode> nodes, Rect bounds)
            {                
                if (last != null)
                {                    
                    QuadNode n = last;
                    do
                    {
                        n = n.Next; // first node.
                        if (n.Bounds.IntersectsWith(bounds))
                        {
                            nodes.Add(n);
                        }
                    } while (n != last);                    
                }
            }

            /// <summary>
            /// Return true if there are any nodes in this Quadrant that intersect the given bounds.
            /// </summary>
            /// <param name="bounds">The bounds to test</param>
            /// <returns>boolean</returns>
            internal bool HasIntersectingNodes(Rect bounds)
            {
                if (bounds.IsEmpty) return false;
                double w = _bounds.Width / 2;
                double h = _bounds.Height / 2;

                // assumption that the Rect struct is almost as fast as doing the operations
                // manually since Rect is a value type.

                Rect topLeft = new Rect(_bounds.Left, _bounds.Top, w, h);
                Rect topRight = new Rect(_bounds.Left + w, _bounds.Top, w, h);
                Rect bottomLeft = new Rect(_bounds.Left, _bounds.Top + h, w, h);
                Rect bottomRight = new Rect(_bounds.Left + w, _bounds.Top + h, w, h);

                bool found = false;

                // See if any child quadrants completely contain this node.
                if (topLeft.IntersectsWith(bounds) && _topLeft != null)
                {
                    found = _topLeft.HasIntersectingNodes(bounds);
                }

                if (!found && topRight.IntersectsWith(bounds) && _topRight != null)
                {
                    found = _topRight.HasIntersectingNodes(bounds);
                }

                if (!found && bottomLeft.IntersectsWith(bounds) && _bottomLeft != null)
                {
                    found = _bottomLeft.HasIntersectingNodes(bounds);
                }

                if (!found && bottomRight.IntersectsWith(bounds) && _bottomRight != null)
                {
                    found = _bottomRight.HasIntersectingNodes(bounds);
                }
                if (!found)
                {
                    found = HasIntersectingNodes(_nodes, bounds);
                }
                return found;
            }

            /// <summary>
            /// Walk the given linked list and test each node against the given bounds/
            /// </summary>
            /// <param name="last">The last node in the circularly linked list.</param>
            /// <param name="bounds">Bounds to test</param>
            /// <returns>Return true if a node in the list intersects the bounds</returns>
            static bool HasIntersectingNodes(QuadNode last, Rect bounds)
            {
                if (last != null)
                {
                    QuadNode n = last;
                    do
                    {
                        n = n.Next; // first node.
                        if (n.Bounds.IntersectsWith(bounds))
                        {
                            return true;
                        }
                    } while (n != last);
                }
                return false;
            }

            /// <summary>
            /// Remove the given node from this Quadrant.
            /// </summary>
            /// <param name="node">The node to remove</param>
            /// <returns>Returns true if the node was found and removed.</returns>
            internal bool RemoveNode(T node)
            {
                bool rc = false;
                if (_nodes != null)
                {
                    QuadNode p = _nodes;
                    while (p.Next.Node != node && p.Next != _nodes)
                    {
                        p = p.Next;
                    }
                    if (p.Next.Node == node)
                    {
                        rc = true;
                        QuadNode n = p.Next;
                        if (p == n)
                        {
                            // list goes to empty
                            _nodes = null;
                        }
                        else
                        {
                            if (_nodes == n) _nodes = p;
                            p.Next = n.Next;
                        }
                    }
                }
                return rc;
            }

        }

        /// <summary>
        /// This determines the overall quad-tree indexing strategy, changing this bounds
        /// is expensive since it has to re-divide the entire thing - like a re-hash operation.
        /// </summary>
        public Rect Bounds
        {
            get { return _bounds; }
            set { _bounds = value; ReIndex();  }
        }

        /// <summary>
        /// Insert a node with given bounds into this QuadTree.
        /// </summary>
        /// <param name="node">The node to insert</param>
        /// <param name="bounds">The bounds of this node</param>
        public void Insert(T node, Rect bounds)
        {
            if (_bounds.Width == 0 || _bounds.Height == 0)
            {
                // todo: localize.
                throw new InvalidOperationException("You must set a non-zero bounds on the QuadTree first");
            }
            if (bounds.Width == 0 || bounds.Height == 0)
            {
                // todo: localize.
                throw new InvalidOperationException("Inserted node must have a non-zero width and height");
            } 
            if (_root == null)
            {
                _root = new Quadrant(null, _bounds);
            }

            Quadrant parent = _root.Insert(node, bounds);

            if (_table == null)
            {
                _table = new Dictionary<T, Quadrant>();
            }
            _table[node] = parent;


        }

        /// <summary>
        /// Get a list of the nodes that intersect the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds to test</param>
        /// <returns>List of zero or mode nodes found inside the given bounds</returns>
        public IEnumerable<T> GetNodesInside(Rect bounds)
        {
            foreach (QuadNode n in GetNodes(bounds))
            {
                yield return n.Node;
            }
        }

        /// <summary>
        /// Get a list of the nodes that intersect the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds to test</param>
        /// <returns>List of zero or mode nodes found inside the given bounds</returns>
        public bool HasNodesInside(Rect bounds)
        {
            if (_root != null)
            {
                _root.HasIntersectingNodes(bounds);
            }
            return false;
        }

        /// <summary>
        /// Get list of nodes that intersect the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds to test</param>
        /// <returns>The list of nodes intersecting the given bounds</returns>
        IEnumerable<QuadNode> GetNodes(Rect bounds)
        {
            List<QuadNode> result = new List<QuadNode>();
            if (_root != null)
            {
                _root.GetIntersectingNodes(result, bounds);
            }
            return result;
        }

        /// <summary>
        /// Remove the given node from this QuadTree.
        /// </summary>
        /// <param name="node">The node to remove</param>
        /// <returns>True if the node was found and removed.</returns>
        public bool Remove(T node)
        {
            if (_table != null)
            {
                Quadrant parent = null;
                if (_table.TryGetValue(node, out parent))
                {
                    parent.RemoveNode(node);
                    _table.Remove(node);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Rebuild all the Quadrants according to the current QuadTree Bounds.
        /// </summary>
        void ReIndex()
        {
            _root = null;
            foreach (QuadNode n in GetNodes(_bounds))
            {
                // todo: it would be more efficient if we added a code path that allowed
                // reuse of the QuadNode wrappers.
                Insert(n.Node, n.Bounds);
            }
        }
#if DEBUG_DUMP
        public void ShowQuadTree(Canvas container)
        {
            if (_root != null)
            {
                _root.ShowQuadTree(container);
            }
        }

        public void Dump(LogWriter w)
        {
            if (_root != null)
            {
                _root.Dump(w);
            }
        }
#endif
    }

#if DEBUG_DUMP
    public class LogWriter : IDisposable
    {
        XmlWriter _xw;
        int _indent;
        int _maxdepth;

        public LogWriter(TextWriter w)
        {
            XmlWriterSettings s = new XmlWriterSettings();
            s.Indent = true;            
            _xw = XmlWriter.Create(w, s);
        }

        public int MaxDepth
        {
            get
            {
                return _maxdepth;
            }
        }

        public void Open(string label)
        {
            _xw.WriteStartElement(label);
            _indent++;
            if (_indent > _maxdepth) _maxdepth = _indent;

        }
        public void Close()
        {
            _indent--;
            _xw.WriteEndElement();
        }
        public void WriteAttribute(string name, string value)
        {
            _xw.WriteAttributeString(name, value);
        }

    #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _xw != null)
            {
                using (_xw)
                {
                    _xw.Flush();
                }
                _xw = null;
            }
        }

        #endregion
    }
#endif

}
