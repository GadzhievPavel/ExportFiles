using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.FilePreview.CADService;
using TFlex.DOCs.Model.References.Nomenclature;

namespace ExportFiles.Handler.Exporter
{
    public class CadVariablesWriter
    {
        /// <summary>
        /// Переменные CAD модели
        /// </summary>
        private VariableCollection variables;
        /// <summary>
        /// Набор переменных для записи данных в CAD модель
        /// </summary>
        private DataVariableCad variableCad;
        private bool needSave;
        public CadVariablesWriter(CadDocument cadDocument, DataVariableCad variableCad, bool needSave)
        {
            variables = cadDocument.GetVariables();
            this.variableCad = variableCad;
        }
        /// <summary>
        /// Метод для записи данных в CAD файл
        /// </summary>
        public void WriteValues()
        {
            foreach (var key in variableCad)
            {
                var val = variableCad[key];
                TrySetVariableValue(key, val);
            }
            if (needSave)
            {
                variables.Save();
            }
        }
        /// <summary>
        /// Метод записи данных в переменную
        /// </summary>
        /// <param name="variableName">имя переменной</param>
        /// <param name="value">значение</param>
        /// <returns></returns>
        private bool TrySetVariableValue(string variableName, object value)
        {
            var foundedVarrible = variables.FirstOrDefault(v => v.Name == variableName);
            if (foundedVarrible is null)
            {
                return false;
            }
            try
            {
                foundedVarrible.Expression = "\"" + value.ToString() + "\"";
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
