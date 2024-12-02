using ExportFiles.Handler.Exporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.FilePreview.CADService;

namespace ExportFiles.Handler.CadVariables
{
    internal interface IControllerVariables
    {
        //void SetVariables(DataVariables dataVariables, VariableCollection variables);
        DataVariableCad GetDataVariableCad();
    }
}
