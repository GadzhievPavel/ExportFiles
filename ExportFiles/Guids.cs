using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Класс с набором Guid
/// </summary>
public static class Guids
{
    /// <summary>
    /// Guid-ы справочника "Документы"
    /// </summary>
    public static class DocumentsReference
    {
        /// <summary>
        /// Типы справочника "Документы"
        /// </summary>
        public static class Classes
        {
            /// <summary>
            /// Тип "Объект состава изделия"
            /// </summary>
            public static readonly Guid objectProductComposition = new Guid("f89e9648-c8a0-43f8-82bb-015cfe1486a4");
        }

    }

    public static class ChangeReference
    {
        /// <summary> Guid справочника 'Изменения' </summary>
        public static readonly Guid Изменения = new Guid("c9a4bb1b-cacb-4f2d-b61a-265f1bfc7fb9");

        public static class Links
        {
            /// <summary> Guid связи 'Рабочие Файлы' справочника 'Файлы' </summary>
            public static readonly Guid ИзмененияРабочиеФайлы = new Guid("6b65a575-3ca4-4fb0-9bfc-4d1655c2d83e");
        }
    }
}