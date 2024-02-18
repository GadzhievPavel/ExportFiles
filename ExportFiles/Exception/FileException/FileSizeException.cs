using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportFiles.Exception
{
    public class FileSizeException : ExportFilesException
    {
        public FileSizeException(string message) : base(message)
        {

        }
    }
}
