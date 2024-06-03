using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.References.Nomenclature;

namespace ExportFiles.Handler.Model
{
    public class GroupDrawing
    {
        private NomenclatureObject nom;

        public GroupDrawing(NomenclatureObject nom)
        {
            if(!(nom.Class.IsDrawing || nom.Class.Guid.Equals(Guids.NomenclatureReference.TypeAssemblyDrawing)))
            {
                throw new System.Exception($"номенклатура {nom} не явдяется групповым чертежом");
            }
            this.nom = nom;
            
        }

        public string getDenotationBaseVariant()
        {
            return nom.Denotation.Value.Replace("СБ", "").Trim();
        }
    }
}
