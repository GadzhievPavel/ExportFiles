using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.References.Documents;
using TFlex.DOCs.Model.References.Files;
using TFlex.DOCs.Model.References.Nomenclature;

namespace ExportFiles.Data
{
    public class InfoExportedFile
    {
        /// <summary>
        /// Экспортируемый файл
        /// </summary>
        public FileObject file {  get; set; }
        /// <summary>
        /// Номенклатура от которой брать параметры в переменные
        /// </summary>
        public NomenclatureObject nomenclature { get; set; }
        /// <summary>
        /// Список связанной с файлом номенклатуры
        /// </summary>
        public HashSet<EngineeringDocumentObject> linkedDocuments { get; set; }

        public override string ToString()
        {
            return $"{file?.ToString()} {nomenclature?.ToString()} {linkedDocuments?.Count}";
        }
    }
}
