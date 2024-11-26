using ExportFiles.Exception.FileException;
using ExportFiles.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using TFlex.DOCs.Model.References.Files;
using System.IO;
using TFlex.DOCs.Model.Desktop;
using TFlex.DOCs.Model.Macros;
using TFlex.DOCs.Model;

namespace ExportFiles.Handler.Exporter
{
    /// <summary>
    /// Класс для работы с файлами
    /// </summary>
    public class FileHandler
    {
        private ServerConnection connection;

        public FileHandler(ServerConnection serverConnection)
        {
            this.connection = serverConnection;
        }

        /// <summary>
        /// Скачать файл в локальную папку
        /// </summary>
        /// <param name="file">файл который надо скачать на локальную машину</param>
        /// <returns></returns>
        public bool LoadFileToLocalPath(FileObject file)
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

        /// <summary>
        /// Загрузить экспортированный файл на сервер
        /// </summary>
        /// <param name="tempFilePath">Относительный путь к временному файлу</param>
        /// <param name="parentFolderPath">родительская папка куда загружать</param>
        /// <param name="fileName">название загружаемого файла</param>
        /// <param name="exportParams">набор настроек</param>
        /// <param name="isNewFile">загружается новый файл или обновляется имеющийся</param>
        /// <returns>Созданный файл</returns>
        public FileObject UploadExportFile(string tempFilePath, string parentFolderPath, bool isNewFile)
        {
            string fileName = Path.GetFileName(tempFilePath);
            try
            {

                var fileReference = new FileReference(connection)
                {
                    LoadSettings = { LoadDeleted = true }
                };
                var parentFolder = (FolderObject)fileReference.FindByRelativePath(parentFolderPath);
                if (parentFolder == null)
                    throw new MacroException(String.Format("Не найдена родительская папка с именем '{0}'", parentFolderPath));

                parentFolder.Children.Load();

                var exportedFile = parentFolder.Children.AsList
                    .FirstOrDefault(child => child.IsFile && child.Name.Value == fileName) as FileObject;

                if (exportedFile is null)
                {
                    var fileType = fileReference.Classes.GetFileTypeByExtension(Path.GetExtension(fileName));
                    exportedFile = parentFolder.CreateFile(
                        tempFilePath,
                        String.Empty,
                        fileName,
                        fileType);
                }
                else
                {
                    if (isNewFile)
                    {
                        var fileType = fileReference.Classes.GetFileTypeByExtension(Path.GetExtension(fileName));
                        exportedFile = parentFolder.CreateFile(
                            tempFilePath,
                            String.Empty,
                            GetUniqueFileName(parentFolder, fileName),
                            fileType);
                    }
                    else
                    {
                        if (!exportedFile.IsCheckedOutByCurrentUser)
                            Desktop.CheckOut(exportedFile, false);

                        File.Copy(tempFilePath, exportedFile.LocalPath, true);
                    }
                }

                return exportedFile;

            }
            catch (SystemException e)
            {
                string exceptionMessage = String.Format(
                    "Ошибка загрузки файла на сервер.{0}" +
                    "При операции загрузки файла на сервер произошли следующие ошибки:{0}{1}",
                    Environment.NewLine, e.Message);
                throw new MacroException(exceptionMessage, e);
            }


        }

        /// <summary>
        /// проверка имени на уникальность
        /// </summary>
        /// <param name="parentFolder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetUniqueFileName(FolderObject parentFolder, string fileName)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);

            var filesName = parentFolder.Children.AsList
                .Where(child => child.IsFile)
                .Select(file => file.Name.Value)
                .ToArray();

            var filesNameSet = new HashSet<string>(filesName);

            var counter = 1;

            while (filesNameSet.Contains(fileName))
            {
                fileName = String.Format("{0}_{1}.{2}", name, counter, extension);
                counter++;
            }

            return fileName;
        }

        /// <summary>
        /// Удаление временной папки
        /// </summary>
        public void ClearTemp(string _tempFolder)
        {
            if (Directory.Exists(_tempFolder))
            {
                foreach (string filePath in Directory.GetFiles(_tempFolder))
                    DeleteFile(filePath);
            }
        }

        /// <summary>
        /// Удаление файла
        /// </summary>
        /// <param name="path">путь к файлу</param>
        private void DeleteFile(string path)
        {
            if (!File.Exists(path))
                return;

            // Получаем атрибуты файла
            var fileAttribute = File.GetAttributes(path);

            if ((fileAttribute & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                // Удаляем атрибут 'Только для чтения'
                var removeAttributes = RemoveAttribute(fileAttribute, FileAttributes.ReadOnly);
                File.SetAttributes(path, removeAttributes);
            }

            File.Delete(path);
        }

        private FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }


    }
}
