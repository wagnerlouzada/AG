using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    /// <summary>
    /// Define o nome fisico da tabela
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    class tdTableAttribute : Attribute
    {
        public String Name { get; set; }
    }
}
