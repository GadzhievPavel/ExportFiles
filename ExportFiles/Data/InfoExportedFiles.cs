using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.References.Documents;
using TFlex.DOCs.Model.References.Files;
using TFlex.DOCs.Model.References.Nomenclature;
using TFlex.DOCs.References.TypesNomenclatureForConvertation;

namespace ExportFiles.Data
{
    public class InfoExportedFiles : IEnumerable<InfoExportedFile>
    {
        private List<InfoExportedFile> infos;
        private TypesNomenclatureForConvertationReference typesNomenclatureForConvertationReference;
        private List<TypesNomenclatureForConvertationReferenceObject> types;

        public InfoExportedFiles(ServerConnection serverConnection)
        {
            this.infos = new List<InfoExportedFile>();
            this.typesNomenclatureForConvertationReference = new TypesNomenclatureForConvertationReference(serverConnection);
            types = new List<TypesNomenclatureForConvertationReferenceObject>();
            this.types = typesNomenclatureForConvertationReference.Objects.Cast<TypesNomenclatureForConvertationReferenceObject>().ToList();
        }

        public IEnumerator<InfoExportedFile> GetEnumerator()
        {
            return infos.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(FileObject file, NomenclatureObject nomenclature)
        {
            var info = new InfoExportedFile() { file = file, nomenclature = nomenclature };
            var documents = file.GetObjects(EngineeringDocumentFields.File);
            info.linkedDocuments = new HashSet<EngineeringDocumentObject>(documents.Cast<EngineeringDocumentObject>());
            infos.Add(info);
        }

        public void Add(InfoExportedFile info)
        {
            infos.Add(info);
        }

        private bool isEnable(NomenclatureObject nom)
        {
            return types.Any(o => o.ClassGuid.Equals(nom.Class.Guid));
        }
    }
}
