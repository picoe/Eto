using System;
using System.Collections;
using Eto.Drawing;
using Eto.Collections;

namespace Eto.Forms
{
	public interface ITreeNode
	{
		Icon Icon { get; set; }
		string Text { get; set; }
		void Clear();
		void AddNode(TreeNode node);
		void RemoveNode(TreeNode node);
	}

	/// <summary>
	/// Summary description for TreeNode.
	/// </summary>
	public class TreeNode : Widget
	{
		private ITreeNode inner;
		private TreeNodeCollection nodes;

		public TreeNode(Generator g) : base(g, typeof(ITreeNode))
		{
			inner = (ITreeNode)Handler;
			nodes = new TreeNodeCollection(this);
		}

		public Icon Icon
		{
			get { return inner.Icon; }
			set { inner.Icon = value; }
		}

		public string Text
		{
			get { return inner.Text; }
			set { inner.Text = value; }
		}

		public TreeNodeCollection Nodes
		{
			get { return nodes; }
		}
	}

	public class TreeNodeCollection : BaseList<TreeNode>
	{
		TreeNode node;

		public TreeNodeCollection(TreeNode node)
		{
			this.node = node;
		}

		public void Add(string text, Icon icon)
		{
			TreeNode child = new TreeNode(node.Generator);
			child.Text = text;
			child.Icon = icon;
			Add(child);
		}

		public override void Clear()
		{
			base.Clear();
			((ITreeNode)node.Handler).Clear();
		}
		protected override void OnAdded (ListEventArgs<TreeNode> e)
		{
			base.OnAdded (e);
			((ITreeNode)node.Handler).AddNode(e.Item);
		}
		
		protected override void OnRemoved (ListEventArgs<TreeNode> e)
		{
			base.OnRemoved (e);
			((ITreeNode)node.Handler).RemoveNode(e.Item);
		}
	}
}
