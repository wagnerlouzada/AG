using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using MapHelper;
using AppV;
using System.IO;
using CustomExtensions;

namespace DB
{

    #region FULL VERSION

    public enum DbTopMode
    {
        Tuplas,
        Percent
    }

    public enum DbOperator
    {
        Equal,
        Diferent,
        Greater,
        Less,
        GreaterEqual,
        LessEqual,
        Match,
        Like,
        Contain,
        Is,
        IsNot
    }

    public enum DbAgregate
    {
        None,
        Sum,
        Count,
        Max,
        Avg
    }

    public enum DbJoinType
    {
        Join,
        Inner,
        Outer,
        RightOuter,
        LeftOuter,
        FullOuter,
        Cross
    }

    public class DbTable
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string PK { get; set; }

        public string Table { get; set; }
    }

    public class DbJoin
    {
        public DbJoinType JoinType { get; set; }
        public string PkTable { get; set; } // alias
        public string FkTable { get; set; } // alias
        public string JoinField { get; set; } // the FK field, pk field is at Table

        public string PkField { get; set; }
        public string Join { get; set; } // para conter o texto INNER, OUTER... etc

        public bool AlreadyUsed { get; set; } = false;

        public List<DbWhereField> Exclusions { get; set; } // for future extends
        public string ConcatOper { get; set; }

    }

    public class DbSelectField
    {     
        public string TableAlias { get; set; }
        public string Fieldname { get; set; }
        public string Alias { get; set; }
        public DbAgregate AgregateFunction { get; set; }
        public string Function { get; set; } /// user string format like: Mult({0}*0.3) // where {0} is the field.
        public string Select { get; set; }

        public bool GroupBy { get; set; } // for future extends
    }

    public class DbUpdateField
    {
        public string TableAlias { get; set; }
        public string Fieldname { get; set; }
        public object Value { get; set; }

        public string FullType { get; set; }
        public string Type { get; set; }

        public string Update { get; set; }
    }

    public class DbWhereField
    {
        public string TableAlias { get; set; }
        public string Fieldname { get; set; }
        public DbOperator Operator { get; set; }
        public object Value { get; set; }

        public string FullType { get; set; }
        public string Type { get; set; }
        public string strOperator { get; set; }
        public string Where { get; set; }
    }

    public class DbOrderField
    {
        public string TableAlias { get; set; }
        public string Fieldname { get; set; }
        public bool Descendent { get; set; }
    }

    #endregion

    public class DBHelper
    {

        #region NOTES
        ///
        /// optou-se por montar os strings ao inves de manter lista de parametros
        ///
        /// <summary>
        /// 
        /// Para tentar facilitar a atualização em tabelas (FLAT/single tables)
        /// 
        /// Exemplo de Utilização:
        /// 
        ///        List<RoboDemoPagamento> rdp = Robot.Data.ObtemRoboDemoPagamento();
        ///        RoboDemoPagamento irdp = new RoboDemoPagamento();
        ///        
        ///        irdp.Id = 5;
        ///        irdp.Id = Robot.Data.CriaRoboDemoPagamento(irdp, false);
        ///        irdp.IdentificacaoPrestador = "XYZKLM";
        ///        irdp.DataCriacao = DateTime.Now;
        ///        
        ///        Robot.Data.ClearParms();
        ///        Robot.Data.AddFieldValue<RoboDemoPagamento>(irdp, "IdentificacaoPrestador");
        ///        Robot.Data.AddFieldValue<RoboDemoPagamento>(irdp, "DataCriacao");
        ///        Robot.Data.UpdateRecord("RoboDemoPagamento", "Id",irdp.Id);
        ///        
        ///        Robot.Data.AddAllFields(irdp);
        ///        Robot.Data.UpdateRecord("RoboDemoPagamento", "Id",irdp.Id);
        ///        
        /// </summary>
        /// <param name="Table"></param>
        /// <param name="PkField"></param>
        /// <param name="Pk"></param>
        /// <param name="UseData"></param>
        /// <returns></returns>
        /// 
        /// Helper
        ///
        /// para testar a montagem das strings para bd
        /// 
        /// DBHelper xxx = new DBHelper(db, "RoboDemoPagamento", "RDP", "ID", typeof(RoboDemoPagamento).ToString());
        ///
        /// xxx.SetSchema("base");
        /// xxx.AddTable("RoboDemoProtocolo", "RP1", "ID");
        /// xxx.AddTable("RoboDemoProtocolo", "RP2", "abcdefg");
        ///
        /// xxx.AddJoin(DbJoinType.Outer, "RDP", "RP1", "fkdata1");
        /// xxx.AddJoin(DbJoinType.Inner, "RP1", "RP2", "fkdata2");
        ///
        /// xxx.AddSelect("RoboDemoProtocolo", "2adef", "");
        /// xxx.AddSelect("RoboDemoProtocolo", "3aef", "a");
        /// xxx.AddSelect("RoboDemoProtocolo", "4def", "b");
        /// xxx.AddSelect("RDP", "campo1", "cp1");
        /// xxx.AddSelect("RDP", "campo2", "cp2", DbAgregate.Sum);
        ///
        /// xxx.AddWhere("RDP", "sdfr", DbOperator.Equal, DataIni);
        /// xxx.AddWhere("RP1", "sddddfr", DbOperator.Greater, cultura);
        ///
        /// xxx.AddOrder("RDP", "wss");
        /// xxx.AddOrder("RP1", "3es");
        ///
        /// xxx.AddUpdt("RoboDemoPagamento", "data", cultura);
        /// xxx.AddUpdt("RoboDemoPagamento", "def", DataIni);
        /// xxx.AddUpdt("RoboDemoPagamento", "jkl", Prestador);
        /// xxx.SetPkValue(123);
        /// xxx.Update();
        /// 
        /// DBHelper xxx = new DBHelper(db, "RoboDemoProtocolo", "RDP", "ID", typeof(RoboDemoPagamento).ToString());
        /// List<RoboDemoProtocolo> rbp = xxx.Select<RoboDemoProtocolo>(true);
        /// RoboDemoProtocolo rbpitem = rbp[0];
        /// xxx.DbSetPkValue(rbpitem.ID);
        /// xxx.Update<RoboDemoProtocolo>(rbpitem);
        /// 
        /// ----------------------------------------------------
        ///
        #endregion

        #region FULL VERSION

        // for tables multiple
        List<DbTable> DbTable = new List<DbTable>();
        List<DbJoin> DbJoin = new List<DbJoin>();
        List<DbSelectField> DbSelect = new List<DbSelectField>();
        List<DbUpdateField> DbUpdtField = new List<DbUpdateField>();
        List<DbWhereField> DbWhere = new List<DbWhereField>();
        List<DbOrderField> DbOrder = new List<DbOrderField>();

        public bool NoLock = true;
        public string Provider { get; set; }  = "System.Data.SqlClient";
        public string ConnectionString { get; set; }  = "";
        public string AditionalUpdateEndClause = "";
        public string AditionalInsertEndClause = "";
        public string AditionalSelectEndClause = "";
        public string AditionalDeleteEndClause = "";

        #endregion

        #region For all versions

        public string Database { get; set; } = "GRMASTER";
        public string PkField { get; set; }
        public string PkValue { get; set; }
        public string Alias { get; set; }
        public string Table { get; set; } = "";
        public string Schema { get; set; } = "";
        public int Top { get; set; } = 0;
        public DbTopMode TopMode { get; set; } = DbTopMode.Tuplas;

        Type ObjType = null;
        public String FieldList { get; set; } = ""; // string para select e insert
        public String ValueList { get; set; } = ""; // string para dados do insert
        public String UpdateStatement { get; set; } = ""; // string para update field=value
        public String WhereFilter { get; set; } = ""; // string com filtros with AND
        public String OrderBy { get; set; } = ""; // string com order by

        #endregion

        public DBHelper(string database, string table, string pkfield, string alias = "", Type objtype = null)
        {
            Database = database;
            Table = table;
            Alias = alias;
            PkField = pkfield;
            ObjType = objtype;
            AddTable(table,  pkfield, alias);
        }

        #region FULL VERSION - OFFICIAL

        public string GetPkField(string table)
        {
            string result = "";
            foreach (DbTable r in DbTable )
            {
                if (r.Alias.ToLower() == table.ToLower())
                {
                    return r.PK;
                }
            }
            foreach (DbTable r in DbTable)
            {
                if (r.Name.ToLower() == table.ToLower())
                {
                    return r.PK;
                }
            }
            return result;
        }

        public string GetPkField()
        {
            return PkField;
        }

        public string GetTableName(string tablealias)
        {
            string result = "";
            foreach (DbTable r in DbTable)
            {
                if (r.Alias.ToLower() == tablealias.ToLower())
                {
                    return r.Table;
                }
            }
            return result;
        }

        public string GetTableAlias(string Table)
        {
            string result = "";

            foreach (DbTable r in DbTable)
            {
                if (r.Name.ToLower() == Table.ToLower())
                {
                    return r.Alias;
                }
            }

            foreach (DbTable r in DbTable)
            {
                if (r.Table.ToLower() == Table.ToLower())
                {
                    return r.Alias;
                }
            }

            return result;
        }

        public bool CheckExistSelectField(string Fieldname)
        {
            bool result = false;
            foreach (DbSelectField r in DbSelect)
            {
                if (r.Fieldname.ToLower() == Fieldname.ToLower())
                {
                    return true;
                }
            }       
            return result;
        }

        public void SetPkValue(object Value)
        {
            PkValue = Value.ToString();
        }

        public void SetDatabase(object Value)
        {
            Database = Value.ToString();
        }

        public void SetSchema(object Value)
        {
            Schema = Value.ToString();
            if (Table != "")
            {
                Table = "[" + Schema + "].[" + Table + "]";
            }
            // ajustar o table basico...
            DbTable[0].Schema = Schema;
            DbTable[0].Table = "[" + Schema + "].[" + DbTable[0].Name + "]";
        }

        public void Clear()
        {
            ClearOrder();
            ClearSelect();
            ClearUpdt();
            ClearWhere();
            ClearJoin();
            // o dbtable necessita de alguns cuidados extras
            string table = DbTable[0].Name;
            string alias = DbTable[0].Alias;
            string pkfield = DbTable[0].PK;
            ClearTable();
            AddTable(table, pkfield, alias);
        }

        public void ClearSelect()
        {
            DbSelect.Clear();
        }

        public void ClearTable()
        {
            DbTable.Clear();
        }

        public void ClearJoin()
        {
            DbJoin.Clear();
        }

        public void ClearUpdt()
        {
            DbUpdtField.Clear();
        }

        public void ClearWhere()
        {
            DbWhere.Clear();
        }

        public void ClearOrder()
        {
            DbOrder.Clear();
        }
  
        public void AddTable(string table, string pk = null, string alias = null, string schema = null)
        {
            if (alias == null) { alias = table; }
            if ((schema == "" || schema==null) && Schema != "") { schema = Schema; }

            // modo expert
            DbTable dbtable = new DbTable();

            if (DbTable.Count == 0)
            {
                Table = table;
                if (Schema != "")
                {
                    Table = "[" + Schema + "].[" + Table + "]";
                }
            }

            dbtable.Name = table.ToLower();
            dbtable.Schema = Schema.ToLower();
            dbtable.Alias = alias.ToLower();
            dbtable.PK = pk.ToLower();

            if (schema != "" && schema !=  null)
            {
                dbtable.Table = "[" + schema.ToLower() + "].[" + dbtable.Name + "]";
            }
            else
            {
                dbtable.Table = "[" + dbtable.Name + "]";
            }

            DbTable.Add(dbtable);

        }

        public void AddJoin(DbJoinType jointype, string pktablealias, string fktablealias, string joinfield, List<DbWhereField> Exclusions = null, string ConcatOper = "AND" )
        {
            DbJoin dbjoin = new DbJoin();
            dbjoin.JoinType = jointype;
            dbjoin.PkTable = pktablealias;
            dbjoin.FkTable = fktablealias;
            dbjoin.JoinField = joinfield;
            dbjoin.PkField = GetPkField(pktablealias);
            switch (jointype)
            {
                case DbJoinType.Join:
                    dbjoin.Join = "JOIN ";
                    break;
                case DbJoinType.Inner:
                    dbjoin.Join = "INNER JOIN ";
                    break;
                case DbJoinType.Outer:
                    dbjoin.Join = "OUTER JOIN ";
                    break;
                case DbJoinType.Cross:
                    dbjoin.Join = "CROSS JOIN ";
                    break;
                case DbJoinType.FullOuter:
                    dbjoin.Join = "FULL JOIN ";
                    break;
                case DbJoinType.LeftOuter:
                    dbjoin.Join = "LEFT OUTER JOIN ";
                    break;
                case DbJoinType.RightOuter:
                    dbjoin.Join = "RIGHT OUTER JOIN ";
                    break;
            }

            if (Exclusions != null) {
                dbjoin.Exclusions = new List<DbWhereField>();
                dbjoin.Exclusions = Exclusions;
                dbjoin.ConcatOper = ConcatOper;
            }

            DbJoin.Add(dbjoin);
            
        }

        public void AddSelect(string field)
        {
            AddSelect(Alias, field, field, DbAgregate.None, "");
        }

        public void AddSelect(string field, string alias, DbAgregate dbagregate = DbAgregate.None, string function = "")
        {
            AddSelect(Alias, field, alias, dbagregate, function);
        }

        public void AddSelect(string tablealias, string field, string alias, DbAgregate dbagregate = DbAgregate.None, string function = "")
        {
            DbSelectField dbselect = new DbSelectField();
            dbselect.TableAlias = tablealias;
            dbselect.Fieldname = field;
            dbselect.Alias = alias;

            dbselect.Select = dbselect.TableAlias == "" ? "[" + dbselect.Fieldname + "]" : "[" + dbselect.TableAlias + "].[" + dbselect.Fieldname + "]";

            if (dbagregate != DbAgregate.None)
            {
                dbselect.AgregateFunction = dbagregate;
                dbselect.Function = Enum.GetName(typeof(DbAgregate), dbagregate) + "({0})";
                dbselect.Select = (String.Format(dbselect.Function, dbselect.Select));
            }
            else
            {
                if (function!="")
                {
                    dbselect.Function = function;
                    dbselect.Select = (String.Format(dbselect.Function, dbselect.Select));
                }
            }

            if (dbselect.Alias != "")
            {
                dbselect.Select = dbselect.Select + " as [" + dbselect.Alias + "]";
            }

            DbSelect.Add(dbselect);

            //public DbAgregate AgregateFunction { get; set; }
            //public string Function { get; set; } /// user string format like: Mult({0}*0.3) // where {0} is the field.

        }

        public void AddUpdt(string field, object value)
        {
            AddUpdt(Table, field, value);
        }

        public void AddUpdt(string table, string field, object value)
        {
            if (table == null || table == "") { table = Table; }

            DbUpdateField dbupdt = new DbUpdateField();
            dbupdt.TableAlias = table;
            dbupdt.Fieldname = field;

            try // poi o valor pode ser nulo... devera ser tratado oportunamente
            {
                String type = value.GetType().ToString().ToLower();

                dbupdt.FullType = type;
                dbupdt.Type = type.Split('.').Last();

                dbupdt.Value = value;
                dbupdt.Update = "" + dbupdt.TableAlias + ".[" + dbupdt.Fieldname + "]";

                DbUpdtField.Add(dbupdt);
            }
            catch { }
        }

        public void AddWhere(string field, DbOperator oper, object value)
        {
            AddWhere(Table, field, oper, value);
        }

        public void AddWhere(string table, string field, DbOperator oper, object value)
        {
            if (table == null || table == "") { table = Table; }

            DbWhereField dbwhere = new DbWhereField();
            dbwhere.TableAlias = table;
            dbwhere.Fieldname = field;

            String type = value.GetType().ToString().ToLower();

            dbwhere.FullType = type;
            dbwhere.Type = type.Split('.').Last();
            dbwhere.Operator = oper;
            switch (oper)
            {
                case DbOperator.Equal:
                    dbwhere.strOperator = " = ";
                    break;
                case DbOperator.Diferent:
                    dbwhere.strOperator = " <> ";
                    break;
                case DbOperator.Contain:
                    dbwhere.strOperator = " contains ";
                    break;
                case DbOperator.Greater:
                    dbwhere.strOperator = " > ";
                    break;
                case DbOperator.GreaterEqual:
                    dbwhere.strOperator = " >= ";
                    break;
                case DbOperator.Less:
                    dbwhere.strOperator = " < ";
                    break;
                case DbOperator.LessEqual:
                    dbwhere.strOperator = " <= ";
                    break;
                case DbOperator.Like:
                    dbwhere.strOperator = " Like ";
                    break;
                case DbOperator.Match:
                    dbwhere.strOperator = " March ";
                    break;
                case DbOperator.Is:
                    dbwhere.strOperator = " is ";
                    break;
                case DbOperator.IsNot:
                    dbwhere.strOperator = " is not ";
                    break;
            }
            dbwhere.Value = value;
            dbwhere.Where = " " + dbwhere.TableAlias + "." + dbwhere.Fieldname + "";

            DbWhere.Add(dbwhere);
        }

        public void AddOrder(string field, bool descendent = false)
        {
            AddOrder(Alias, field, descendent);
        }

        public void AddOrder(string tablealias, string field, bool descendent = false)
        {
            DbOrderField dborder = new DbOrderField();
            dborder.TableAlias = tablealias;
            dborder.Fieldname = field;
            dborder.Descendent = descendent;
            DbOrder.Add(dborder);
        }

        public string SelectStr()
        {
            string SelectStatement = "";
   
            foreach (DbSelectField fld in DbSelect)
            {

                string conector = "";
                if (SelectStatement != "")
                {
                    conector = ", ";
                }

                SelectStatement = SelectStatement + conector + fld.Select;

            }

            return SelectStatement;
        }

        public string UpdateStr()
        {
            string UpdateStatement = "";
            string Delimitador = "";
            foreach (DbUpdateField fld in DbUpdtField)
            {

                if (!IsNumericType(fld.Type)) { Delimitador = "'"; }
                if (IsLogical(fld.Type)) { Delimitador = ""; }

                object propValue = fld.Value.ToString();
                //
                // As datas podem ser tratadas neste ponto... para contemplar formatos... restrições etc.
                //
                string strValue = propValue.ToString();
                if (IsNumericType(fld.Type))
                {
                    strValue = strValue.Replace(",", ".");
                }
                if (IsDateType(fld.Type))
                {
                    DateTime dtValue = (DateTime)CHelper.toDate(strValue, false);
                    strValue = toDateTimeIso(dtValue);
                }

                string conector = "";
                if (UpdateStatement != "")
                {
                    conector = ",";
                }

                if (propValue != null)
                {
                    if (IsLogical(fld.Type))
                    {
                        if (strValue.ToUpper() == "TRUE") { strValue = "1"; }
                        else { strValue = "0";  }
                    }
                    UpdateStatement = UpdateStatement + conector + fld.Update + " = " + Delimitador + strValue + Delimitador;
                }
            }
            return UpdateStatement;
        }

        public string UpdateStr<T>(T Obj, string[] Except, bool isSelectAll = false, bool includePk = false)
        {
            string UpdateStatement = "";
            object o = Obj;
            string[] propertyNames = o.GetType().GetProperties().Select(p => p.Name).ToArray();
            foreach (var Fieldname in propertyNames)
            {
                if (Except != null)
                {
                    if (Except.Contains(Fieldname))
                    {
                        continue;
                    }
                }

                if (!includePk)
                {
                    if (Fieldname.ToLower() == PkField.ToLower()) { continue; }
                }

                object FieldValue = o.GetType().GetProperty(Fieldname).GetValue(o, null);

                // check if field is at dbselect...
                bool doit = CheckExistSelectField(Fieldname);
                if (!doit && isSelectAll) { doit = true; }

                if (FieldValue != null && doit)
                {
                    Mapper nMap = new Mapper(Obj.GetType(), Obj.GetType());

                    string pptype = Obj.GetType().GetProperty(Fieldname.ToString()).PropertyType.ToString();
                    if (!nMap.isCollection(Obj.GetType().GetProperty(Fieldname.ToString()).PropertyType))
                    {
                        {
                        //AddFieldValue<T>(Obj, prop.ToString());
                        string Delimitador = "";

                        if (UpdateStatement != "")
                        {
                            UpdateStatement = UpdateStatement + ", ";
                        }



                        String type = Obj.GetType().GetProperty(Fieldname.ToString()).PropertyType.ToString().ToLower();
                        if (!IsNumericType(type)) { Delimitador = "'"; }
                        if (IsLogical(type)) { Delimitador = ""; }

                        object prpValue = Obj.GetType().GetProperty(Fieldname.ToString()).GetValue(Obj, null);
                        //
                        // As datas podem ser tratadas neste ponto... para contemplar formatos... restrições etc.
                        //
                        string strValue = prpValue.ToString();

                        if (IsNumericType(type))
                        {
                            strValue = strValue.Replace(",", ".");
                        }

                        if (IsDateType(type))
                        {
                            DateTime dtValue = (DateTime)CHelper.toDate(strValue, false);
                            strValue = toDateTimeIso(dtValue);
                        }

                            if (FieldValue != null)
                            {
                                if (IsLogical(type))
                                {
                                    if (strValue.ToUpper() == "TRUE") { strValue = "1"; }
                                    else { strValue = "0"; }
                                }
                                UpdateStatement = UpdateStatement + Fieldname + "=" + Delimitador + strValue + Delimitador;
                            }
                        }
                    }
                }

            }

            return UpdateStatement;
        }

        //
        // for use with WHERE and Exlusion Joins
        //
        public string BaseWhereStr(List<DbWhereField> wherelist, string Conector)
        {
            string WhereStatement = "";
            string Delimitador = "";
            foreach (DbWhereField fld in wherelist)
            {
                Delimitador = "";
                if (!IsNumericType(fld.Type)) { Delimitador = "'"; }
                if (IsLogical(fld.Type)) { Delimitador = ""; }

                object propValue = fld.Value.ToString();
                //
                // As datas podem ser tratadas neste ponto... para contemplar formatos... restrições etc.
                //
                string strValue = propValue.ToString();
                if (IsNumericType(fld.Type))
                {
                    strValue = strValue.Replace(",", ".");
                }
                if (IsDateType(fld.Type))
                {
                    DateTime dtValue = (DateTime)CHelper.toDate(strValue, false);
                    strValue = toDateTimeIso(dtValue);
                }

                string conector = "";
                if (WhereStatement != "")
                {
                    conector = " " + Conector.ToUpper() + " ";
                }

                if (propValue != null)
                {
                    WhereStatement = WhereStatement + conector + fld.Where + fld.strOperator + Delimitador + strValue + Delimitador;
                }
                else
                {
                    if (fld.Operator == DbOperator.Is || fld.Operator == DbOperator.IsNot)
                    {
                        WhereStatement = WhereStatement + conector + fld.Where + fld.strOperator + " null ";
                    }
                }
            }
            return WhereStatement;
        }

        public string WhereStr()
        {
            return BaseWhereStr(DbWhere, "AND");
        }

        public string OrderStr()
        {
            string OrderStatement = "";
            foreach (DbOrderField fld in DbOrder)
            {

                string conector = "";
                if (OrderStatement != "")
                {
                    conector = ", ";
                }

                OrderStatement = OrderStatement + conector + fld.Fieldname + (fld.Descendent ? " DESC " : "");

            }
            return OrderStatement;
        }

        public string GroupByStr()
        {
            string GropyByStatement = "";
            foreach (DbSelectField fld in DbSelect)
            {
                if (fld.GroupBy)
                {
                    string conector = "";
                    if (GropyByStatement != "")
                    {
                        conector = ", ";
                    }

                    GropyByStatement = GropyByStatement + conector + "[" + fld.TableAlias + "].[" + fld.Fieldname + "]";
                }

            }
            return GropyByStatement;
        }

        public string JoinStr(string father, int level)
        {
            string join = "";
            foreach (DbJoin fld in DbJoin)
            {
                if (!fld.AlreadyUsed)
                {
                    if (fld.PkTable == father)
                    {
                        join = join + "\n" + new String(' ', (level) * 3) + fld.Join + GetTableName(fld.FkTable) + " as [" + fld.FkTable + "]";

                        if (NoLock)
                        {
                            if (join != "")
                            {
                                join = join + " WITH (NOLOCK) ";
                            }
                        }

                        string extra = JoinStr(fld.FkTable, level+1);
                        if (extra != "")
                        {
                            join = join + new String(' ', (level+1)*3) + extra;
                        }
                        // aqui inserir rotina para exclusions 
                        string where = "";
                        if (fld.Exclusions != null)
                        {
                            where = BaseWhereStr(fld.Exclusions, fld.ConcatOper);
                        }
                        if (where != "") { where = "Where " + where;  }
                        // -----------------------------------
                        //join = join + "\n" + new String(' ', (level) * 3) + " ON [" + fld.FkTable + "].[" + GetPkField(father) + "] = [" + father + "].[" + fld.JoinField + "] " + where;

                        join = join + "\n" + new String(' ', (level) * 3) + " ON [" + fld.FkTable + "].[" +  fld.JoinField + "] = [" + father + "].[" + GetPkField(father)  + "] " + where;

                        fld.AlreadyUsed = true;
                    }
                }
            }

           return join;
        }

        public string FromStr()
        {
            string alias = GetTableAlias(Table);
            string FromStatement = Table + " " + (alias != "" ? " as [" + alias + "]" : "");

            if (NoLock)
            {
                if (FromStatement != "")
                {
                    FromStatement = FromStatement + " WITH (NOLOCK) ";
                }
            }

            if (DbJoin.Count != 0)
            {
                foreach (DbJoin fld in DbJoin)
                {   
                    if (fld.PkTable.ToLower() == alias.ToLower()) {
                        FromStatement = FromStatement + JoinStr(fld.PkTable, 1);
                    }
                }
            }


            return FromStatement;
        }

        public List<T> Select<T>(bool usewhere = false) where T : class, new()
        {
            string select = SelectStr();
            string TOP = "";
            if (Top != 0)
            {
                 TOP = " TOP (" + Top.ToString() + (TopMode == DbTopMode.Percent ? "%" : "" ) + ") ";
            }
            

            if (select == "") { select = "*"; }

            string from = FromStr();
            
            string where = "";
            if (usewhere) { where = WhereStr(); }

            string order = OrderStr();

            string groupby = GroupByStr();

            string SelectStatement = "";
            string table = Table;

            SelectStatement = "Select " + TOP + select + 
                                " from " + from +
                                (groupby != "" ? " Group By " + groupby : "") +
                                (where!= "" ? " Where " + where : ""  ) + 
                                (order!="" ? " Order by " + order : "");

            Database dbQuery = OpenDatabase(Database);

            List<T> lst = dbQuery.ExecuteQuery<T>(SelectStatement).ToList();

            return lst;

            // Select [dataPagto] from[CL000001].[Resultados_Pagamento_IdPrest] WITH(NOLOCK)

        }

        private Database OpenDatabase (string Database)
        {

            string projectRootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            ConnectionString = Startup.CONNECTIOSTRING;
            string provider = ConnectionString.Substring(0, ConnectionString.IndexOf(';'));
            provider = provider.Substring(ConnectionString.IndexOf('=') + 1);
            ConnectionString = ConnectionString.Substring(ConnectionString.IndexOf(';') + 1);

            ConnectionString = ConnectionString.ExchangeEnclosured("{AppDir}", projectRootPath);

            Database dbQuery = null;

            //if (ConnectionString == "")
            //{
            //    dbQuery = new Database(Database);
            //}
            //else
            //{
                dbQuery = new Database(ConnectionString, provider);
            //}

            return dbQuery;

        }

        public long Update(bool usewhere = false)
        {
            string UpdateStatement = "";

            string where = "";
            if (usewhere) { where = WhereStr(); }
            else { where = PkField + " = " + PkValue.ToString(); }

            string table = Table;
         
            UpdateStatement = "Update " + table + " \nSet \n" + UpdateStr() + "\n Where " + where + " " + AditionalUpdateEndClause;

            Database dbQuery = OpenDatabase(Database);

            dbQuery.CommandType = CommandType.Text;
            dbQuery.CommandText = UpdateStatement;
            var ret = dbQuery.ExecuteCommand();

            return ret;
        }
    
        public long Update(object pkfield)
        {
            string table = Table;
            string UpdateStatement = "Update " + Table + " \nSet \n" + UpdateStr() + "\n Where " + pkfield.ToString() + " = " + PkValue.ToString() + " "+ AditionalUpdateEndClause;

            Database dbQuery = OpenDatabase(Database);

            dbQuery.CommandType = CommandType.Text;
            dbQuery.CommandText = UpdateStatement;
            var ret = dbQuery.ExecuteCommand();

            return ret;
        }

        public long Update<T>(T obj, bool isSelectAll = false, bool includePk = false )
        {
            string table = Table;
            string[] exept = new string[] { PkField };
            string UpdtStatement = UpdateStr<T>(obj, exept, isSelectAll, includePk);
            string UpdateStatement = "Update " + Table + " \nSet \n" + UpdtStatement + "\n Where " + PkField.ToString() + " = " + PkValue.ToString() + " " + AditionalUpdateEndClause;

            Database dbQuery = OpenDatabase(Database);

            dbQuery.CommandType = CommandType.Text;
            dbQuery.CommandText = UpdateStatement;
            var ret = dbQuery.ExecuteCommand();

            return ret;
        }

        public long Insert<T>(T Data)
        {

            Database dbQuery = OpenDatabase(Database);

            dbQuery.CommandText = String.Format(@"
                                                if NOT exists(SELECT {2} FROM {0} where {2} = {1}) 
                                                    begin
                                                        insert into {0} Default Values {3};
                                                        Select CAST(scope_identity() AS bigint);
                                                    end
                                                else Select CAST( {1} as bigint );
                                                ", Table, PkValue.ToString(), PkField, AditionalInsertEndClause);

            dbQuery.CommandType = CommandType.Text;

            long ret = (long)dbQuery.ExecuteScalar();

            PkValue = ret.ToString();

            if (Data!=null)
            {
                Update<T>(Data, true, false);
            }

            return ret;
        }

        public long Insert<T>(T Data, object KeyValue)
        {
            long ret = 0;

            string keyValue = KeyValue.ToString();

            Database dbQuery = OpenDatabase(Database);

            dbQuery.CommandText = $"if NOT exists(SELECT {PkField} FROM {Table} where {PkField} = {keyValue}) " +
                                        $"begin " +
                                            $"insert into {Table} ({ PkField}) Values ({ keyValue}) {AditionalInsertEndClause}; " +
                                            $"Select CAST(scope_identity() AS bigint); " +
                                        $"end " +
                                    $"else Select CAST( {keyValue} as bigint ); ";
                                     

            dbQuery.CommandType = CommandType.Text;

            dbQuery.ExecuteScalar();

            PkValue = keyValue;

            try
            {
                ret = Convert.ToInt32(keyValue);
            }
            catch { }

            if (Data != null)
            {
                Update<T>(Data, true, false);
            }

            return ret;
        }

        public long Insert()
        {

            Database dbQuery = OpenDatabase(Database);

            dbQuery.CommandText = String.Format(@"
                                                if NOT exists(SELECT {2} FROM {0} where {2} = {1}) 
                                                    begin
                                                        insert into {0} Default Values {3};
                                                        Select CAST(scope_identity() AS bigint);
                                                    end
                                                else Select CAST( {1} as bigint );
                                                ", Table, PkValue.ToString(), PkField, AditionalInsertEndClause);

            dbQuery.CommandType = CommandType.Text;

            long ret = (long)dbQuery.ExecuteScalar();

            PkValue = ret.ToString();

            return ret;
        }

        public long Delete()
        {

            Database dbQuery = OpenDatabase(Database);

            dbQuery.CommandText = String.Format(@"
                                                if exists(SELECT {2} FROM {0} where {2} = {1}) 
                                                    begin
                                                        Delete FROM {0} where {2} = {1} {3};
                                                    end
                                                else Select CAST( {0} as bigint );
                                                ", Table, PkValue.ToString(), PkField, AditionalDeleteEndClause);

            dbQuery.CommandType = CommandType.Text;

            long ret = (long)dbQuery.ExecuteScalar();

            PkValue =  0.ToString();

            return ret;
        }

        #region extra functions

        public static bool IsLogical(string type)
        {
            type = type.Split('.').Last().ToLower();
            switch (type)
            {
                case "boolean":
                case "bool":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNumericType(string type)
        {
            type = type.Split('.').Last().ToLower();
            switch (type)
            {
                case "byte":
                case "sbyte":
                case "uint16":
                case "uint32":
                case "uint64":
                case "float":
                case "int16":
                case "int32":
                case "int64":
                case "decimal":
                case "double":
                case "single":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsDateType(string type)
        {
            type = type.Split('.').Last();
            switch (type)
            {
                case "date":
                case "datetime":
                    return true;
                default:
                    return false;
            }
        }

        //
        // As funçoes abaixo já podem existir em algum outro ponto do sistema
        //
        public static string toDateTimeIso(DateTime dtValue)
        {

            string year = dtValue.Year.ToString();
            string month = dtValue.Month.ToString();
            string day = dtValue.Day.ToString();
            string hour = dtValue.Hour.ToString();
            string minute = dtValue.Minute.ToString();
            string second = dtValue.Second.ToString();

            while (year.Length < 4) { year = "0" + year; }
            while (month.Length < 2) { month = "0" + month; }
            while (day.Length < 2) { day = "0" + day; }
            while (hour.Length < 2) { hour = "0" + hour; }
            while (minute.Length < 2) { minute = "0" + minute; }
            while (second.Length < 2) { second = "0" + second; }

            string result = "";
            result = year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second;
            return result;
        }

        public static string toDateIso(DateTime dtValue)
        {

            string year = dtValue.Year.ToString();
            string month = dtValue.Month.ToString();
            string day = dtValue.Day.ToString();

            while (year.Length < 4) { year = "0" + year; }
            while (month.Length < 2) { month = "0" + month; }
            while (day.Length < 2) { day = "0" + day; }

            string result = "";
            result = year + "-" + month + "-" + day;
            return result;
        }

        #endregion

        #endregion

        #region FLAT FILE MODEL - DEPRECATED

        public long AddRecord(long Pk, bool UseData)
        {
            Database dbQuery = OpenDatabase(Database);

            if (FieldList == "") { UseData = false; }

            if (UseData)
            {
                dbQuery.CommandText = String.Format(@"
                                                if NOT exists(SELECT {4} FROM {0} where {4} = {1}) 
                                                    begin
                                                        insert into {0} ({2}) values ({3});
                                                        Select CAST(scope_identity() AS bigint);
                                                    end
                                                else Select CAST( {1} as bigint );
                                                ", Table, Pk.ToString(), FieldList, ValueList, PkField);
            }
            else
            {
                dbQuery.CommandText = String.Format(@"
                                                if NOT exists(SELECT {2} FROM {0} where {2} = {1}) 
                                                    begin
                                                        insert into {0} Default Values;
                                                        Select CAST(scope_identity() AS bigint);
                                                    end
                                                else Select CAST( {1} as bigint );
                                                ", Table, Pk.ToString(), PkField);
            }
            dbQuery.CommandType = CommandType.Text;

            long ret = (long)dbQuery.ExecuteScalar();

            return ret;
        }

        public string MountStatement(string FieldList, string Where, string Order)
        {

            if (FieldList == "") { FieldList = "*"; }

            String SqlStatement = "Select " + FieldList + " from " + Table + " ";

            if (Where != "" && Where != null)
            {
                SqlStatement = SqlStatement + " Where " + Where;
            }

            if (Order != "" && Order != null)
            {
                SqlStatement = SqlStatement + " Order By " + Order;
            }

            return SqlStatement;
        }

        public long UpdateRecord(long PK)
        {
            Database dbQuery = OpenDatabase(Database);

            dbQuery.CommandText = String.Format(@"
                                                if exists(SELECT {3} FROM {0} where {3} = {2}) 
                                                    begin
                                                      Update {0} Set {1} Where {3} = {2};
                                                    end
                                                ", Table, UpdateStatement, PK.ToString(), PkField);
            //
            // if exists(SELECT Id FROM RoboDemoProtocolo where Id = 397) 
            //    begin
            //       Update RoboDemoProtocolo Set AgendaId = 12345610,PrestadorId = 16,OperadoraId = 326305,PagamentoId = 11,Login = '10833919',MesAno = '032019',Protocolo = '2540564560',DataCriacao = '2019-10-5 0:0:0',DataEnvio = '2019-3-1 0:0:0',DataPagto = '2001-1-1 0:0:0',ValorInformado = 18512.26,ValorProcessado = 18308.86,ValorLiberado = 18512.26,ValorGlosa = 203.4,ValorRevisto = 0,QtdeContasPagas = 0,QtdeContasGlosadas = 0,Observacao = '',Status = 'CONFIRMADO MXM',CodigoArq = 0,Desativado = 0 Where Id = 397;
            //    end
            //
            // if exists(SELECT Id FROM RoboDemoProtocolo where Id = 398) 
            //    begin
            //       Update RoboDemoProtocolo Set AgendaId = 12345610,PrestadorId = 16,OperadoraId = 326305,PagamentoId = 11,Login = '10833919',MesAno = '032019',Protocolo = '2540564561',DataCriacao = '2019-10-5 0:0:0',DataEnvio = '2019-3-1 0:0:0',DataPagto = '2001-1-1 0:0:0',ValorInformado = 18990.18,ValorProcessado = 18882.65,ValorLiberado = 18990.18,ValorGlosa = 107.53,ValorRevisto = 0,QtdeContasPagas = 0,QtdeContasGlosadas = 0,Observacao = '',Status = 'CONFIRMADO MXM',CodigoArq = 0,Desativado = 0 Where Id = 398;
            //    end
            //
            dbQuery.CommandType = CommandType.Text;
            var ret = dbQuery.ExecuteCommand();

            return ret;
        }

        public void UpdateRecords()
        {
            Database dbQuery = OpenDatabase(Database);

            dbQuery.CommandText = String.Format(@"Update {0} Set {1} Where {2};", Table, UpdateStatement, WhereFilter);
            //
            // Update RoboDemoProtocolo Set Desativado=0 Where PagamentoId = 11;
            //
            dbQuery.CommandType = CommandType.Text;
            var ret = dbQuery.ExecuteCommand();

            return;
        }

        public void ClearParms()
        {
            FieldList = "";
            ValueList = "";
            UpdateStatement = "";
            WhereFilter = "";
            OrderBy = "";
        }

        public void AddOrderField(String FieldName, String Mode = "ASC")
        {
            if (OrderBy != "")
            {
                OrderBy = OrderBy + ",";
            }
            OrderBy = OrderBy + FieldName;
            if (Mode != "") { OrderBy = OrderBy + " " + Mode; }
        }

        public void AddFieldFilter<T>(T Obj, String FieldName, String Value, String Operator)
        {
            string Delimitador = "";

            if (WhereFilter != "" && Operator != null)
            {
                WhereFilter = WhereFilter + " and ";
            }

            String type = Obj.GetType().GetProperty(FieldName).PropertyType.ToString().ToLower();
            if ("system.string|system.date|system.datetime".Contains(type)) { Delimitador = "'"; }

            object propValue = Obj.GetType().GetProperty(FieldName).GetValue(Obj, null);


            //
            // Para o Where não haverá tratamento de busca como nulo
            //
            if (Value != null)
            {
                //
                // As datas podem ser tratadas neste ponto... para contemplar formatos... restrições etc.
                //
                string strValue = Value.ToString();
                if ("system.date|system.datetime".Contains(type))
                {
                    DateTime dtValue = (DateTime)CHelper.toDate(strValue);
                    strValue = dtValue.Year.ToString() + "-" + dtValue.Month.ToString() + "-" + dtValue.Day.ToString() + " " + dtValue.Hour.ToString() + ":" + dtValue.Minute + ":" + dtValue.Second.ToString();
                }

                if (Operator == "") { Operator = "="; }
                if (Operator != null) // only not null operator becomes alive at where clause
                {
                    WhereFilter = WhereFilter + FieldName + " " + Operator + " " + Delimitador + strValue + Delimitador;
                }
            }
            else // for null values, but with operator
            {
                if (Operator.ToLower() == "is" || Operator.ToLower() == "is not" || Operator.ToLower() == "isnot")
                {
                    WhereFilter = WhereFilter + FieldName + Operator + " null ";
                }
            }
        }

        public void AddFieldValue<T>(T Obj, String FieldName, String Value, String Operator)
        {
            string Delimitador = "";

            if (FieldList != "")
            {
                FieldList = FieldList + ",";
                ValueList = ValueList + ",";
                UpdateStatement = UpdateStatement + ",";
            }

            if (WhereFilter != "" && Operator != null)
            {
                WhereFilter = WhereFilter + " and ";
            }

            String type = Obj.GetType().GetProperty(FieldName).PropertyType.ToString().ToLower();
            if ("system.string|system.date|system.datetime".Contains(type)) { Delimitador = "'"; }

            object propValue = Obj.GetType().GetProperty(FieldName).GetValue(Obj, null);
            //
            // As datas podem ser tratadas neste ponto... para contemplar formatos... restrições etc.
            //
            string strValue = propValue.ToString();
            if (!"system.string|system.date|system.datetime".Contains(type))
            {
                strValue = strValue.Replace(",", ".");
            }
            if (type == "system.date")
            {
                DateTime dtValue = (DateTime)CHelper.toDate(strValue);
                strValue = toDateIso(dtValue);
            }
            if (type == "system.datetime")
            {
                DateTime dtValue = (DateTime)CHelper.toDate(strValue, false);
                strValue = toDateTimeIso(dtValue);
            }
            //
            // Para o Where não haverá tratamento de busca como nulo
            //
            if (Value != null)
            {
                if (Operator == "") { Operator = "="; }
                if (Operator != null) // only not null operator becomes alive at where clause
                {
                    WhereFilter = WhereFilter + FieldName + " " + Operator + " " + Delimitador + strValue + Delimitador;
                }
            }
            else // for null values, but with operator
            {
                if (Operator.ToLower() == "is" || Operator.ToLower() == "is not" || Operator.ToLower() == "isnot")
                {
                    WhereFilter = WhereFilter + FieldName + Operator + " null ";
                }
            }

            if (propValue != null)
            {
                FieldList = FieldList + FieldName;
                ValueList = ValueList + Delimitador + strValue + Delimitador;
                UpdateStatement = UpdateStatement + FieldName + "=" + Delimitador + strValue + Delimitador;
            }
        }

        public void AddFieldValue<T>(T Obj, String FieldName)
        {
            string Delimitador = "";

            if (FieldList != "")
            {
                FieldList = FieldList + ",";
                ValueList = ValueList + ",";
                UpdateStatement = UpdateStatement + ",";
            }

            String type = Obj.GetType().GetProperty(FieldName).PropertyType.ToString().ToLower();
            if ("system.string|system.date|system.datetime".Contains(type)) { Delimitador = "'"; }

            object propValue = Obj.GetType().GetProperty(FieldName).GetValue(Obj, null);
            //
            // As datas podem ser tratadas neste ponto... para contemplar formatos... restrições etc.
            //
            string strValue = propValue.ToString();
            if (!"system.string|system.date|system.datetime".Contains(type)) { strValue = strValue.Replace(",", "."); }
            if (type == "system.date")
            {
                DateTime dtValue = (DateTime)CHelper.toDate(strValue);
                strValue = toDateIso(dtValue);
            }
            if (type == "system.datetime")
            {
                DateTime dtValue = (DateTime)CHelper.toDate(strValue, false);
                strValue = toDateTimeIso(dtValue);
            }
            if (propValue != null)
            {
                FieldList = FieldList + FieldName;
                ValueList = ValueList + Delimitador + strValue + Delimitador;
                UpdateStatement = UpdateStatement + FieldName + "=" + Delimitador + strValue + Delimitador;
            }
        }

        public void AddAllFields<T>(T Obj, string[] Except, bool IncludePk = false)
        {
            ClearParms();
            object o = Obj;
            string[] propertyNames = o.GetType().GetProperties().Select(p => p.Name).ToArray();
            foreach (var prop in propertyNames)
            {
                if (Except != null)
                {
                    if (Except.Contains(prop))
                    {
                        continue;
                    }
                }

                if (!IncludePk)
                {
                    if (prop.ToLower() == PkField.ToLower()) { continue; }
                }

                object propValue = o.GetType().GetProperty(prop).GetValue(o, null);

                if (propValue != null)
                {
                    AddFieldValue<T>(Obj, prop.ToString());
                }

            }
        }

        #endregion

        private Object isDBNull(Object obj)
        {
            if (obj is DBNull)
                return null;
            else return obj;
        }

    }

}
