using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MapHelper
{
 
    public class ClassStructure
    {
        public string Name { get; set; }
        public List<StructureItem> Items { get; set; }
    }

    public class CollectionStructure
    {
        public string Type { get; set; }
        public int Count { get; set; }
        public List<StructureItem> Element { get; set; }
        public string FullType { get; set; }
    }

    public class StructureItem
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Type { get; set; }
        public string FullType { get; set; }
    }

    public class Mapping
    {
        public Type FromClass { get; set; }
        public Type ToClass { get; set; }
        public Dictionary<string, string> FromTo { get; set; }

        public Mapping (Type from, Type to)
        {
            FromClass = from;
            ToClass = to;
        }

    }

    public class Mapper
    {

        ///
        /// <summary>
        /// 
        /// o Mapper mapeia de uma estrutura plana (ou não) para uma estrutura que pode ser plana, o não plana.
        /// o objetivo inicial é permitir que a leitura de dados de um BD possa ser mapeado para
        /// objetos diversos. um possivel uso é utilizar estruturas complexas em propertygrid. clonar dados para objetos 
        /// de classes parecidas etc
        /// 
        //
        // copia campos mapeados
        //
        // Mapper map = new Mapper(typeof(RoboDemoProtocolo), typeof(RoboDemoPagamento));
        // map.AddItem("ID", "PagamentoId");
        // map.AddItem("MesAno", "MesAno");
        // map.AddItem("Login", "Login");
        // map.AddItem("PrestadorId", "OperadoraId");
        // RoboDemoPagamento dest = new RoboDemoPagamento();
        // RoboDemoProtocolo org = new RoboDemoProtocolo();
        // org.ID = 1234;
        // org.PrestadorId = 2345;
        // org.MesAno = "023000";
        // org.Login = "login anonimo";
        // RoboDemoPagamento xxx = map.Map<RoboDemoProtocolo, RoboDemoPagamento>(org, dest, true);
        //
        // copia os campos comuns ...
        //
        // Mapper map = new Mapper(typeof(RoboDemoProtocolo), typeof(RoboDemoPagamento));
        // RoboDemoProtocolo dest = new RoboDemoProtocolo();
        // RoboDemoProtocolo xxx = map.Map<RoboDemoPagamento, RoboDemoProtocolo>(RoboPagamento, dest, true);
        ///
        /// </summary>
        /// 

        public List<string> log = new List<string>();

        public Mapping mapping; // = null;

        public Mapper (Type from, Type to)
        {
            mapping = new Mapping(from, to);
            Dictionary<string, string> mappings = new Dictionary<string, string>();
            mapping.FromTo = mappings;
        }

        public void AddItem (string from, string to)
        {
            mapping.FromTo.Add(from, to);
        }

        public T Map<F,T>(F from, T to, bool createObj = false)
        {
            T destination = (T)Activator.CreateInstance(typeof(T));

            Type toProperties = typeof(T);
            Dictionary<String, PropertyInfo> toPropertiesDic = new Dictionary<string, PropertyInfo>();
            foreach (var prop in toProperties.GetProperties())
            {
                toPropertiesDic.Add(prop.Name, prop);
            }

            Type fromProperties = typeof(F);
            Dictionary<String, PropertyInfo> fromPropertiesDic = new Dictionary<string, PropertyInfo>();
            foreach (var prop in fromProperties.GetProperties())
            {
                fromPropertiesDic.Add(prop.Name, prop);
            }

            foreach (var prop in fromPropertiesDic)
            {
                try
                {
                    String FromProperty = prop.Key.ToString();

                    object FieldValue = from.GetType().GetProperty(FromProperty).GetValue(from, null);
                    try
                    {
                        string ToProperty = FromProperty; // mapear automaticamente.... sem nomes
                        if (mapping.FromTo.Count > 0)
                        {   
                            ToProperty = mapping.FromTo[FromProperty].ToString();
                        }

                        if (toPropertiesDic.ContainsKey(ToProperty)) // achou a propriedade
                        {
                            PropertyInfo propInfo = toPropertiesDic[ToProperty];
                            propInfo.SetValue(destination, isDBNull(FieldValue));
                        }
                    }
                    catch { }
                }
                catch { }
            }

            return destination;
        }

        private Object isDBNull(Object obj)
        {
            if (obj is DBNull)
                return null;
            else return obj;
        }

        public void testListProperties()
        {
            
            Mapping testeobj = new Mapping(typeof(Mapping), typeof(Mapping));
            log.Clear();

            GetProperties(testeobj, 0, "");

        }

        public bool isCollection(Type type)
        {
            bool result = false;
            string ptype = type.ToString();
            if (ptype.ToLower().Contains("collection")) { return true; }
            return result;
        }

        public CollectionStructure GetCollectionType(Type type)
        {
            CollectionStructure result = new CollectionStructure();
            result.Type = "";
            result.Count = 0;
            result.FullType = type.ToString();
            if (isCollection(type))
            {
                
                string ptype = type.ToString().Substring(0, type.ToString().Length-1); // remove last character ("]")
                result.Type = type.ToString().Substring(0, ptype.IndexOf("`")).Split('.').Last();
                result.Count = Convert.ToInt16(ptype.Substring(ptype.IndexOf("`") + 1).Split('[').First());

                List<StructureItem> Elements = new List<StructureItem>();
                result.Element = Elements;
                foreach (string s in ptype.Substring(ptype.IndexOf("`") + 1).Split('[').Last().Split(','))
                {
                    StructureItem itm = new StructureItem();
                    itm.Type = s.Split('.').Last();
                    itm.FullType = s;
                    Elements.Add(itm);
                }
            }
            return result;
        }

        private void GetProperties(object obj, int level, string PreviousClass)
        {
            if (level == 0)
            {

            }

            if (obj == null) return;

            // modificar oportunamente --------------------
            string indentString = new string(' ', level*3);
            // --------------------------------------------


            Type objType = obj.GetType();
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj, null);

                var elems = propValue as IList;
                if ((elems != null) && !(elems is string[]))
                {
                    foreach (var item in elems)
                    {
                        GetProperties(item, level + 3, PreviousClass+item+".");
                    }
                }
                else
                {
                    // This will not cut-off System.Collections because of the first check
                    if (property.PropertyType.Assembly == objType.Assembly)
                    {
                        string ptype = property.PropertyType.ToString();
                        if (ptype.ToLower().Contains("system.collections")) { ptype = "(Collection)" + ptype ; }

                        log.Add(String.Format("{0}{1}: ({2})", indentString, PreviousClass+property.Name, ptype));

                        GetProperties(propValue, level + 2, PreviousClass + property.Name + ".");
                    }
                    else
                    {
                        if (propValue is string[])
                        {
                            var str = new StringBuilder();
                            foreach (string item in (string[])propValue)
                            {
                                str.AppendFormat("{0}; ", item);
                            }
                            propValue = str.ToString();
                            str.Clear();
                        }

                        string ptype = property.PropertyType.ToString();
                        if (isCollection(property.PropertyType))
                        {
                            CollectionStructure collStrct = GetCollectionType(property.PropertyType);
                        }
                        
                        if (ptype.ToLower().Contains("system.collections"))
                        {
                            ptype = "(Collections)" + ptype;
                        }

                        log.Add(String.Format("{0}{1}: {2} ({3})", indentString, PreviousClass + property.Name, propValue, ptype));
                    }
                }
            }
        }

    }

}