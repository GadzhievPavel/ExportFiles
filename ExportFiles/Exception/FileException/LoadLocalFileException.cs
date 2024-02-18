using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportFiles.Exception.FileException
{
    public class LoadLocalFileException : ExportFilesException
    {
        public LoadLocalFileException(string message) : base(message)
        {
        }
    }
}
