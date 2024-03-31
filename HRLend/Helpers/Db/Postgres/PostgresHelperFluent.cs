namespace Helpers.Db.Postgres
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Helpers.Db.Postgres;

    public class PostgresHelperFluent
    {
        protected readonly string _connectionString;
        protected readonly string _sqlText;
        protected bool _isStoredProcedure;
        protected bool _isOutputParameter;
        protected List<KeyValuePair<string, object>> _parameters;
 
        public PostgresHelperFluent(string connectionString, string sqlText)
        {
            _connectionString = connectionString;
            _sqlText = sqlText;
            _isStoredProcedure = true;
            _isOutputParameter = false;
            _parameters = new List<KeyValuePair<string, object>>();
        }

        #region fluent methods

        public PostgresHelperFluent AsStoredProcedure()
        {
            _isStoredProcedure = true;
            return this;
        }

        public PostgresHelperFluent AsSqlText()
        {
            _isStoredProcedure = false;
            return this;
        }

        public PostgresHelperFluent AddParameter(string name, object value)
        {
            _parameters.Add(new KeyValuePair<string, object>(name, value));
            return this;
        }
        public PostgresHelperFluent AddParameter(string name, string value)
        {

            _parameters.Add(new KeyValuePair<string, object>(name, value ?? string.Empty));
            return this;
        }

        public PostgresHelperFluent AddParameterNullable(string name, object value)
        {
            _parameters.Add(new KeyValuePair<string, object>(name, value ?? DBNull.Value));
            return this;
        }

        public PostgresHelperFluent AddOutputParameter(string name, object value)
        {
            _parameters.Add(new KeyValuePair<string, object>(name, new OutPutValue { Value = value ?? DBNull.Value }));
            return this;
        }

        public PostgresHelperFluent AddParameters(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            _parameters.AddRange(parameters);
            return this;
        }

        #endregion

        #region finish methods

        public void ExecuteNonQuery()
        {
            _connectionString.ExecuteNonQuery(_sqlText, isStoredProcedure: _isStoredProcedure, parameters: _parameters);
        }

        public TResult ExecuteScalar<TResult>()
        {
            return _connectionString.ExecuteScalar<TResult>(_sqlText, isStoredProcedure: _isStoredProcedure, parameters: _parameters);
        }

        #endregion
    }

    public class SqlServerHelperFluent<TEntity> : PostgresHelperFluent
    {
        protected List<Action<IDataRecord, IList<TEntity>>> _converters;

        public SqlServerHelperFluent(string connectionString, string sqlText) : base (connectionString, sqlText)
        {
            _converters = new List<Action<IDataRecord, IList<TEntity>>>();
        }

        #region base fluent methods

        public new SqlServerHelperFluent<TEntity> AddParameter(string name, object value)
        {
       
            base.AddParameter(name, value);
            return this;
        }
        public new SqlServerHelperFluent<TEntity> AddParameterNullable(string name, object value)
        {

            base.AddParameterNullable(name, value);
            return this;
        }

        public new SqlServerHelperFluent<TEntity> AddParameters(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            base.AddParameters(parameters);
            return this;
        }

        public new SqlServerHelperFluent<TEntity> AsStoredProcedure()
        {
            base.AsStoredProcedure();
            return this;
        }

        public new SqlServerHelperFluent<TEntity> AsSqlText()
        {
            base.AsSqlText();
            return this;
        }

        #endregion

        public SqlServerHelperFluent<TEntity> AddConverter(Action<IDataRecord, IList<TEntity>> converter)
        {
            _converters.Add(converter);
            return this;
        }

        public IEnumerable<TEntity> ExecuteSelectMany()
        {
            return _connectionString.ExecuteSelectMany<TEntity>(_sqlText, isStoredProcedure: _isStoredProcedure, parameters: _parameters, converters: _converters.ToArray());
        }

        public IEnumerableWithPage<TEntity> ExecuteSelectManyWithPage()
        {
            return _connectionString.ExecuteSelectManyWithPage<TEntity>(_sqlText, isStoredProcedure: _isStoredProcedure, parameters: _parameters, converters: _converters.ToArray());
        }
        public IEnumerableWithOffer<TEntity> ExecuteSelectManyWithOffer()
        {
            return _connectionString.ExecuteSelectManyWithOffer<TEntity>(_sqlText, isStoredProcedure: _isStoredProcedure, parameters: _parameters, converters: _converters.ToArray());
        }
    }

    public static class SqlServerHelperFluentExtentions
    {
        public static PostgresHelperFluent PrepareExecute(this string connectionString, string sqlText)
        {
            return new PostgresHelperFluent(connectionString, sqlText);
        }

        public static SqlServerHelperFluent<TEntity> PrepareExecute<TEntity>(this string connectionString, string sqlText)
        {
            return new SqlServerHelperFluent<TEntity>(connectionString, sqlText);
        }
    }
}