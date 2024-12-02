﻿using ExportFiles.Exception;
using ExportFiles.Exception.FileException;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.FilePreview.CADService;
using TFlex.DOCs.Model.References.Documents;
using TFlex.DOCs.Model.References;
using TFlex.DOCs.Model.References.Files;
using TFlex.DOCs.Model.References.Nomenclature;
using TFlex.DOCs.Model.Signatures;
using ExportFiles.Handler.Model;
using TFlex.DOCs.Model.References.Users;
using ExportFiles.Handler.Exporter;

namespace ExportFiles.Handler.CadVariables
{
    /// <summary>
    /// Класс для установки значений в переменные в GRB файлы
    /// </summary>
    public class ControllerVariables : IControllerVariables
    {
        /// <summary>
        /// Провайдер для доступа к содержимому GRB файла
        /// </summary>
        private CadDocumentProvider provider;
        /// <summary>
        /// Подключение к серверу
        /// </summary>
        private ServerConnection connection;
        /// <summary>
        /// Формат GRB файла
        /// </summary>
        private string extension = ".grb";

        private DataVariables data;
        public ControllerVariables(ServerConnection connection)
        {
            this.connection = connection;
            provider = CadDocumentProvider.Connect(connection, extension);
        }

        public ControllerVariables(DataVariables data)
        {
            this.data = data;
        }

        /// <summary>
        /// Заполнение основной надписи чертежа (Наименование/Обозначение/Основной материал(если есть))
        /// </summary>
        /// <param name="variables"></param>
        /// <param name="file">Оригинал grb</param>
        /// <param name="nom">номенклатура</param>
        private void SetBaseInfoVariables(VariableCollection variables, FileObject file, NomenclatureObject nom)
        {
            if (nom.Class.IsAssembly)
            {
                TrySetVarribleValue(variables, "$Vid_Chert", "Сборочный чертеж");
            }
            TrySetVarribleValue(variables, "$Наименование", nom.Name);

            var nomWrap = new At3bootNomenclatureObject(nom);
            if (nomWrap.haveGroupDrawing())
            {
                var groupDrawing = nomWrap.getGroupDrawing();
                TrySetVarribleValue(variables, "$Обозначение", groupDrawing.getDenotationBaseVariant());
            }
            else
            {
                TrySetVarribleValue(variables, "$Обозначение", nom.Denotation);
            }


            var nomenclature = (NomenclatureObject)nom;
            var document = nomenclature.LinkedObject as EngineeringDocumentObject;

            ReferenceObject material = null;
            try
            {
                document.TryGetObject(Guids.DocumentsReference.Links.ОсновнойМатериал, out material);
            }
            catch
            {

            }

            if (material is null)
            {
                return;
            }
            var materialName = material[Guids.MaterialReference.Parameter.СводноеНаименование].GetString();

            TrySetVarribleValue(variables, "$Материал2", materialName);
        }

        /// <summary>
        /// Установка в основную надпись параметров на основе ИИ
        /// </summary>
        /// <param name="variables"></param>
        /// <param name="file">оригинал GRB</param>
        /// <param name="notification">ИИ</param>
        private void SetNotificationVariables(VariableCollection variables, FileObject file, ReferenceObject notification)
        {
            if (notification.Equals(null))
            {
                return;
            }
            TrySetVarribleValue(variables, "$ii", notification[Guids.NoticeModificationReference.Parameter.ОбозначениеИзвещенияОбИзменения].GetString());

            var changes = notification.GetObjects(Guids.NoticeModificationReference.Links.ИзвещенияИзменения);

            foreach (var change in changes)
            {
                var nomenclature = change.GetObject(Guids.ChangeReference.Links.ОбъектЭСИ) as NomenclatureObject;
                if (nomenclature is null)
                {
                    return;
                }
                var doc = nomenclature.LinkedObject as EngineeringDocumentObject;
                if (doc is null)
                {
                    return;
                }
                if (doc.GetFiles().Contains(file))
                {
                    TrySetVarribleValue(variables, "$izm", change[Guids.ChangeReference.Parameter.НомерИзменения].GetString());
                }

                var isNew = change[Guids.ChangeReference.Parameter.ПараметрНовый].GetBoolean();
                TrySetVarribleValue(variables, "$flag_new", isNew);

            }

            //var signDocumentoved = file.Signatures.Where(sign => sign.SignatureObjectType.Id == 25).FirstOrDefault();
            //if (signDocumentoved is null)
            //{
            //    return;
            //}
            //TrySetVarribleValue(variables, "$d_doc", signDocumentoved.SignatureDate.Value.ToString("d.MM.yy"));
        }

        /// <summary>
        /// Установка подписей
        /// </summary>
        /// <param name="signatures"></param>
        /// <param name="variables"></param>
        private void SetSignaturesVariables(SignatureCollection signatures, VariableCollection variables)
        {
            SetSignatureVarrible(signatures, variables, "$Разработал", "$Дата_разраб", 3);//Разработал
            SetSignatureVarrible(signatures, variables, "$Проверил", "$Дата_пров", 2);//Проверил
            SetSignatureVarrible(signatures, variables, "$Н_контр", "$Дата_н_контр", 1);//Нормоконтроль
            SetSignatureVarrible(signatures, variables, "$Утвердил", "$Дата_утв", 6);//Утвердил
            SetSignatureVarrible(signatures, variables, "$Т_контр", "$Дата_т_контр", 5);//Т.контр
        }

        /// <summary>
        /// Установка одной подписи
        /// </summary>
        /// <param name="signatures">Коллекция подписей Doc's</param>
        /// <param name="varribles"></param>
        /// <param name="userVarribleName">имя пользователя</param>
        /// <param name="dataVarrivbleName">дата подписи</param>
        /// <param name="signatureId">id типа подписи</param>
        private void SetSignatureVarrible(SignatureCollection signatures, VariableCollection varribles, string userVarribleName, string dataVarrivbleName, int signatureId)
        {
            //Находим подпись с указанным идентификатором
            var userSignature = signatures.FirstOrDefault(s => s.SignatureObjectType.Id == signatureId);
            if (userSignature is null)
                return;

            var user = userSignature.UserObject as User;
            TrySetVarribleValue(varribles, userVarribleName, user.ShortName);
            TrySetVarribleValue(varribles, dataVarrivbleName, userSignature.SignatureDate.Value.ToString("d.MM.yy"));
        }

        /// <summary>
        /// Метод для установки значения в указанную переменную
        /// </summary>
        /// <param name="varribles"></param>
        /// <param name="varribleName">наименование переменной в GRB</param>
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



        public void SetVariables(DataVariables dataVariables, VariableCollection variables)
        {
            SetSignaturesVariables(dataVariables.GetFileObject().Signatures, variables);
            if (dataVariables.GetNotice() != null)
            {
                SetNotificationVariables(variables, dataVariables.GetFileObject(), dataVariables.GetNotice());
            }
            SetBaseInfoVariables(variables, dataVariables.GetFileObject(), dataVariables.GetNomenclature());
            variables.Save();
        }

        private List<CadVariable> makeSignature(SignatureCollection signatures)
        {

        }

        private DataVariableCad GetDataVariableCad()
        {
            var dataCad = new DataVariableCad();
            var signatures = data.fileObject.Signatures;

        }
    }
}
