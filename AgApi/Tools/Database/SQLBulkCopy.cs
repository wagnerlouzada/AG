using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DB
{

    public class SQLBulkCopy
    {
        public Boolean ExecuteBulkCopy(String _table, DataTable _dataTable, Database _sqlConnection, Int32 _timeOut = 660)
        {
            try
            {
                using (SqlBulkCopy bulkcopy = new SqlBulkCopy((SqlConnection)_sqlConnection.ActiveConnection))
                {
                    bulkcopy.BulkCopyTimeout = _timeOut;
                    bulkcopy.DestinationTableName = _table;
                    bulkcopy.WriteToServer(_dataTable);
                        
                    bulkcopy.Close();
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _sqlConnection.TransactionRollback();
                throw new Exception(String.Format("Ocorreu uma falha no processamento da carga: {0} ", ex.Message));
            }            
        }   
    }

}