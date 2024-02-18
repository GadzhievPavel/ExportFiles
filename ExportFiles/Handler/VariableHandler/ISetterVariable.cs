using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.References;
using TFlex.DOCs.Model.References.Files;
using TFlex.DOCs.Model.References.Nomenclature;

namespace ExportFiles.Handler
{
    public interface ISetterVariable
    {
        void SetVariables(FileObject file, NomenclatureObject nomenclature);

    }
}
