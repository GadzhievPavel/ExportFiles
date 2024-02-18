using ExportFiles.Exception;
using ExportFiles.Exception.FileException;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.Classes;
using TFlex.DOCs.Model.FilePreview.CADService;
using TFlex.DOCs.Model.FilePreview.CADService.TFlexCadDocument;
using TFlex.DOCs.Model.Macros;
using TFlex.DOCs.Model.References;
using TFlex.DOCs.Model.References.Documents;
using TFlex.DOCs.Model.References.Files;
using TFlex.DOCs.Model.References.Nomenclature;

namespace ExportFiles
{
    public class ExportFilesGrbToTif
    {
        private static readonly string _tempFolder = Path.Combine(Path.GetTempPath(), "Temp DOCs", "ExportGRB");
        private CadDocumentProvider provider;
        private string _extensionDoc = "tif";
        private int _resolution = 300;
        private bool _exportToNewFile = true;

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
            this.classObjectProductComposition = documentReference.Classes.Find(Guids.DocumentsReference.Classes.objectProductComposition);
            this.provider = CadDocumentProvider.Connect(connection, ".grb");
        }


        /// <summary>
        /// Задать файл
        /// </summary>
        /// <param name="file"></param>
        private void SetFileObject(FileObject file)
        {
            fileObject = file;
        }

        /// <summary>
        /// Задать номенклатуру
        /// </summary>
        /// <param name="nomenclature"></param>
        private void SetNomenclature(NomenclatureObject nomenclature)
        {
            this.nomenclature = nomenclature;
        }

        /// <summary>
        /// Экспортировать файл
        /// </summary>
        /// <param name="fileObject">файл grb</param>
        /// <param name="isNewFile">создать новый подлинник</param>
        /// <returns>новый подлинник</returns>
        private FileObject ExportToFormat(bool isNewFile)
        {
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
            catch
            {
                
            }
            finally
            {
                ClearTemp();
            }
            
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

                //получаем переменные документа CAD
                var varribles = document.GetVariables();

                //Записываем в переменные данные о подписях объекта
                SetSignaturesVarribles(fileObject.Signatures, varribles);
                SetBaseInfoVariables(varribles, fileObject, nomenclature as NomenclatureObject);
                //Сохраняем коллекцию переменных
                varribles.Save();

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
