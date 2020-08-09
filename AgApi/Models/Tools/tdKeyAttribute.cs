using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    /// <summary>
    /// Define os campos que fazem parte da chave
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    class tdKeyAttribute : Attribute
    {
        /// <summary>
        /// Define se o campo faz parte da chave da tabela
        /// </summary>
        public bool Key { get; set; } = false;
        /// <summary>
        /// Define a ordem quando mais que um campo faz parte da tabela
        /// </summary>
        public Int32 Order { get; set; } = 0;
    }
}
