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
    /// <summary>
    /// Класс обертка Номенклатуры PDM 
    /// </summary>
    public class At3bootNomenclatureObject
    {
        private NomenclatureObject nom;
        public At3bootNomenclatureObject(NomenclatureObject nom)
        {
            this.nom = nom;
        }

        /// <summary>
        /// Имеет групповой чертеж
        /// </summary>
        /// <returns></returns>
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

            var findedGroupDrawings = getAllVariantsWhitGroupDrawings(nomLinkedGrbFiles, versions);

            var listGroupNom = findedGroupDrawings.Where(n => n.Class.Equals(nom.Class)).ToList();

            return listGroupNom.Count > 1;
        }

        /// <summary>
        /// Вернуть все исполнения связанные групповым чертежом
        /// </summary>
        /// <param name="versions"></param>
        /// <param name="groupNomenclature"></param>
        /// <returns></returns>
        private List<NomenclatureObject> getAllVariantsWhitGroupDrawings(List<NomenclatureObject> versions, List<NomenclatureObject> groupNomenclature)
        {
            List<NomenclatureObject> nomList = new List<NomenclatureObject>();
            foreach (var nom in groupNomenclature)
            {
                if (versions.Contains(nom))
                {
                    nomList.Add(nom);
                }
            }
            return nomList.Any() ? nomList : null;
        }

        /// <summary>
        /// Возвращает групповой чертеж
        /// </summary>
        /// <returns></returns>
        public GroupDrawing getGroupDrawing()
        {
            var versions = nom.GetVersions();
            var selectDoc = nom.LinkedObject as EngineeringDocumentObject;
            var selectGrbFiles = selectDoc.GetFiles().Where(file => file.Class.Extension.ToLower().Equals("grb")).FirstOrDefault();
            var nomLinkedGrbFiles = findNomenclatureByFile(selectGrbFiles);

            var findedGroupDrawings = getAllVariantsWhitGroupDrawings(nomLinkedGrbFiles, versions);
            var drawing = findedGroupDrawings.Where(n => n.Class.IsDrawing || n.Class.Guid.Equals(Guids.NomenclatureReference.TypeAssemblyDrawing)).FirstOrDefault();
            return new GroupDrawing(drawing);

        }
        /// <summary>
        /// Поиск номенклатуры по прикрепленному файлу
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public List<NomenclatureObject> findNomenclatureByFile(FileObject file)
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
