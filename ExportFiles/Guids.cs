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
    }

