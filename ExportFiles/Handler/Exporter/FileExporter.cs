using ExportFiles.Exception;
using ExportFiles.Exception.FileException;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.Desktop;
using TFlex.DOCs.Model.FilePreview.CADService;
using TFlex.DOCs.Model.FilePreview.CADService.TFlexCadDocument;
using TFlex.DOCs.Model.Macros;
using TFlex.DOCs.Model.References.Files;
using TFlex.DOCs.References.Configurations;

namespace ExportFiles.Handler.Exporter
{
    public class FileExporter
    {
        /// <summary>
        ///  Генерировать новый подлинник или нет
        /// </summary>
        private bool isNew;
        /// <summary>
        /// Файл исходник формата grb
        /// </summary>
        private FileObject file;
        /// <summary>
        /// подключение к приложению T-FLEX DOCs
        /// </summary>
        private ServerConnection connection;
        /// <summary>
        /// путь для хранения файлов во время операции экспорта формата
        /// </summary>
        private static readonly string _tempFolder = Path.Combine(Path.GetTempPath(), "Temp DOCs", "ExportGRB");

        private ExportParams exportParameters;
        /// <summary>
        /// провайдер для работы с CAD документом T-FLEX
        /// </summary>
        private CadDocumentProvider provider;

        private FileHandler fileHandler;

        public FileExporter(ServerConnection connection, string nameConfig, bool isNew)
        {
            this.connection = connection;
            SetSettings(nameConfig);
            this.isNew = isNew;

            if (!Directory.Exists(_tempFolder))
                Directory.CreateDirectory(_tempFolder);

            this.provider = CadDocumentProvider.Connect(connection, ".grb");
            this.fileHandler = new FileHandler(connection);
        }

        /// <summary>
        /// Чтение конфига для эксплорта
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="ExportFilesException"></exception>
        private void SetSettings(string nameConfig)
        {
            var configReference = new ConfigurationsReference(this.connection);
            var config = configReference.FindConfig(nameConfig);

            this.exportParameters = new ExportParams(config.getParameters());
            try
            {
                string tempExportingFilePath = Path.Combine(_tempFolder, String.Format("{0}.{1}", Guid.NewGuid(), exportParameters.extension));
                exportParameters.tempExportingFilePath = tempExportingFilePath;
            }
            catch (SystemException ex)
            {
                throw new ExportFilesException(ex);
            }

        }

        /// <summary>
        /// Задаем файл исходник в формате grb
        /// </summary>
        /// <param name="file"></param>
        /// <exception cref="ExportFilesException"></exception>
        public void SetFile(FileObject file)
        {
            if (file is null)
            {
                throw new ExportFilesException($"в конструктор подан null файл");
            }
            if (!file.Class.Extension.ToLower().Equals("grb"))
            {
                throw new ExportFilesException("файл не grb формата");
            }
            this.file = file;
        }

        /// <summary>
        /// Экспортировать файл
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ExportFilesException"></exception>
        /// <exception cref="MacroException"></exception>
        public FileObject Export()
        {
            FileObject uploadedFile = null;
            try
            {
                fileHandler.LoadFileToLocalPath(file);

                
                var exportedFileName = Path.GetFileNameWithoutExtension(file.Name);

                if (exportParameters is null)
                {
                    throw new ExportFilesException("отсутсвуют параметры для экспорта");
                }

                var document = provider.OpenDocument(file.LocalPath, false);
                if (document is null)
                {
                    throw new ExportFilesException(String.Format(
                        "Ошибка получения страниц.{0}" +
                        "При операции получения страниц произошла следующая ошибка:{0}" +
                        "Файл '{1}' не может быть открыт", Environment.NewLine, file.Name));
                }

                var exportContext = GetExportContext(document);
                var pathNewFile = document.Export(exportContext);

                uploadedFile = fileHandler.UploadExportFile(exportParameters.tempExportingFilePath, file.Parent.Path, isNew);
                document.Close(exportParameters.saveChangesInLocalFile);

                if (pathNewFile == null)
                {
                    throw new MacroException(String.Format(
                        "Ошибка экспорта.{0}" +
                        "При операции экспорта в '{1}' произошли следующие ошибки:{0}" +
                        "Файл '{2}' не может быть экспортирован",
                        Environment.NewLine, exportParameters.extension, file.Name));
                }


            }
            catch (SystemException ex)
            {
                throw new ExportFilesException(ex);
            }
            finally
            {
                
            }

            return uploadedFile;
        }


        private ExportContext GetExportContext( CadDocument document)
        {
            var exportContext = new ExportContext(exportParameters.tempExportingFilePath);
            exportContext["resolution"] = exportParameters.resolution;

            var pages = document.GetTFlexPagesInfo();

            var selectPage = selectPages(pages, exportParameters);

            exportContext.Pages.AddRange(selectPage.Select(sp => sp.Index));
            return exportContext;
        }

        private HashSet<TFlexPageInfo> selectPages(TFlexPageInfo[] flexPages, ExportParams exportParams)
        {
            var pages = new HashSet<TFlexPageInfo>();
            foreach (TFlexPageInfo page in flexPages)
            {
                foreach (var namePage in exportParams.pages)
                {
                    if (page.Name.Contains(namePage))
                    {
                        pages.Add(page);
                    }
                }

            }
            return pages;
        }
    }
}
