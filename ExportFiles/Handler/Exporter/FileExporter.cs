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
using TFlex.DOCs.Model.FilePreview.CADService;
using TFlex.DOCs.Model.FilePreview.CADService.TFlexCadDocument;
using TFlex.DOCs.Model.Macros;
using TFlex.DOCs.Model.References.Files;

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

        private string _extensionDoc = "tif";

        private ExportParams exportParameters;
        /// <summary>
        /// провайдер для работы с CAD документом T-FLEX
        /// </summary>
        private CadDocumentProvider provider;
        public FileExporter(ServerConnection connection, FileObject fileSettings, bool isNew)
        {
            this.connection = connection;
            SetSettings(fileSettings);
            this.isNew = isNew;

            if (!Directory.Exists(_tempFolder))
                Directory.CreateDirectory(_tempFolder);

            this.provider = CadDocumentProvider.Connect(connection, ".grb");
        }

        public FileExporter(ServerConnection connection, bool isNew) : this(connection, null, isNew)
        {

        }

        public FileExporter(ServerConnection connection) : this(connection, null, false)
        {

        }

        /// <summary>
        /// Чтение конфига для эксплорта
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="ExportFilesException"></exception>
        public void SetSettings(FileObject config)
        {
            LoadFileToLocalPath(config);
            var path = config.LocalPath;
            string textFromFile = null;
            using (FileStream fstream = File.OpenRead(path))
            {
                byte[] buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
                textFromFile = Encoding.UTF8.GetString(buffer);
            }
            try
            {

                exportParameters = (ExportParams)JsonConvert.DeserializeObject(textFromFile);
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
        }

        public FileObject Export()
        {
            FileObject uploadedFile = null;
            try
            {
                LoadFileToLocalPath(file);
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

                var exportContext = GetExportContext(exportParameters, document);
                var pathNewFile = document.Export(exportContext);

                document.Close(exportParameters.saveChangesInLocalFile);

                if (pathNewFile == null)
                {
                    throw new MacroException(String.Format(
                        "Ошибка экспорта.{0}" +
                        "При операции экспорта в '{1}' произошли следующие ошибки:{0}" +
                        "Файл '{2}' не может быть экспортирован",
                        Environment.NewLine, _extensionDoc, file.Name));
                }
            }
            catch
            {

            }
        }

        private ExportContext GetExportContext(ExportParams exportParams, CadDocument document)
        {
            var exportedFileName = getNameExportFile(file);
            string tempExportingFilePath = Path.Combine(_tempFolder, String.Format("{0}.{1}", Guid.NewGuid(), exportParams.extension));

            var exportContext = new ExportContext(tempExportingFilePath);
            exportContext["resolution"] = exportParams.resolution;

            var pages = document.GetTFlexPagesInfo();

            var selectPage = selectPages(pages, exportParams);

            exportContext.Pages.AddRange(selectPage.Select(sp => sp.Index));
            return exportContext;
        }

        /// <summary>
        /// Загрузка файлов в локальную папку
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool LoadFileToLocalPath(FileObject file)
        {
            // Получаем последнюю версию файла
            file.GetHeadRevision();

            if (file.Size == 0)
            {
                throw new FileSizeException($"Файл '{file}' не содержит данных");
            }

            if (File.Exists(file.LocalPath))
                return true;

            throw new LoadLocalFileException($"Ошибка загрузки файла '{file}'");
        }

        private string getNameExportFile(FileObject file)
        {
            string exportedFileName = file.Name;
            exportedFileName = exportedFileName.Substring(0, exportedFileName.Length - 4);
            return exportedFileName;
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

        /// <summary>
        /// Загрузить экспортированный файл на сервер
        /// </summary>
        /// <param name="tempFilePath">Относительный путь к временному файлу</param>
        /// <param name="parentFolderPath">Относительный путь к родительской папке</param>
        /// <param name="grbFileObject">Объект grb файла</param>
        /// <param name="fileName">Имя, с которым файл будет сохранен в справочник</param>
        /// <returns>Созданный файл</returns>
        private FileObject UploadExportFile(string tempFilePath, string parentFolderPath, FileObject grbFileObject, string fileName)
        {
        }
}
