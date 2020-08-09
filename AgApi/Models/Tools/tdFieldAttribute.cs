using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DB
{
    /// <summary>
    /// Define o nome fisico do campo, utilizar quando o nome da propriedade nao refletir o nome fisico do campo na tabela
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class tdFieldAttribute : Attribute
    {
        public String Name { get; set; }
    }
}