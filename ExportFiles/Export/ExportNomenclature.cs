using DeveloperUtilsLibrary;
using ExportFiles.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.Desktop;
using TFlex.DOCs.Model.References;
using TFlex.DOCs.Model.References.ConversionTaskQueue;
using TFlex.DOCs.Model.References.Nomenclature;
using TFlex.DOCs.Model.Search;

namespace ExportFiles.Export
{
    /// <summary>
    /// Класс для формирования подлинников номенклатуры
    /// </summary>
    public class ExportNomenclature
    {
        /// <summary>
        /// Коллекция номенклатуры и файлов
        /// </summary>
        private InfoExportedFiles infoExportedFiles { get; set; }
        /// <summary>
        /// справочник конвертации подлинников
        /// </summary>
        private ConversionTaskQueueReference queueReference { get; set; }
        
        public ExportNomenclature(ServerConnection serverConnection)
        {
            this.infoExportedFiles = new InfoExportedFiles(serverConnection);
            this.queueReference = new ConversionTaskQueueReference(serverConnection);
            
        }

        /// <summary>
        /// метод для добавления номенклатуры
        /// </summary>
        /// <param name="nom">номенклатура</param>
        public void AddNomenclature(NomenclatureObject nom)
        {
            infoExportedFiles.Add(nom);
        }

        /// <summary>
        /// Экспортировать подлинники
        /// </summary>
        /// <param name="nameGroupTask">группа задач</param>
        /// <param name="moduleConvertion">модуль конвертации</param>
        /// <param name="formatConvertion">формат конвертации</param>
        public void Export(string nameGroupTask, string moduleConvertion, string formatConvertion)
        {
            var groupTask = queueReference.Find(
                Filter.Parse($"[Наименование] = '{nameGroupTask}' И [Тип] = 'Группа задач'",
                queueReference.ParameterGroup)).FirstOrDefault() as ConversionTaskQueueReferenceObject;


            var saveSet = new ReferenceObjectSaveSet();
            foreach (var exp in infoExportedFiles)
            {
                var task = queueReference.CreateReferenceObject(groupTask) as ConversionTaskQueueReferenceObject;
                task.StartUpdate();
                var taskBuilder = new TaskConvertionBuilder(task);
                task = taskBuilder.SetFile(exp.file).ConnectFileToNomenclature(true).
                    SetNameOutputFile(exp.file.Name).SetFormatConvertation(formatConvertion).
                    SetModuleConvertation(moduleConvertion).SetNomenclature(exp.nomenclature).Build();
                saveSet.Add(task);
            }
            saveSet.EndChanges();
        }
    }
}
