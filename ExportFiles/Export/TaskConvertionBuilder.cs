using DeveloperUtilsLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.References;
using TFlex.DOCs.Model.References.ConversionTaskQueue;
using TFlex.DOCs.Model.References.ConversionTaskQueue.FileConversionModule;
using TFlex.DOCs.Model.References.Files;
using TFlex.DOCs.Model.References.Nomenclature;
using TFlex.DOCs.Model.Search;

namespace ExportFiles.Export
{
    public class TaskConvertionBuilder
    {
        private class Guids
        {
            public class Parameters
            {
                public static readonly Guid nameOutputFile = new Guid("7edd141e-9d76-406b-9ddd-b5f4f00a3e31");
                public static readonly Guid nomenclature = new Guid("71d8d7eb-d4b8-4dc6-9f4e-5465027d31f7");
                public static readonly Guid connectionFileToNomenclature = new Guid("d61d35ea-bb05-4b8e-9c99-4354c2f0fa06");
                public static readonly Guid filePrototype = new Guid("bb956629-2962-417f-a008-c9d2cd5577a4");
                public static readonly Guid formatConversion = new Guid("72d85286-e161-42db-99cd-f532a7fb3bea");
            }

            public class Links
            {
                public static readonly Guid SourceFile = new Guid("d0899e97-9779-4d6b-afc6-1cf143b0fc6b");
                public static readonly Guid ModuleConversionFile = new Guid("9dd4aafa-6827-4d12-bbeb-125fc13b295f");
                public static readonly Guid FormatConvertation = new Guid("00d084d6-19f5-4d15-8b11-7d45ccb4ea15");
                public static readonly Guid FilesConversion = new Guid("6f8bb6e4-c452-4577-a372-acba10b8324e");
            }
        }
        private ServerConnection connection;
        private ConversionTaskQueueReferenceObject task;
        private ConversionTaskQueueReference queueReference;
        private FileConversionModuleReference fileConversionModuleReference;
        private Reference formatConvertationReference;

        private FileConversionModuleReferenceObject module;
        private FileObject sourceFile;
        private ReferenceObject formatConversion;
        private FileObject filePrototype;
        private String nameOutputFile;
        private NomenclatureObject nomenclatureObject;
        private bool connectFileToNomenclature;
        public TaskConvertionBuilder(ConversionTaskQueueReferenceObject taskConvertion)
        {
            this.task = taskConvertion;
            this.queueReference = taskConvertion.Reference;
            this.connection = taskConvertion.Reference.Connection;
            this.fileConversionModuleReference = new FileConversionModuleReference(connection);
            this.formatConvertationReference = new FileConversionModuleReference(connection);
        }

        public void Save(string str)
        {
            task.EndUpdate(str);
        }

        public void Edit()
        {
            task.StartUpdate();
        }

        public TaskConvertionBuilder SetModuleConvertation(string name)
        {
            this.module = fileConversionModuleReference.Find(
                Filter.Parse($"[Наименование] = '{name}'",
                fileConversionModuleReference.ParameterGroup)).FirstOrDefault() as FileConversionModuleReferenceObject;
            if(this.module == null)
            {
                throw new ArgumentNullException($"модуль конвертации {name} не найден");
            }
            return this;
        }

        public TaskConvertionBuilder SetFile(FileObject file)
        {
            this.sourceFile = file;
            return this;
        }

        public TaskConvertionBuilder SetFormatConvertation(string typeFile)
        {
            this.formatConversion = formatConvertationReference.Find(
                Filter.Parse($"[Наименование] = '{typeFile}'",
                formatConvertationReference.ParameterGroup)).FirstOrDefault();
            if(this.formatConversion == null)
            {
                throw new ArgumentException($"формат конвертации {typeFile} не найден");
            }
            return this;
        }

        public TaskConvertionBuilder SetFilePrototype(FileObject file)
        {
            this.filePrototype = file;
            return this;
        }

        public TaskConvertionBuilder SetNameOutputFile(string nameOutputFile)
        {
            this.nameOutputFile = nameOutputFile;
            return this;
        }

        public TaskConvertionBuilder SetNomenclature(NomenclatureObject nom)
        {
            this.nomenclatureObject = nom;
            return this;
        }

        public TaskConvertionBuilder ConnectFileToNomenclature(bool flag)
        {
            this.connectFileToNomenclature = flag;
            return this;
        }

        public ConversionTaskQueueReferenceObject Build()
        {
            this.task.Name.Value = sourceFile.Name;
            this.task.SetLinkedObject(Guids.Links.SourceFile, sourceFile);
            this.task.SetLinkedObject(Guids.Links.FormatConvertation, formatConversion);
            this.task[Guids.Parameters.nameOutputFile].Value = Path.GetFileNameWithoutExtension(sourceFile.Name);
            this.task[Guids.Parameters.nomenclature].Value = nomenclatureObject.Guid;
            this.task[Guids.Parameters.connectionFileToNomenclature].Value = connectFileToNomenclature;
            return this.task;
        }
    }
}
