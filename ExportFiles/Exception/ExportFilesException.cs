using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportFiles.Exception
{
    public class ExportFilesException : SystemException
    {
        public ExportFilesException(string message): base(message)   { }
    }
}
