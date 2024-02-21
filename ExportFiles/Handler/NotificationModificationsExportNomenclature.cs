using DeveloperUtilsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.References;
using TFlex.DOCs.Model.References.Modifications;
using TFlex.DOCs.Model.References.Nomenclature;
using TFlex.DOCs.Model.References.Nomenclature.ModificationNotices;

namespace ExportFiles.Handler
{
    public class ModificationNoticeExportNomenclature : NomenclatureExport
    {
        /// <summary>
        /// Объект справочника "Извещения об изменениях"
        /// </summary>
        private ReferenceObject notice;
        
        public ModificationNoticeExportNomenclature(ServerConnection connection) : base(connection)
        {
        }

        /// <summary>
        /// Добавляет извещение для экспорта файлов номенклатуры
        /// </summary>
        /// <param name="notice"></param>
        public void AddNotice(ReferenceObject notice)
        {
            var modifications = notice.GetObjects(ModificationReferenceObject.RelationKeys.ModificationNotice);
            foreach (var modification in modifications)
            {
                var nomenclature = modification.GetObject(ModificationReferenceObject.RelationKeys.PDMObject) as NomenclatureObject;
                if (isEnabledStage(nomenclature))
                {
                    AddNomenclature(nomenclature);
                }
            }
        }

        private bool isEnabledStage(NomenclatureObject nomenclature)
        {
            return !nomenclature.SystemFields.Stage.Guid.Equals(StageGuids.Хранение);
        }
    }
}
