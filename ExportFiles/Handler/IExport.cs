using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.References.Files;

namespace ExportFiles.Handler
{
    public interface IExport
    {
        /// <summary>
        /// Формирует подлинники с форматом tif
        /// </summary>
        /// <param name="isNewFile"></param>
        void Export(bool isNewFile);

        /// <summary>
        /// Возвращает подлинники с форматом tif
        /// </summary>
        /// <returns></returns>
        List<FileObject> GetTifFiles();
    }
}
