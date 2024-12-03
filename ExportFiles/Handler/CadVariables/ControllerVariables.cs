using ExportFiles.Exception;
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
using NomenclatureExtensionLibray;

namespace ExportFiles.Handler.CadVariables
{
    /// <summary>
    /// Класс для установки значений в переменные в GRB файлы
    /// </summary>
    public class ControllerVariables : IControllerVariables
    {

        private DataVariables data;

        public ControllerVariables(DataVariables data)
        {
            this.data = data;
        }

        private DataVariableCad MakeSignatures(ref DataVariableCad dataCad, SignatureCollection signatures)
        {
            SignaturaVariable(ref dataCad, signatures, 3, "$Разработал", "$Дата_разраб");
            SignaturaVariable(ref dataCad, signatures, 2, "$Проверил", "$Дата_пров");
            SignaturaVariable(ref dataCad, signatures, 1, "$Н_контр", "$Дата_н_контр");
            SignaturaVariable(ref dataCad, signatures, 6, "$Утвердил", "$Дата_утв");
            SignaturaVariable(ref dataCad, signatures, 5, "$Т_контр", "$Дата_т_контр");
            return dataCad;
        }

        private DataVariableCad SignaturaVariable(ref DataVariableCad dataCad, SignatureCollection signatures, int id, string varShortName, string varDate)
        {
            var signature = signatures.FirstOrDefault(s => s.SignatureObjectType.Id == id);
            if (signature == null)
            {
                return dataCad;
            }

            var shortName = (signature.UserObject as User).ShortName;
            var date = signature.SignatureDate.Value.ToString("d.MM.yy");

            dataCad.Add(new CadVariable { Key = varShortName, Value = shortName });
            dataCad.Add(new CadVariable { Key = varDate, Value = date });
            return dataCad;
        }

        private DataVariableCad NotificationVariable(ref DataVariableCad dataCad)
        {
            dataCad.Add("$ii", data.notice[Guids.NoticeModificationReference.Parameter.ОбозначениеИзвещенияОбИзменения].GetString());
            var changes = data.notice.GetObjects(Guids.NoticeModificationReference.Links.ИзвещенияИзменения);
            foreach (var change in changes)
            {
                var nomenclature = change.GetObject(Guids.ChangeReference.Links.ОбъектЭСИ) as NomenclatureObject;
                if (nomenclature is null)
                {
                    break;
                }
                var doc = nomenclature.LinkedObject as EngineeringDocumentObject;
                if (doc is null)
                {
                    break;
                }
                if (doc.GetFiles().Contains(data.fileObject))
                {
                    dataCad.Add("$izm", change[Guids.ChangeReference.Parameter.НомерИзменения].GetString());
                }
                dataCad.Add("$flag_new", change[Guids.ChangeReference.Parameter.ПараметрНовый].GetBoolean());
            }
            return dataCad;
        }

        private DataVariableCad SetBaseInfoVariables(ref DataVariableCad dataCad)
        {
            if (data.GetNomenclature().Class.IsAssembly)
            {
                dataCad.Add("$Vid_Chert", "Сборочный чертеж");
            }
            dataCad.Add("$Наименование", data.GetNomenclature().Name);

            if (data.GetNomenclature().HaveGroupDrawing())
            {
                var groupDrawing = new GroupDrawing(data.GetNomenclature().GetGroupDrawing());
                dataCad.Add("$Обозначение", groupDrawing.getDenotationBaseVariant());
            }
            else
            {
                dataCad.Add("$Обозначение", data.GetNomenclature().Denotation);
            }

            var document = data.GetNomenclature().LinkedObject as EngineeringDocumentObject;
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
                return dataCad;
            }

            dataCad.Add("$Материал2", material[Guids.MaterialReference.Parameter.СводноеНаименование].GetString());

            return dataCad;
        }

        public DataVariableCad GetDataVariableCad()
        {
            var dataCad = new DataVariableCad();
            var signatures = data.fileObject.Signatures;
            MakeSignatures(ref dataCad, signatures);

            if (data.notice != null)
            {
                NotificationVariable(ref dataCad);
            }
            SetBaseInfoVariables(ref dataCad);

            return dataCad;
        }
    }
}
