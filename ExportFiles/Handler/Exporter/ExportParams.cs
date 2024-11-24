using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportFiles.Handler.Exporter
{
    public class ExportParams
    {
        /// <summary>
        /// Тип документа
        /// </summary>
        public string typeDocument;
        /// <summary>
        /// расширение
        /// </summary>
        public int resolution;
        /// <summary>
        /// странице содержат в названии
        /// </summary>
        public List<string> pages;
        /// <summary>
        /// код назначения
        /// </summary>
        public string code;
        /// <summary>
        /// сохранять изменения вносимые в локальный файл
        /// </summary>
        public bool saveChangesInLocalFile;
        /// <summary>
        /// расширение файла
        /// </summary>
        public string extension;
    }
}
