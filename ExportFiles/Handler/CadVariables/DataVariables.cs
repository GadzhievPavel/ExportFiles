using ExportFiles.Exception.FileException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.References;
using TFlex.DOCs.Model.References.Documents;
using TFlex.DOCs.Model.References.Files;
using TFlex.DOCs.Model.References.Nomenclature;

namespace ExportFiles.Handler.CadVariables
{
    /// <summary>
    /// Набор данных для установки значений в переменные
    /// </summary>
    public class DataVariables
    {
        private NomenclatureObject nomenclature;
        public FileObject fileObject { get; set; }
        public ReferenceObject notice { get; set; }
        public DataVariables() { }

        public DataVariables(NomenclatureObject nomenclature, FileObject fileObject, ReferenceObject notice)
        {
            SetNomenclature(nomenclature);
            this.fileObject = fileObject;
            this.notice = notice;
        }

        public DataVariables(NomenclatureObject nomenclature) : this(nomenclature, null, null) { }

        public DataVariables(NomenclatureObject nomenclature, FileObject fileObject): this(nomenclature, fileObject, null) { }
        /// <summary>
        /// Задать номенклатуру
        /// </summary>
        /// <param name="nomenclature"></param>
        /// <exception cref="DataVariablesException">Если номенклатура не обладает связью на файлы</exception>
        public void SetNomenclature(NomenclatureObject nomenclature)
        {
            var doc = nomenclature.LinkedObject as EngineeringDocumentObject;
            if (doc is null)
            {
                throw new DataVariablesException($"Объект {nomenclature} не может содержать файлов");
            }
            this.nomenclature = nomenclature;
        }

        public NomenclatureObject GetNomenclature() { return nomenclature; }


    }
}
