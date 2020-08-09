using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class DbFilter
    {
        public String FieldName { get; set; }
        public DB.DbOperator Operator { get; set; }
        public Object Value { get; set; }
    }
}
