using DeveloperUtilsLibrary;
using ExportFiles.Exception;
using ExportFiles.Exception.FileException;
using ExportFiles.Handler.CadVariables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.Classes;
using TFlex.DOCs.Model.Desktop;
using TFlex.DOCs.Model.FilePreview.CADService;
using TFlex.DOCs.Model.FilePreview.CADService.TFlexCadDocument;
using TFlex.DOCs.Model.Macros;
using TFlex.DOCs.Model.References;
using TFlex.DOCs.Model.References.Documents;
using TFlex.DOCs.Model.References.Files;
using TFlex.DOCs.Model.References.Nomenclature;
using TFlex.DOCs.Model.Structure;
using TFlex.DOCs.Resources.Strings;

namespace ExportFiles
{
    public class ExportFilesGrbToTif
    {
        private static readonly string _tempFolder = Path.Combine(Path.GetTempPath(), "Temp DOCs", "ExportGRB");
        private CadDocumentProvider provider;
        private string _extensionDoc = "tif";
        private int _resolution = 300;
        private bool _exportToNewFile = true;
        /// <summary> Параметр код назначения </summary>
        private ParameterInfo _codeParameter;
        private string _selectedCodeName = "Подлинник";

        private ServerConnection connection;
        DocumentReference documentReference;

        /// <summary>
        /// Файл
        /// </summary>
        private FileObject fileObject;
        /// <summary>
        /// Объект номенклатуры
        /// </summary>
        private NomenclatureObject nomenclature;

        public ExportFilesGrbToTif(ServerConnection connection)
        {
            this.connection = connection;
            this.documentReference = new DocumentReference(connection);
            this.provider = CadDocumentProvider.Connect(connection, ".grb");
        }


        /// <summary>
        /// Задать файл
        /// </summary>
        /// <param name="file"></param>
        public void SetFileObject(FileObject file)
        {
            fileObject = file;
        }

        /// <summary>
        /// Задать номенклатуру
        /// </summary>
        /// <param name="nomenclature"></param>
        public void SetNomenclature(NomenclatureObject nomenclature)
        {
            this.nomenclature = nomenclature;
        }

        /// <summary>
        /// Экспортировать файл
        /// </summary>
        /// <param name="fileObject">файл grb</param>
        /// <param name="isNewFile">создать новый подлинник</param>
        /// <returns>новый подлинник</returns>
        public FileObject ExportToFormat(bool isNewFile)
        {
            FileObject uploadedFile = null;
            if (fileObject == null)
            {
                return null;
            }
            if (fileObject.Class.Extension != "grb")
            {
                return null;
            }

            if (!Directory.Exists(_tempFolder))
                Directory.CreateDirectory(_tempFolder);

            try
            {
                if (!LoadGrbFileToLocalPath(fileObject))
                    return null;
                //var pagesInfo = GetPagesInfo(fileObject.LocalPath);
                _exportToNewFile = isNewFile;

                string exportedFileName = fileObject.Name;
                exportedFileName = exportedFileName.Substring(0, exportedFileName.Length - 4);
                string tempExportingFilePath = Path.Combine(_tempFolder, String.Format("{0}.{1}", Guid.NewGuid(), _extensionDoc));

                var exportContext = new ExportContext(tempExportingFilePath);
                exportContext["resolution"] = _resolution;

                var selectedPages = GetPagesInfo(fileObject.LocalPath);

                var pages = selectedPages.Where(pg => pg.Name.Contains("Страница"));

                exportContext.Pages.AddRange(pages.Cast<TFlexPageInfo>().Select(sp => sp.Index));

                ExportGrbToSelectedFormat(exportContext);

                uploadedFile = UploadExportFile(tempExportingFilePath, fileObject.Parent.Path, fileObject, exportedFileName);
            }
            catch (SystemException ex)
            {
                throw ex;
            }
            finally
            {
                ClearTemp();
            }
            return uploadedFile;
        }

        public FileObject ExportToFormat(bool isNewFile, DataVariables dataVariables)
        {
            FileObject uploadedFile = null;
            if (fileObject == null)
            {
                return null;
            }
            if (fileObject.Class.Extension != "grb")
            {
                return null;
            }

            if (!Directory.Exists(_tempFolder))
                Directory.CreateDirectory(_tempFolder);

            try
            {
                if (!LoadGrbFileToLocalPath(fileObject))
                    return null;
                //var pagesInfo = GetPagesInfo(fileObject.LocalPath);
                _exportToNewFile = isNewFile;

                string exportedFileName = fileObject.Name;
                exportedFileName = exportedFileName.Substring(0, exportedFileName.Length - 4);
                string tempExportingFilePath = Path.Combine(_tempFolder, String.Format("{0}.{1}", Guid.NewGuid(), _extensionDoc));

                var exportContext = new ExportContext(tempExportingFilePath);
                exportContext["resolution"] = _resolution;

                var selectedPages = GetPagesInfo(fileObject.LocalPath);

                var pages = selectedPages.Where(pg => pg.Name.Contains("Страница"));

                exportContext.Pages.AddRange(pages.Cast<TFlexPageInfo>().Select(sp => sp.Index));

                ExportGrbToSelectedFormat(exportContext, dataVariables);

                uploadedFile = UploadExportFile(tempExportingFilePath, fileObject.Parent.Path, fileObject, exportedFileName);
            }
            catch (SystemException ex)
            {
                throw ex;
            }
            finally
            {
                ClearTemp();
            }
            return uploadedFile;
        }
        /// <summary>
        /// Экспорт выбранного файла
        /// </summary>
        /// <param name="exportContext">Контекст экспорта</param>
        /// <exception cref="MacroException"></exception>
        private void ExportGrbToSelectedFormat(ExportContext exportContext)
        {
            var pathToGrbFile = fileObject.LocalPath;

            // Открытие документа grb
            // Менять документ не будем, поэтому второй аргумент функции (readOnly) равен true
            using (var document = provider.OpenDocument(pathToGrbFile, true))
            {
                var tempGrbFileName = Path.GetFileName(pathToGrbFile);

                // Проверяем был ли открыт документ
                if (document == null)
                    throw new MacroException(String.Format("Файл '{0}' не может быть открыт", tempGrbFileName));

                // Экспортируем документ в другой формат на основе контекста настройки
                // Получаем полный путь до экспортированного файла, для дальнейшей проверки экспорта
                var path = document.Export(exportContext);

                // Закрываем grb документ без сохранения
                document.Close(false);

                if (path == null)
                {
                    throw new MacroException(String.Format(
                        "Ошибка экспорта.{0}" +
                        "При операции экспорта в '{1}' произошли следующие ошибки:{0}" +
                        "Файл '{2}' не может быть экспортирован",
                        Environment.NewLine, _extensionDoc, tempGrbFileName));
                }
            }
        }

        private void ExportGrbToSelectedFormat(ExportContext exportContext, DataVariables dataVariables)
        {
            var pathToGrbFile = fileObject.LocalPath;

            // Открытие документа grb
            // Менять документ не будем, поэтому второй аргумент функции (readOnly) равен true
            using (var document = provider.OpenDocument(pathToGrbFile, true))
            {
                var tempGrbFileName = Path.GetFileName(pathToGrbFile);

                // Проверяем был ли открыт документ
                if (document == null)
                    throw new MacroException(String.Format("Файл '{0}' не может быть открыт", tempGrbFileName));


                var variables = document.GetVariables();
                var variablesController = new ControllerVariables(this.connection);
                variablesController.SetVariables(dataVariables, variables);

                // Экспортируем документ в другой формат на основе контекста настройки
                // Получаем полный путь до экспортированного файла, для дальнейшей проверки экспорта
                var path = document.Export(exportContext);

                // Закрываем grb документ без сохранения
                document.Close(false);

                if (path == null)
                {
                    throw new MacroException(String.Format(
                        "Ошибка экспорта.{0}" +
                        "При операции экспорта в '{1}' произошли следующие ошибки:{0}" +
                        "Файл '{2}' не может быть экспортирован",
                        Environment.NewLine, _extensionDoc, tempGrbFileName));
                }
            }
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
            try
            {
                var fileReference = new FileReference(connection)
                {
                    LoadSettings = { LoadDeleted = true }
                };

                var parentFolder = (TFlex.DOCs.Model.References.Files.FolderObject)fileReference.FindByRelativePath(parentFolderPath);
                if (parentFolder == null)
                    throw new MacroException(String.Format("Не найдена родительская папка с именем '{0}'", parentFolderPath));
                parentFolder.Children.Load();

                var uploadingFileName = String.Format("{0}.{1}", fileName, _extensionDoc);

                var exportedFile = parentFolder.Children.AsList
                    .FirstOrDefault(child => child.IsFile && child.Name.Value == uploadingFileName) as FileObject;

                if (exportedFile is null)
                {
                    var fileType = GetFileType(fileReference);
                    exportedFile = parentFolder.CreateFile(
                        tempFilePath,
                        String.Empty,
                        uploadingFileName,
                        fileType);
                }
                else
                {
                    if (_exportToNewFile)
                    {
                        var fileType = GetFileType(fileReference);
                        exportedFile = parentFolder.CreateFile(
                            tempFilePath,
                            String.Empty,
                            GetUniqueExportedFileName(uploadingFileName, parentFolder, fileName),
                            fileType);
                    }
                    else
                    {
                        if (!exportedFile.IsCheckedOutByCurrentUser)
                            Desktop.CheckOut(exportedFile, false);

                        File.Copy(tempFilePath, exportedFile.LocalPath, true);
                    }
                }

                exportedFile.BeginChanges();
                SetCodeParameter(exportedFile);
                AddDocument(grbFileObject, exportedFile);
                exportedFile.EndChanges();
                AddFileToChange(grbFileObject, exportedFile);
                Desktop.CheckIn(exportedFile, String.Format(
                    "Экспорт файла:{0}'{1}'{0}в формат '{3}':{0}'{2}'",
                    Environment.NewLine, grbFileObject.Path, exportedFile.Path, _extensionDoc), false);
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
        /// Установка кода назначения файла
        /// </summary>
        /// <param name="exportedFile"></param>
        private void SetCodeParameter(FileObject exportedFile)
        {
            if (_codeParameter != null)
            {
                int codeValue = (int)_codeParameter.ValueList.GetValue(_selectedCodeName);
                exportedFile.Code.Value = codeValue;
            }
        }

        /// <summary>
        /// Подключение подлинника к документу
        /// </summary>
        /// <param name="grbFileObject"></param>
        /// <param name="exportedFile"></param>
        private void AddDocument(FileObject grbFileObject, FileObject exportedFile)
        {
            var masterObject = grbFileObject.MasterObject;
            if (masterObject is EngineeringDocumentObject documentObject)
            {
                if (!exportedFile.Links.ToMany[EngineeringDocumentFields.File]
                    .Objects
                    .Any(document => document.SystemFields.Guid == documentObject.SystemFields.Guid))
                {
                    exportedFile.AddLinkedObject(EngineeringDocumentFields.File, documentObject);
                }
            }
        }

        private void AddFileToChange(FileObject grbFileObject, FileObject exportedFile)
        {
            var masterObject = grbFileObject.MasterObject;
            if (masterObject != null && masterObject.Reference.ParameterGroup.Guid == Guids.ChangeReference.Изменения)
            {
                if (!masterObject.Links.ToMany[Guids.ChangeReference.Links.ИзмененияРабочиеФайлы]
                    .Objects
                    .Any(workingFile => workingFile.SystemFields.Guid == exportedFile.SystemFields.Guid))
                {
                    masterObject.BeginChanges();
                    masterObject.AddLinkedObject(Guids.ChangeReference.Links.ИзмененияРабочиеФайлы, exportedFile);
                    masterObject.EndChanges();
                }
            }
        }
        /// <summary>
        /// Удаление временной папки
        /// </summary>
        private void ClearTemp()
        {
            if (Directory.Exists(_tempFolder))
            {
                foreach (string filePath in Directory.GetFiles(_tempFolder))
                    DeleteFile(filePath);
            }
        }

        private FileType GetFileType(FileReference fileReference)
        {
            var fileType = fileReference.Classes.GetFileTypeByExtension(_extensionDoc);
            if (fileType is null)
            {
                string typeName = String.Format(Texts.FileNameWithExtension, _extensionDoc.ToUpper());
                fileType = fileReference.Classes.CreateFileType(typeName, String.Empty, _extensionDoc);
            }

            return fileType;
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

        private string GetUniqueExportedFileName(string exportedFileName, TFlex.DOCs.Model.References.Files.FolderObject parentFolder, string fileName)
        {
            var filesName = parentFolder.Children.AsList
                .Where(child => child.IsFile)
                .Select(file => file.Name.Value)
                .ToArray();

            var filesNameSet = new HashSet<string>(filesName);

            var counter = 1;

            while (filesNameSet.Contains(exportedFileName))
            {
                exportedFileName = String.Format("{0}_{1}.{2}", fileName, counter, _extensionDoc);
                counter++;
            }

            return exportedFileName;
        }

        /// <summary>
        /// Загрузка файлов в локальную папку
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool LoadGrbFileToLocalPath(FileObject file)
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
        /// Вернуть набор информации о страницах
        /// </summary>
        /// <param name="pathToGrbFile"></param>
        /// <returns></returns>
        /// <exception cref="MacroException">выкидывает исключение, если не получилось открыть документ</exception>
        private TFlexPageInfo[] GetPagesInfo(string pathToGrbFile)
        {
            TFlexPageInfo[] pagesInfo;

            // Открытие документа grb. Менять документ не будем, поэтому второй аргумент функции (readOnly) равен true.
            using (var document = provider.OpenDocument(pathToGrbFile, true))
            {
                // Проверяем, был ли открыт документ
                if (document == null)
                {
                    var fileName = Path.GetFileName(pathToGrbFile);

                    throw new MacroException(String.Format(
                        "Ошибка получения страниц.{0}" +
                        "При операции получения страниц произошла следующая ошибка:{0}" +
                        "Файл '{1}' не может быть открыт", Environment.NewLine, fileName));
                }

                pagesInfo = document.GetTFlexPagesInfo();
            }

            return pagesInfo;
        }
    }
}
