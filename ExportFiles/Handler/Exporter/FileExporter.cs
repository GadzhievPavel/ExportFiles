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
        public FileExporter(ServerConnection connection, FileObject fileSettings, bool isNew)
        {
            this.connection = connection;
            SetSettings(fileSettings);
            this.isNew = isNew;

            if (!Directory.Exists(_tempFolder))
                Directory.CreateDirectory(_tempFolder);
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
            catch(SystemException ex)
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


                var exportedFileName = getNameExportFile(file);
                string tempExportingFilePath = Path.Combine(_tempFolder, String.Format("{0}.{1}", Guid.NewGuid(), _extensionDoc));

            }
            catch
            {

            }
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
    }
}
