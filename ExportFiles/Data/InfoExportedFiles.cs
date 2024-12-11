using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.References;
using TFlex.DOCs.Model.References.Documents;
using TFlex.DOCs.Model.References.Files;
using TFlex.DOCs.Model.References.Nomenclature;

namespace ExportFiles.Data
{
    public class InfoExportedFiles : IEnumerable<InfoExportedFile>
    {
        private List<InfoExportedFile> infos;
        private Reference typesNomenclatureForConvertationReference;
        private List<ReferenceObject> types;

        private readonly Guid guidReferenceTypesNomenclature = new Guid("fcf00da8-06aa-4c57-86a9-522ef0a752b0");
        private readonly Guid paramGuidClassNomenclature = new Guid("44bf14f7-4672-4336-bc61-759a8ace5908");
        public InfoExportedFiles(ServerConnection serverConnection)
        {
            this.infos = new List<InfoExportedFile>();
            this.typesNomenclatureForConvertationReference = serverConnection.ReferenceCatalog.Find(guidReferenceTypesNomenclature).CreateReference();
            types = new List<ReferenceObject>();
            this.types = typesNomenclatureForConvertationReference.Objects.Cast<ReferenceObject>().ToList();
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
            if (isEnable(nomenclature))
            {
                var info = new InfoExportedFile() { file = file, nomenclature = nomenclature };
                var documents = file.GetObjects(EngineeringDocumentFields.File);
                info.linkedDocuments = new HashSet<EngineeringDocumentObject>(documents.Cast<EngineeringDocumentObject>());
                infos.Add(info);
            }
        }

        public void Add(InfoExportedFile info)
        {
            if (isEnable(info.nomenclature))
            {
                infos.Add(info);
            }
        }

        private bool isEnable(NomenclatureObject nom)
        {
            return types.Any(o => o[paramGuidClassNomenclature].Value.Equals(nom.Class.Guid));
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            types.ForEach(o => { sb.Append($"{o.ToString()} "); });
            return sb.ToString();
        }
    }
}
