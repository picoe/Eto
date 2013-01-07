using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swf = System.Windows.Forms;

namespace Eto.Platform.Windows
{
    public class DataObject : IDataObject
    {
        swf.IDataObject inner;

        public DataObject(
            swf.IDataObject inner)
        {
            this.inner = inner;
        }

        public object GetData(Type type)
        {
            return inner.GetData(type);
        }

        public string[] GetFormats()
        {
            return inner.GetFormats();
        }
    }
}
