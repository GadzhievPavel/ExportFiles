using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.Classes;
using TFlex.DOCs.Model.FilePreview.CADService;
using TFlex.DOCs.Model.References.Documents;
using TFlex.DOCs.Model.References.Files;
using TFlex.DOCs.Model.References.Nomenclature;
using TFlex.DOCs.Model.Signatures;

namespace ExportFiles.Handler
{
    public class FileVariables
    {
        private FileObject file;
        private NomenclatureObject nomenclature;
        private CadDocumentProvider provider;
        private SignatureCollection signatureCollection;

        private HashSet<ClassObject> excludedClassesObjects;

        public FileVariables(FileObject file, NomenclatureObject nomenclature, CadDocumentProvider provider, HashSet<ClassObject> excludedClassesObjects)
        {
            this.file = file;
            this.nomenclature = nomenclature;
            this.provider = provider;
            this.signatureCollection = file.Signatures;
            this.excludedClassesObjects = excludedClassesObjects;
        }

        public FileVariables(FileObject file, NomenclatureObject nomenclature, CadDocumentProvider provider, List<ClassObject> excludedClassesObjects)
        {
            this.file = file;
            this.nomenclature = nomenclature;
            this.provider = provider;
            this.signatureCollection = file.Signatures;
            this.excludedClassesObjects = new HashSet<ClassObject>();
            foreach(var clOb in excludedClassesObjects)
            {
                this.excludedClassesObjects.Add(clOb);
            }
        }

        public void FillVariables()
        {
            var path = file.LocalPath;
            using (var document = provider.OpenDocument(path, true))
            {
                //получаем переменные документа CAD
                var varribles = document.GetVariables();
                SetSignaturesVarribles(signatureCollection, varribles);

            }
        }
        
        /// <summary>
        /// Установка подписей на чертеж
        /// </summary>
        /// <param name="signatures">Коллекция подписей</param>
        /// <param name="varribles">Коллекция переменных grb файла</param>
        private void SetSignaturesVarribles(SignatureCollection signatures, VariableCollection varribles)
        {
            SetSignatureVarrible(signatures, varribles, "$Разработал", "$Дата_разраб", 3);//Разработал
            SetSignatureVarrible(signatures, varribles, "$Проверил", "$Дата_пров", 2);//Проверил
            SetSignatureVarrible(signatures, varribles, "$Н_контр", "$Дата_н_контр", 1);//Нормоконтроль
            SetSignatureVarrible(signatures, varribles, "$Утвердил", "$Дата_утв", 6);//Утвердил
            SetSignatureVarrible(signatures, varribles, "$Т_контр", "$Дата_т_контр", 5);//Т.контр
        }

        /// <summary>
        /// Установка подписей на чертеж
        /// </summary>
        /// <param name="signatures">Коллекция подписей</param>
        /// <param name="varribles">Переменные grb файла</param>
        /// <param name="userVarribleName">Имя пользователя</param>
        /// <param name="dataVarrivbleName">Дата подписи</param>
        /// <param name="signatureId">ID подписи</param>
        private void SetSignatureVarrible(SignatureCollection signatures, VariableCollection varribles, string userVarribleName, string dataVarrivbleName, int signatureId)
        {
            //Находим подпись с указанным идентификатором
            var userSignature = signatures.FirstOrDefault(s => s.SignatureObjectType.Id == signatureId);
            if (userSignature is null)
                return;

            TrySetVarribleValue(varribles, userVarribleName, userSignature.UserName);
            TrySetVarribleValue(varribles, dataVarrivbleName, userSignature.SignatureDate.Value.ToString("d.MM.yy"));
        }

        /// <summary>
        /// Заполнение переменных в основной надписи обозначение, наименование и пр. 
        /// </summary>
        /// <param name="variables"></param>
        /// <param name="files"></param>
        private void SetBaseInfoVariables(VariableCollection variables, FileObject file, NomenclatureObject nom)
        {
            TrySetVarribleValue(variables, "$Наименование", nom.Name);
            TrySetVarribleValue(variables, "$Обозначение", nom.Denotation);
            var nomenclature = (NomenclatureObject)nom;
            var document = nomenclature.LinkedObject as EngineeringDocumentObject;
            if (document is null)
            {
                return;
            }
            //if (!document.Class.IsInherit(_classObjectStructProduct))
            //{
            //    return;
            //}
            if (document.Class.Guid.Equals(new Guid("582dad76-1b07-4c4b-b97d-cc89b0149aa6")) ||
                document.Class.Guid.Equals(new Guid("83e1ef55-0658-4e3e-afeb-d8fceee3c86d")) ||
                document.Class.Guid.Equals(new Guid("0d6ff46a-e8a1-4485-98cd-0955a1b54a4d"))
                )
            {
                return;
            }
            //var material = document.GetObject(Guids.Links.ОсновнойМатериал);
            //if (material is null)
            //{
            //    return;
            //}
            //var materialName = material[Guids.Parameters.НазваниеОписаниеМатериала].GetString();
            //TrySetVarribleValue(variables, "$Материал2", materialName);
        }


        /// <summary>
        /// Устанавливаем значение в переменную
        /// </summary>
        /// <param name="varribles"></param>
        /// <param name="varribleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool TrySetVarribleValue(VariableCollection varribles, string varribleName, object value)
        {
            //Ищем переменную по наименованию
            var foundedVarrible = varribles.FirstOrDefault(v => v.Name == varribleName);
            if (foundedVarrible is null)
                return false;

            try
            {
                //Записываем в переменную значение выражения
                foundedVarrible.Expression = "\"" + value.ToString() + "\"";
                return true;
            }
            catch
            {
                return false;
            }
        }
    }


}
