using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
//using RutarBackgroundServices.AppsettingsJson;
using System.Collections.Generic;
using CustomExtensions;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Reflection;
using AppV;

namespace DB
{
    public class Database 
    {
        #region Atributos

        //private IDataReader _DataReader = null;
        //private DbCommand _Command = null;
        //private DbConnection _Connection = null;
        //private DbTransaction _Transaction = null;

        //private String _ProviderName;
        //private String _ConnectionString;
        //private String ConnectionStringKey;


        public IDataReader _DataReader = null;
        public DbCommand _Command = null;
        public DbConnection _Connection = null;
        public DbTransaction _Transaction = null;

        public String _ProviderName;
        public String _ConnectionString;
        public String ConnectionStringKey;

        //private ConnectionStringSettings ConnectionStringConfig;

        protected DbProviderFactory providerFactory;

        #endregion


        public DbConnection ActiveConnection
        {
            get
            {
                return _Connection;
            }
        }
             
        #region Método Construtor

        /// <summary>
        ///     Método contrutor que cria a conexão com a string de conexão padrão
        /// </summary>
        public Database() : this(null)
        {
        }

        /// <summary>
        ///     Método contrutor que cria a conexão com a string de conexão desejada
        /// </summary>
        /// <param name="NomeChaveConexao">Nome da chave da string de conexão no arquivo de configuração</param>
        public Database(String ConnectionStringKey)
        {
            string projectRootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            try
            {
                string connectionString = "";
                string provider = "";

                if (ConnectionStringKey != "")
                {
                    connectionString = Startup.Configuration["AppConnectionStrings:" + ConnectionStringKey];
                    provider = connectionString.Substring(0, connectionString.IndexOf(';'));
                    provider = provider.Substring(connectionString.IndexOf('=') + 1);
                    connectionString = connectionString.Substring(connectionString.IndexOf(';') + 1);

                    connectionString = connectionString.ExchangeEnclosured("{AppDir}", projectRootPath);
                }

                if (connectionString != null && connectionString != "")
                {
                    _ConnectionString = connectionString;
                    _ProviderName = provider;
                    providerFactory = DbProviderFactories.GetFactory(provider);
                }
                else
                {
                    throw new Exception("Não existe a conexão " + this.ConnectionStringKey + " no arquivo de configuração.");
                }

                CreateCommand();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método contrutor que cria a conexão com a string de conexão desejada
        /// </summary>
        /// <param name="ConnectionString">A string de conexão</param>
        /// <param name="ProviderName">O nome do Provider</param>
        public Database(String ConnectionString, String ProviderName)
        {
            try
            {
                _ConnectionString = ConnectionString;
                _ProviderName = ProviderName;
                providerFactory = DbProviderFactories.GetFactory(_ProviderName);
                CreateCommand();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Métodos privados

        /// <summary>
        /// Método para verificar o provider da conexão e concatenar o nome mais o coringa para o provaider da conexao.
        /// </summary>
        /// <param name="ParameterName">Nome do paramentro</param>
        /// <returns>Retorna o parametro com o coringa</returns>
        private String ParameterIdentifier(String ParameterName)
        {
            string Prefix = "";
            switch (_ProviderName)
            {
                case "System.Data.SqlClient":
                    Prefix = "@";
                    if (ParameterName.IndexOf(Prefix) > -1)
                        return ParameterName;
                    break;
            }
            return Prefix + ParameterName;
        }

        /// <summary>
        /// Método que incapsula a criação do dbCommand
        /// </summary>
        private void CreateCommand()
        {
            if (_Command == null)
            {
                _Command = providerFactory.CreateCommand();
                CreateConnection();
                _Command.Connection = _Connection;
            }
        }
       
        /// <summary>
        /// Método que incapsula a criação da conexão com o banco de dados 
        /// </summary>
        private void CreateConnection()
        {
            if (_Connection == null)
            {
                _Connection = providerFactory.CreateConnection();
                _Connection.ConnectionString = _ConnectionString;
            }
        }

        #endregion


        #region Conexao

        /// <summary>
        /// get do status da conexão
        /// </summary>
        public ConnectionState State
        {
            get { return _Connection.State; }
        }
        /// <summary>
        /// Abri a conexão
        /// </summary>
        public void Open()
        {
            if (_Connection.State != ConnectionState.Open)
                _Connection.Open();
        }
        /// <summary>
        /// Fecha a conexao
        /// </summary>
        public void Close()
        {
            if (_Connection.State == ConnectionState.Open)
            {
                _Connection.Close();
            }
        }

        #endregion

        #region Query

        /// <summary>
        /// Executa o comando e retorna um DataTable
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable ExecuteDataTable()
        {
            DataSet ds = ExecuteDataSet();
            if (ds.Tables.Count > 0)
                return ds.Tables[0];

            return null;
        }

        /// <summary>
        ///     Executa query sem transação a inicialização, finalização e fechamento da conexão será do programador
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSet()
        {
            return ExecuteDataSet(false);
        }
        public DataSet ExecuteDataSet(Boolean KeepConnectionOpen)
        {
            DataSet ds = null;
            try
            {
                DbDataAdapter da = providerFactory.CreateDataAdapter();
                ds = new DataSet();
                da.SelectCommand = _Command;
                da.Fill(ds);

                if (_Command.Transaction == null)
                    da.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_Command.Transaction == null && !KeepConnectionOpen)
                    this.Close();
            }
            return ds;
        }

        /// <summary>
        /// Executa o comando e retorna um DataReader
        /// </summary>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteDataReader(CommandBehavior comportamento)
        {
            this.Open();
            _DataReader = _Command.ExecuteReader(comportamento);
            return _DataReader;
        }

        #endregion

        #region Transacional

        /// <summary>
        /// Inicializa transação
        /// </summary>
        public void BeginTransaction()
        {
            this.Open();
            _Transaction = _Connection.BeginTransaction();
            _Command.Transaction = _Transaction;
        }

        /// <summary>
        /// Comita uma transação
        /// </summary>
        public void TransactionCommit()
        {
            if (_Transaction != null)
                _Transaction.Commit();
            this.Close();
        }

        /// <summary>
        /// Rollback na transação
        /// </summary>
        public void TransactionRollback()
        {
            if (_Transaction != null)
                _Transaction.Rollback();
            this.Close();
        }

        #endregion

        #region Execução de DML

        /// <summary>
        /// Método para executar um comando de incluir / alterar/ Excluir 
        /// </summary>
        /// <returns>Retorna o total de registros afetados</returns>
        public Int32 ExecuteCommand()
        {
            return ExecuteCommand(false);
        }
        public Int32 ExecuteCommand(Boolean KeepConnectionOpen)
        {
            Int32 RecordAffected = 0;
            try
            {
                this.Open();
                RecordAffected = _Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_Command.Transaction == null && !KeepConnectionOpen)
                    this.Close();
            }
            return RecordAffected;
        }

        /// Executa o comando e  retorna um objeto
        /// </summary>
        /// <returns>object</returns>
        public Object ExecuteScalar()
        {
            return ExecuteScalar(false);
        }
        public Object ExecuteScalar(Boolean KeepConnectionOpen)
        {
            Object obj = null;
            try
            {
                this.Open();
                obj = _Command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_Command.Transaction == null && !KeepConnectionOpen)
                    this.Close();
            }
            return obj;
        }

        /// <summary>
        /// Método para executar um comando de incluir / alterar/ Excluir com transação
        /// </summary>
        /// <returns>Retorna o total de registros afetados</returns>
        public Int32 ExecuteTransactionalCommand()
        {
            Int32 RecordAffected = 0;
            try
            {
                if (_Connection.State == ConnectionState.Closed)
                {
                    this.Open();
                    _Transaction = _Connection.BeginTransaction();
                    _Command.Transaction = _Transaction;
                    RecordAffected = _Command.ExecuteNonQuery();
                    _Transaction.Commit();
                }
                return RecordAffected;
            }
            catch (Exception ex)
            {
                if (_Transaction != null)
                    _Transaction.Rollback();

                throw ex;
            }
            finally
            {
                if (_Connection.State == ConnectionState.Open)
                    _Connection.Close();
            }
        }

        #endregion

        #region Parametros

        /// <summary>
        /// Limpa os parâmetros do comando
        /// </summary>
        public void ClearParameters()
        {
            _Command.Parameters.Clear();
        }

        /// <summary>
        /// Propriedade para acessar a lista de parametros do comando
        /// </summary>
        public DbParameterCollection ParameterCollection
        {
            get
            {
                return this._Command.Parameters;
            }
        }

        /// <summary>
        /// Método para criar parametro na propriedade comando
        /// </summary>
        /// <param name="ParameterName">Nome do parametro</param>
        /// <param name="Type">Type do parametro</param>
        /// <param name="Value">Value</param>
        /// <param name="Size">Tamanho do parametro</param>
        public void CreateParameter(String ParameterName, DbType Type, Object Value, Int32 Size)
        {
            if (Value == null)
                Value = DBNull.Value;

            DbParameter _Parameter = providerFactory.CreateParameter();
            _Parameter.DbType = Type;
            _Parameter.Size = Size;
            _Parameter.ParameterName = ParameterIdentifier(ParameterName);
            _Parameter.Value = Value;
            _Command.Parameters.Add(_Parameter);
        }

        /// <summary>
        /// Método para criar parametro na propriedade comando
        /// </summary>
        /// <param name="ParameterName">Nome do parametro</param>
        /// <param name="Value">Value</param>
        public void CreateParameter(String ParameterName, Object Value)
        {
            if (Value == null)
                Value = DBNull.Value;

            DbParameter _Parameter = providerFactory.CreateParameter();
            _Parameter.ParameterName = ParameterIdentifier(ParameterName);
            _Parameter.Value = Value;
            _Command.Parameters.Add(_Parameter);
        }

        /// <summary>
        /// Método para criar parametro na propriedade comando
        /// </summary>
        /// <param name="ParameterName">Nome do parametro</param>
        /// <param name="Type">Type do parametro</param>
        /// <param name="Value">Value</param>
        public void CreateParameter(String ParameterName, DbType Type, Object Value)
        {
            if (Value == null)
                Value = DBNull.Value;

            DbParameter _Parameter = providerFactory.CreateParameter();
            _Parameter.DbType = Type;
            _Parameter.ParameterName = ParameterIdentifier(ParameterName);
            _Parameter.Value = Value;
            _Command.Parameters.Add(_Parameter);
        }

        /// <summary>
        /// Método para criar parametro na propriedade comando de OutPut
        /// </summary>
        /// <param name="ParameterName">Nome do parametro</param>
        /// <param name="Type">Type do parametro</param>
        /// <param name="Direcao">Se o parametro é de Input /InputOutput /Output /ReturnValue</param>
        public void CreateParameter(String ParameterName, DbType Type, ParameterDirection Direcao)
        {
            DbParameter _Parameter = providerFactory.CreateParameter();
            _Parameter.DbType = Type;
            _Parameter.ParameterName = ParameterIdentifier(ParameterName);
            _Parameter.Direction = Direcao;
            _Command.Parameters.Add(_Parameter);
        }

        /// <summary>
        /// Método para criar parametro na propriedade comando de OutPut
        /// </summary>
        /// <param name="ParameterName">Nome do parametro</param>
        /// <param name="Type">Type do parametro</param>
        /// <param name="Direcao">Se o parametro é de Input /InputOutput /Output /ReturnValue</param>
        /// <param name="Size">Tamanho do parametro</param>
        public void CreateParameter(String ParameterName, DbType Type, ParameterDirection Direction, Int32 Size)
        {
            DbParameter _Parameter = providerFactory.CreateParameter();
            _Parameter.DbType = Type;
            _Parameter.Size = Size;
            _Parameter.ParameterName = ParameterIdentifier(ParameterName);
            _Parameter.Direction = Direction;
            _Command.Parameters.Add(_Parameter);

        }

        /// <summary>
        /// Retorna o valor de um parametro
        /// </summary>
        /// <param name="ParameterName">Nome do parametro</param>
        /// <returns>retorna o valor do parametro</returns>
        public DbParameter GetParameter(String ParameterName)
        {
            return this._Command.Parameters[ParameterIdentifier(ParameterName)];
        }

        #endregion

        #region Comando
        /// <summary>
        /// Lê e escreve o texto do comando
        /// </summary>
        public String CommandText
        {
            set { _Command.CommandText = value; }
            get { return _Command.CommandText; }
        }
        /// <summary>
        /// Lê e escreve o tipo do comando
        /// </summary>
        public CommandType CommandType
        {
            set { _Command.CommandType = value; }
            get { return _Command.CommandType; }
        }

        public int CommandTimeout
        {
            set { _Command.CommandTimeout = value; }
            get { return _Command.CommandTimeout; }
        }
        #endregion

    }
}