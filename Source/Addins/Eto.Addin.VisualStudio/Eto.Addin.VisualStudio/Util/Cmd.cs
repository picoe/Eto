using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Addin.VisualStudio.Util
{
	struct Cmd
	{
		public Guid CmdGroup;
		public uint CmdID;

		public static Cmd FromID<T>(T val)
			where T : struct
		{
			return new Cmd(typeof(T).GUID, Convert.ToUInt32(val));
		}

		public Cmd(Guid group, uint id)
		{
			CmdGroup = group;
			CmdID = id;
		}

		public override bool Equals(object obj)
		{
			return obj is Cmd && (Cmd)obj == this;
		}

		public override int GetHashCode()
		{
			return CmdGroup.GetHashCode() ^ CmdID.GetHashCode();
		}

		public static bool operator ==(Cmd cmd1, Cmd cmd2)
		{
			return cmd1.CmdGroup == cmd2.CmdGroup && cmd1.CmdID == cmd2.CmdID;
		}

		public static bool operator !=(Cmd cmd1, Cmd cmd2)
		{
			return !(cmd1 == cmd2);
		}
	}
}
