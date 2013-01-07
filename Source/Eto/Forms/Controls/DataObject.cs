using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Forms
{
    public interface IDataObject
    {
        object GetData(Type type);

        string[] GetFormats();
    }
}
