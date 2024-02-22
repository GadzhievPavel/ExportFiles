using DeveloperUtilsLibrary;
using ExportFiles.Handler.CadVariables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.Classes;
using TFlex.DOCs.Model.References;
using TFlex.DOCs.Model.References.Documents;
using TFlex.DOCs.Model.References.Files;
using TFlex.DOCs.Model.References.Nomenclature;

namespace ExportFiles
{
    public class NomenclatureExport
    {
        /// <summary>
        /// список номенклатуры, которую необходимо конвертировать
        /// </summary>
        //private HashSet<NomenclatureObject> nomenclatures;
        /// <summary>
        /// Конвертер
        /// </summary>
        protected ExportFilesGrbToTif export;
        /// <summary>
        /// коллекция разрешенных типов номенклатур для генерации подлинников
        /// </summary>
        private HashSet<NomenclatureType> enabledClassesObjectsNomenclature;
        /// <summary>
        /// Справочник ЭСИ
        /// </summary>
        private NomenclatureReference nomenclatureReference;
        /// <summary>
        /// Экспортируемые файлы
        /// </summary>
        //private HashSet<FileObject> files;

        /// <summary>
        /// Набор пар номенклатура-файл
        /// </summary>
        protected Dictionary<NomenclatureObject, FileObject> fileObjects;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nomenclatures">список экспортируемой номенклатуры</param>
        /// <param name="types">разрешенные для конвертации типы</param>
        /// <param name="connection">соединение сервера</param>
        ///

        private StageController stageController;

        protected ControllerVariables controllerVariables;
        public NomenclatureExport(List<NomenclatureObject> nomenclatures, HashSet<NomenclatureType> types, ServerConnection connection)
        {
            this.nomenclatureReference = new NomenclatureReference(connection);
            this.enabledClassesObjectsNomenclature = types;
            this.fileObjects = new Dictionary<NomenclatureObject, FileObject>();
            //this.nomenclatures = new HashSet<NomenclatureObject>();
            //files = new HashSet<FileObject>();
            foreach (var nom in nomenclatures)
            {
                AddNomenclature(nom);
            }
            this.export = new ExportFilesGrbToTif(connection);
            this.stageController = new StageController(connection);
            this.controllerVariables = new ControllerVariables(connection);
        }

        public NomenclatureExport(ServerConnection connection)
        {
            this.enabledClassesObjectsNomenclature = new HashSet<NomenclatureType> { };
            this.nomenclatureReference = new NomenclatureReference(connection);
            this.fileObjects = new Dictionary<NomenclatureObject, FileObject> { };
            //this.nomenclatures = new HashSet<NomenclatureObject> { };
            this.export = new ExportFilesGrbToTif(connection);
            //files = new HashSet<FileObject>();
            this.stageController = new StageController(connection);
            this.controllerVariables = new ControllerVariables(connection);
        }


        /// <summary>
        /// Добавление разрешенного для конвертирования типа номенклатуры
        /// </summary>
        /// <param name="nomenclatureType">тип номенклатуры</param>
        public void AddEnabledClassObjectNomenclature(NomenclatureType nomenclatureType)
        {
            enabledClassesObjectsNomenclature.Add(nomenclatureType);
        }

        /// <summary>
        /// Добавление разрешенного для конвертирования типа номенклатуры
        /// </summary>
        /// <param name="guidClassObject">строка гуида типа номенклатуры</param>
        public void AddEnabledClassObjectNomenclature(String guidClassObject)
        {
            var nomenclatureType = nomenclatureReference.Classes.Find(new Guid(guidClassObject));
            enabledClassesObjectsNomenclature.Add(nomenclatureType);
        }
        /// <summary>
        /// Добавить номенклатуру для конвертации
        /// </summary>
        /// <param name="nomenclature"></param>
        /// <returns></returns>
        public void AddNomenclature(NomenclatureObject nomenclature)
        {
            if (!IsEnableClass(nomenclature))
            {
                return;
            }
            var file = HaveGRBFile(nomenclature);
            if (file is null)
            {
                return;
            }
            if (!fileObjects.Keys.Contains(nomenclature) && !fileObjects.Values.Contains(file))
            {
                fileObjects.Add(nomenclature, file);
            }
        }

        /// <summary>
        /// Проверка на наличие файлов
        /// </summary>
        /// <param name="nomenclature"></param>
        /// <returns></returns>
        public FileObject HaveGRBFile(NomenclatureObject nomenclature)
        {
            var document = nomenclature.LinkedObject as EngineeringDocumentObject;
            if (document is null)
            {
                return null;
            }
            return document.GetFiles().Where(file => file.Class.Extension.ToLower().Equals("grb")).FirstOrDefault();
        }
        /// <summary>
        /// Проверка номенклатуры на возможность конвертации
        /// </summary>
        /// <param name="nomenclature"></param>
        /// <returns></returns>
        private bool IsEnableClass(NomenclatureObject nomenclature)
        {
            var doc = nomenclature.LinkedObject as EngineeringDocumentObject;
            if (doc is null)
            {
                return false;
            }

            if (enabledClassesObjectsNomenclature.Count == 0)
            {
                return true;
            }

            return enabledClassesObjectsNomenclature.Contains(nomenclature.Class);
        }

        /// <summary>
        /// Создает подлинник и подключает его к номенклатуре, к которой подключен исходник
        /// </summary>
        /// <param name="isNewFiles">делать новый файл подлинника или нет</param>
        public void Export(bool isNewFiles)
        {
            foreach (var pair in fileObjects)
            {
                var fileSource = pair.Value;
                export.SetFileObject(fileSource);
                controllerVariables.SetVarriables(fileSource);
                var newFile = export.ExportToFormat(isNewFiles);
                if (isNewFiles)
                {
                    addAllLinkedNomenclature(newFile, fileSource);
                }
            }
        }

        /// <summary>
        /// Подключаем подлинники ко всей номенклатуре, к которой подключен исходник
        /// </summary>
        /// <param name="newFile"></param>
        /// <param name="sourceFile"></param>
        protected void addAllLinkedNomenclature(FileObject newFile, FileObject sourceFile)
        {
            var list = sourceFile.GetObjects(Guids.DocumentsReference.Links.Files);
            var documents = list.Cast<EngineeringDocumentObject>();

            foreach (var document in documents)
            {
                var guidPrevStage = document.SystemFields.Stage.Guid;
                bool flag = false;
                if (!isEditable(document))
                {
                    stageController.ChangeStage(StageGuids.Корректировка, new List<ReferenceObject>() { document });
                    flag = true;
                }
                document.StartUpdate();
                document.AddFile(newFile);
                document.EndUpdate($"Добавление подлинника {newFile} в доккумент {document}");
                if (flag)
                {
                    stageController.ChangeStage(guidPrevStage, new List<ReferenceObject>() { document });
                }
                document.Reload();
            }
        }

        /// <summary>
        /// Можно редактировать или нет
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool isEditable(ReferenceObject obj)
        {
            return obj.SystemFields.Stage.Guid.Equals(StageGuids.Корректировка) ||
                obj.SystemFields.Stage.Guid.Equals(StageGuids.Разработка) ||
                obj.SystemFields.Stage.Guid.Equals(StageGuids.Исправление);

        }
    }
}
