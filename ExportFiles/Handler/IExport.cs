using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportFiles.Handler
{
    public interface IExport
    {
        void Export(bool isNewFile);
    }
}
