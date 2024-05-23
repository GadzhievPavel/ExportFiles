using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.References;
using TFlex.DOCs.Model.References.Documents;
using TFlex.DOCs.Model.References.Files;
using TFlex.DOCs.Model.References.Nomenclature;

namespace ExportFiles.Handler.Model
{
    public class At3bootNomenclatureObject
    {
        private NomenclatureObject nom;
        public At3bootNomenclatureObject(NomenclatureObject nom)
        {
            this.nom = nom;
        }

        public bool haveGroupDrawing()
        {
            var versions = nom.GetVersions();

            var selectDoc = nom.LinkedObject as EngineeringDocumentObject;
            var selectGrbFiles = selectDoc.GetFiles().Where(file => file.Class.Extension.ToLower().Equals("grb")).FirstOrDefault();

            if (selectGrbFiles is null)
            {
                return false;
            }

            var nomLinkedGrbFiles = findNomenclatureByFile(selectGrbFiles);

            var findedGroupDrawings = getGroupDrawings(nomLinkedGrbFiles, versions);

            var baseVersion = nom.BaseVersion;

            return findedGroupDrawings.Contains(baseVersion);
        }

        private List<NomenclatureObject> getGroupDrawings(List<NomenclatureObject> versions, List<NomenclatureObject> groupNomenclature)
        {
            List<NomenclatureObject> nomList = new List<NomenclatureObject>();
            foreach(var nom in groupNomenclature)
            {
                if (versions.Contains(nom))
                {
                    nomList.Add(nom);
                }
            }
            return nomList.Any()? nomList: null;
        }

        private List<NomenclatureObject> findNomenclatureByFile(FileObject file)
        {
            List<NomenclatureObject> nomenclatures = new List<NomenclatureObject>();
            var documents = file.GetObjects(FileObject.RelationKeys.Document);
            foreach (var document in documents)
            {
                var doc = document as EngineeringDocumentObject;
                nomenclatures.Add(doc.GetLinkedNomenclatureObject());
            }

            return nomenclatures;
        }
    }
}
