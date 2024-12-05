using DeveloperUtilsLibrary;
using ExportFiles.Handler;
using ExportFiles.Handler.CadVariables;
using ExportFiles.Handler.Exporter;
using ExportFiles.Handler.Model.API1;
using NomenclatureExtensionLibray;
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
using TFlex.DOCs.References.Configurations;
using static ExportFiles.Handler.Exporter.FileExporter;

namespace ExportFiles
{
    public class NomenclatureExport : IExport
    {
        /// <summary>
        /// Экспортер подлиников
        /// </summary>
        protected FileExporter fileExporter;
        /// <summary>
        /// коллекция разрешенных типов номенклатур для генерации подлинников
        /// </summary>
        private HashSet<NomenclatureType> enabledClassesObjectsNomenclature;
        /// <summary>
        /// Справочник ЭСИ
        /// </summary>
        private NomenclatureReference nomenclatureReference;

        /// <summary>
        /// Набор пар номенклатура-файл
        /// </summary>
        protected Dictionary<NomenclatureObject, FileObject> fileObjects;

        private StageController stageController;

        private ConfigurationsReference configReference;

        private Config config;

        protected ControllerVariables controllerVariables;

        private readonly string listTypesConfig = "Список guid номенклатуры для формирования tif";
        //public NomenclatureExport(List<NomenclatureObject> nomenclatures, HashSet<NomenclatureType> types, ServerConnection connection)
        //{
        //    this.nomenclatureReference = new NomenclatureReference(connection);
        //    this.enabledClassesObjectsNomenclature = types;
        //    this.fileObjects = new Dictionary<NomenclatureObject, FileObject>();

        //    foreach (var nom in nomenclatures)
        //    {
        //        AddNomenclature(nom);
        //    }

        //    this.configReference = new ConfigurationsReference(connection);
        //    this.stageController = new StageController(connection);

        //    ReadConfigTypesNomenclature(listTypesConfig);
        //}

        public NomenclatureExport(ServerConnection connection)
        {
            this.fileExporter = new FileExporter(connection);
            this.enabledClassesObjectsNomenclature = new HashSet<NomenclatureType> { };
            this.nomenclatureReference = new NomenclatureReference(connection);
            this.fileObjects = new Dictionary<NomenclatureObject, FileObject> { };
            this.stageController = new StageController(connection);
            this.configReference = new ConfigurationsReference(connection);
            ReadConfigTypesNomenclature(listTypesConfig);
            
        }


        private void ReadConfigTypesNomenclature(string nameConfig)
        {
            config = configReference.FindConfig(nameConfig).getParameters();
            var listStrings = new List<string>();
            listStrings.Add(config["Деталь"]);
            listStrings.Add(config["Сборочная единица"]);
            listStrings.Add(config["Изделие"]);
            listStrings.Add(config["Спецификация"]);
            listStrings.Add(config["Комплект"]);
            listStrings.Add(config["Комплекс"]);
            listStrings.Add(config["Заготовка"]);
            listStrings.Add(config["перечень элементов"]);
            listStrings.Add(config["Ведомость"]);
            listStrings.Add(config["Ведомость покупных изделий"]);
            listStrings.Add(config["Ведомость спецификаций"]);
            listStrings.Add(config["Ведомость замен"]);

            foreach(var l in listStrings)
            {
                AddEnabledClassObjectNomenclature(l);
            }
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
            var files = nomenclature.GetFiles("grb");
            if (files is null)
            {
                return;
            }
            foreach (var file in files)
            {
                if (!fileObjects.Keys.Contains(nomenclature) && !fileObjects.Values.Contains(file))
                {
                    fileObjects.Add(nomenclature, file);
                }
            }
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

        public List<FileObject> Export(string nameConfig)
        {
            var exportedFiles = new List<FileObject>();
            foreach (var pair in fileObjects)
            {
                var fileSource = pair.Value;
                fileExporter.SetFile(fileSource);
                var exportParams = fileExporter.SetSettings(nameConfig);
                var data = new DataVariables(pair.Key, fileSource);
                ControllerVariables controllerVariables = new ControllerVariables(data);
                fileExporter.setVariable = controllerVariables.GetDataVariableCad;
                var exportedFile = fileExporter.Export();

                exportedFiles.Add(exportedFile);

                if (exportParams.isNewFile)
                {
                    addAllLinkedNomenclature(exportedFile, fileSource);
                }
                exportedFile.EndUpdate("save");
            }
            return exportedFiles;
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
                if (!StageController.isEditable(document))
                {
                    stageController.ChangeStage(StageGuids.Исправление, new List<ReferenceObject>() { document });
                    flag = true;
                }
                document.StartUpdate();
                document.AddFile(newFile);
                document.EndUpdate($"Добавление подлинника {newFile} в документ {document}");
                if (flag)
                {
                    stageController.ChangeStage(guidPrevStage, new List<ReferenceObject>() { document });
                }
                document.Reload();
            }
        }

        public List<NomenclatureObject> GetNomenclatures()
        {
            return this.fileObjects.Keys.ToList();
        }
    }
}
