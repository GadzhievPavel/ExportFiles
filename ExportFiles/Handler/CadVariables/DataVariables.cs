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
        private FileObject fileObject;
        private ReferenceObject notice;
        public DataVariables() { }

        public DataVariables(NomenclatureObject nomenclature, FileObject fileObject, ReferenceObject notice)
        {
            SetNomenclature(nomenclature);
            SetFileObject(fileObject);
            SetNotice(notice);
        }

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

        /// <summary>
        /// Задать файл
        /// </summary>
        /// <param name="fileObject"></param>
        public void SetFileObject(FileObject fileObject)
        {
            this.fileObject = fileObject;
        }
        public FileObject GetFileObject() { return fileObject; }
        /// <summary>
        /// Задать ИИ
        /// </summary>
        /// <param name="notice"></param>
        /// <exception cref="DataVariablesException">Если объект не является ИИ</exception>
        public void SetNotice(ReferenceObject notice)
        {
            if (!notice.Reference.ParameterGroup.ReferenceInfo.Guid.Equals(Guids.NoticeModificationReference.ИзвещенияОбИзменениях))
            {
                throw new DataVariablesException($"объект {notice} не относится к справчнику \"Измевещния обИзменениях\"");
            }
            this.notice = notice;
        }
        public ReferenceObject GetNotice() { return notice; }
    }
}
