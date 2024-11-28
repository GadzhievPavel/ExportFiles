using ExportFiles.Handler.Model.API1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.References.Configurations;

namespace ExportFiles.Handler.Exporter
{
    public class ExportParams
    {
        public ExportParams(Config config)
        {
            this.resolution = config["Resolution"];
            this.code = config["code"];
            this.saveChangesInLocalFile = config["SaveChangesInLocalFile"];
            this.extension = config["Extension"];
            this.tempExportingFilePath = config["TempExportingFilePath"];
            this.pages = new List<string>();
            pages.Add(config["namePage"]);
        }
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
        /// <summary>
        /// Временная папка для экспортированного файла
        /// </summary>
        public string tempExportingFilePath;

        public override string ToString()
        {
            var sb = new StringBuilder().AppendLine($"" +
                $"resolution - {resolution}\n" +
                $"code - {code}\n" +
                $"saveChangesInLocalFile - {saveChangesInLocalFile}\n" +
                $"extension - {extension}\n" +
                $"tempExportingFilePath - {tempExportingFilePath}\n" +
                $"");
            pages.ForEach(page => { sb.AppendLine($"page - {page}"); });

            return sb.ToString();

        }
    }
}
