using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.FilePreview.CADService;

namespace ExportFiles.Handler.VariableHandler
{
    /// <summary>
    /// Класс для установки значения в переменные файла grb
    /// </summary>
    public class SetVariablesFile
    {

        /// <summary>
        /// Устанавливаем значение в переменную
        /// </summary>
        /// <param name="varribles"></param>
        /// <param name="varribleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool TrySetVarribleValue(VariableCollection varribles, string varribleName, object value)
        {
            //Ищем переменную по наименованию
            var foundedVarrible = varribles.FirstOrDefault(v => v.Name == varribleName);
            if (foundedVarrible is null)
                return false;

            try
            {
                //Записываем в переменную значение выражения
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
