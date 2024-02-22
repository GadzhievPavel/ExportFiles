using DeveloperUtilsLibrary;
using ExportFiles.Handler.CadVariables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.References;
using TFlex.DOCs.Model.References.Files;
using TFlex.DOCs.Model.References.Modifications;
using TFlex.DOCs.Model.References.Nomenclature;
using TFlex.DOCs.Model.References.Nomenclature.ModificationNotices;

namespace ExportFiles.Handler
{
    /// <summary>
    /// Класс для формирования подлинников в ИИ
    /// </summary>
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

        /// <summary>
        /// Создает подлинники из оригиналлов grb прикрепленных к изменяемой номенклатуре в ИИ
        /// </summary>
        /// <param name="isNewFiles"></param>
        public new void Export(bool isNewFiles)
        {
            foreach (var pair in fileObjects)
            {
                var fileSource = pair.Value;
                export.SetFileObject(fileSource);
                DataVariables dataVariables = new DataVariables();
                dataVariables.SetNotice(notice);
                dataVariables.SetFileObject(fileSource);
                dataVariables.SetNomenclature(pair.Key);
                controllerVariables.SetVarriables(dataVariables);
                var newFile = export.ExportToFormat(isNewFiles);
                if (isNewFiles)
                {
                    addAllLinkedNomenclature(newFile, fileSource);
                }
            }
        }

        /// <summary>
        /// Проверка на возможность редактирования
        /// </summary>
        /// <param name="nomenclature"></param>
        /// <returns></returns>
        private bool isEnabledStage(NomenclatureObject nomenclature)
        {
            return !nomenclature.SystemFields.Stage.Guid.Equals(StageGuids.Хранение);
        }
    }
}
