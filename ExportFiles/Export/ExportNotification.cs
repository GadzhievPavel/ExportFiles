using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.References.Modifications;
using TFlex.DOCs.Model.References.Nomenclature;
using TFlex.DOCs.Model.References.Nomenclature.ModificationNotices;

namespace ExportFiles.Export
{
    public class ExportNotification : ExportNomenclature
    {
        public ExportNotification(ServerConnection serverConnection) : base(serverConnection)
        {

        }

        public void AddNotification(ModificationNoticeReferenceObject notice)
        {
            var changes = notice.GetObjects(ModificationReferenceObject.RelationKeys.ModificationNotice);
            foreach (var change in changes)
            {
                var pdmObject = change.GetObject(ModificationReferenceObject.RelationKeys.PDMObject);
                AddNomenclature(pdmObject as NomenclatureObject);
            }
        }


    }
}
