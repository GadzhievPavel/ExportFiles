using ExportFiles.Handler.CadVariables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.References.Files;
using TFlex.DOCs.Model.References.Nomenclature;

namespace ExportFiles.Handler
{
    public interface IExport
    {
        /// <summary>
        /// Формирует подлинники с форматом tif
        /// </summary>
        /// <param name="isNewFile"></param>
        List<FileObject> Export(string nameConfig);

    }
}
