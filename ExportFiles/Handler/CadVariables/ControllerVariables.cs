using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.FilePreview.CADService;
using TFlex.DOCs.Model.References.Files;

namespace ExportFiles.Handler.CadVariables
{
    public class ControllerVariables
    {
        private CadDocumentProvider provider;
        private ServerConnection connection;
        private string extension = ".grb";

        public ControllerVariables(ServerConnection connection)
        {
            this.connection = connection;
            provider = CadDocumentProvider.Connect(connection, extension);
        }

        public void SetVarriables(FileObject file)
        {

        }

    }
}
