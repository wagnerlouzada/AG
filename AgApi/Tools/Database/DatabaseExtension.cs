using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DB
{
    public static class DatabaseExtension
    {

        private static Object isDBNull(this Object obj)
        {
            try
            {
                if (obj is DBNull)
                    return null;
                else return obj;
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// Encapsula a execucao de um DataReader retornando um enumerator do tipo passado como parametro
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="cmd"></param>
        /// <param name="parametros"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExecuteQuery<T>(this Database db, String cmd, Dictionary<String, Object> parametros = null) where T : class, new()
        {
            db.CommandText = cmd;
            db.CommandType = CommandType.Text;
            // Melhorias

            // outra forma de atribuir parametros 
            // capturar todos os parametros do SQL com @XXXX
            // e passar os parametros na ordem em que encontrar o parametro!

            // outra forma é substituir os "{0}, {1}" ... por @Par0 e atribuir o parametro do indice 0 a query
            if (parametros!=null)
            {
                db.ClearParameters();
                foreach(var kv in parametros)
                {
                    db.CreateParameter(kv.Key, kv.Value);   
                }
            }

            Type tModelType = typeof(T);
            Dictionary<String, PropertyInfo> dicProperty = new Dictionary<string, PropertyInfo>();
            foreach (var prop in tModelType.GetProperties())
            {
                String nome = "";
                var attr = prop.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(tdFieldAttribute));
                if (attr != null)
                    nome = (attr as tdFieldAttribute).Name;
                else
                    nome = prop.Name;

                dicProperty.Add(nome.ToLower(), prop);
            }

            List<T> result = new List<T>();
            using (IDataReader dr = db.ExecuteDataReader(CommandBehavior.CloseConnection))
            {
                while (dr.Read())
                {
                    T rec = new T();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        String nome = dr.GetName(i).ToLower();
                        if (dicProperty.ContainsKey(nome)) // achou a propriedade
                        {
                            PropertyInfo propInfo = dicProperty[nome];
                            //try// foi incluido para tentar fazer passar....precisa testar se o tipo pode ter null
                            //{
                                propInfo.SetValue(rec, dr.GetValue(i).isDBNull());
                        //}
                        //    catch (Exception ex) { }
                    }
                    }
                    result.Add(rec);
                }
            }

            return result;
        }

    }

}