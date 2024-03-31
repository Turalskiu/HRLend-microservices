namespace Helpers.Db.Postgres
{
    using Helpers.Db.Postgres;
    using Npgsql;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class OutPutValue
    {
        public object Value;
    }

    public static class PostgresHelper
    {
        /// <summary>
        /// Проверка на наличие столбца в запросе
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HasColumn(this IDataReader reader, string name)
        {
            string nameLow = name.ToLower();
            for (int n = 0; n < reader.FieldCount; n++)
            {
                if (reader.GetName(n).ToLower() == nameLow) return true;
            }
            return false;
        }


        public static NpgsqlCommand CreateCommand(this string connectionString,
            string sql,
            IEnumerable<KeyValuePair<string, object>> parameters = null,
            IEnumerable<NpgsqlParameter> specialParameters = null
        )
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException("connectionString");
            if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentNullException("sql");

            NpgsqlCommand command = new NpgsqlConnection(connectionString).CreateCommand();
            command.CommandTimeout = 60 * 60 * 60;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = sql;

            if (parameters != null && parameters.Any())
            {
                parameters.ToList().ForEach(
                    x => { command.Parameters.AddWithValue(x.Key, x.Value ?? DBNull.Value); }
                );
            }
            if (specialParameters != null && specialParameters.Any())
            {
                specialParameters.ToList().ForEach(
                    x => { command.Parameters.Add(x); }
                );
            }

            return command;
        }


        public static NpgsqlCommand CreateCommand(this string connectionString, string sql, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException("connectionString");
            if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentNullException("sql");

            NpgsqlCommand command = new NpgsqlConnection(connectionString).CreateCommand();
            command.CommandTimeout = 60 * 60 * 60;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = sql;

            if (parameters != null && parameters.Any())
            {
                parameters.ToList().ForEach(
                    x => { command.Parameters.AddWithValue(x.Key, x.Value ?? DBNull.Value); }
                );
            }

            return command;
        }


        public static NpgsqlCommand CreateCommand(this string connectionString, string sql, bool isProcedure = true, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException("connectionString");
            if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentNullException("sql");

            NpgsqlCommand command = new NpgsqlConnection(connectionString).CreateCommand();
            command.CommandTimeout = 60 * 60 * 60;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = sql;
          
            if (parameters != null && parameters.Any())
            {
                parameters.ToList().ForEach(
                    x => { command.Parameters.AddWithValue(x.Key, x.Value ?? DBNull.Value); }
                );
            }

            return command;
        }

        public static NpgsqlCommand CreateCommandWithOutput(this string connectionString, string sql, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException("connectionString");
            if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentNullException("sql");

            NpgsqlCommand command = new NpgsqlConnection(connectionString).CreateCommand();
            command.CommandTimeout = 60 * 60 * 60;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = sql;

            if (parameters != null && parameters.Any())
            {
                parameters.ToList().ForEach(
                    x =>
                    {
                        object value = x.Value;
                        bool isOutPut = false;
                        if (x.Value is OutPutValue)
                        {
                            value = ((OutPutValue)x.Value).Value;
                            isOutPut = true;
                        }
                        var parameter = command.Parameters.AddWithValue(x.Key, value ?? DBNull.Value);
                        if (isOutPut)
                        {
                            parameter.Direction = ParameterDirection.Output;
                        }
                    }
                );

            }

            return command;
        }

        public static void ExecuteNonQuery(this string connectionString, string sql, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            NpgsqlCommand command = connectionString.CreateCommand(sql, parameters);
            using (command.Connection)
            {
                command.Connection.Open();
                command.ExecuteNonQuery();
                command.Connection.Close();
            }
        }

        public static void ExecuteNonQuery(this string connectionString,
            string sql,
            IEnumerable<KeyValuePair<string, object>> parameters = null,
            IEnumerable<NpgsqlParameter> specialParameters = null
        )
        {
            NpgsqlCommand command = connectionString.CreateCommand(sql, parameters, specialParameters);
            using (command.Connection)
            {
                command.Connection.Open();
                command.ExecuteNonQuery();
                command.Connection.Close();
            }
        }

        public static void ExecuteNonQuery(this string connectionString, string sql, bool isStoredProcedure = false, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            NpgsqlCommand command = connectionString.CreateCommand(sql, parameters);
            using (command.Connection)
            {
                command.Connection.Open();
                command.ExecuteNonQuery();
                command.Connection.Close();
            }
        }

        public static object ExecuteScalar(this string connectionString, string sql, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            object returnScalar;

            NpgsqlCommand command = connectionString.CreateCommand(sql, parameters);
            using (command.Connection)
            {
                command.Connection.Open();
                returnScalar = command.ExecuteScalar();
                command.Connection.Close();
            }

            return returnScalar;
        }

        public static object ExecuteScalar(this string connectionString, 
            string sql, 
            IEnumerable<KeyValuePair<string, object>> parameters = null,
            IEnumerable<NpgsqlParameter> specialParameters = null)
        {
            object returnScalar;

            NpgsqlCommand command = connectionString.CreateCommand(sql, parameters, specialParameters);
            using (command.Connection)
            {
                command.Connection.Open();
                returnScalar = command.ExecuteScalar();
                command.Connection.Close();
            }

            return returnScalar;
        }

        public static IEnumerable<KeyValuePair<string, object>> ExecuteOutputParameters(this string connectionString, string sql, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            List<KeyValuePair<string,object>> result = new List<KeyValuePair<string, object>>();

            NpgsqlCommand command = connectionString.CreateCommandWithOutput(sql, parameters);
            using (command.Connection)
            {
                command.Connection.Open();
                command.ExecuteNonQuery();
                command.Connection.Close();

                foreach(NpgsqlParameter parameter in command.Parameters)
                {
                    if(parameter.Direction == ParameterDirection.Output)
                    {
                        result.Add(new KeyValuePair<string, object>(parameter.ParameterName, parameter.Value));
                    }
                }
            }

            return result;
        }

        public static TResult ExecuteScalar<TResult>(this string connectionString, string sql, bool isStoredProcedure = false, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            object returnScalar;

            NpgsqlCommand command = connectionString.CreateCommand(sql, isStoredProcedure, parameters);
            using (command.Connection)
            {
                command.Connection.Open();
                returnScalar = command.ExecuteScalar();
                command.Connection.Close();
            }
            return (TResult)returnScalar;
        }

        public static TResult ExecuteScalar<TResult>(this string connectionString, 
            string sql, bool isStoredProcedure = false, 
            IEnumerable<KeyValuePair<string, object>> parameters = null,
            IEnumerable<NpgsqlParameter> specialParameters = null)
        {
            object returnScalar;

            NpgsqlCommand command = connectionString.CreateCommand(sql, parameters, specialParameters);
            using (command.Connection)
            {
                command.Connection.Open();
                returnScalar = command.ExecuteScalar();
                command.Connection.Close();
            }
            return (TResult)returnScalar;
        }


        public static IEnumerable<TEntity> ExecuteSelect<TEntity>(this string connectionString, string sql, Func<IDataRecord, TEntity> converter, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            if (converter == null) throw new ArgumentNullException("converter");

            NpgsqlCommand command = connectionString.CreateCommand(sql, parameters);
            using (command.Connection)
            {
                command.Connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                {
                    IList<TEntity> selectResult = new List<TEntity>();
                    while (reader.Read())
                    {
                        selectResult.Add(converter(reader));
                    }
                    return selectResult;
                }
            }
        }

        public static IEnumerable<TEntity> ExecuteSelect<TEntity>(this string connectionString, Func<IDataRecord, TEntity> converter, string sql, bool isStoredProcedure = false, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            if (converter == null) throw new ArgumentNullException("converter");

            NpgsqlCommand command = connectionString.CreateCommand(sql, isStoredProcedure, parameters);
            command.CommandTimeout = command.Connection.ConnectionTimeout;
            using (command.Connection)
            {
                command.Connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                {
                    IList<TEntity> selectResult = new List<TEntity>();
                    while (reader.Read())
                    {
                        selectResult.Add(converter(reader));
                    }
                    return selectResult;
                }
            }
        }

        public static DataTable ExecuteDataTable(this string connectionString, string sql, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            NpgsqlCommand command = connectionString.CreateCommand(sql, parameters);

            using (command.Connection)
            {
                command.Connection.Open();
                using (var adapter = new NpgsqlDataAdapter(command))
                {
                    var table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
        }

        public static DataTable ExecuteDataTable(this string connectionString, string sql, bool isStoredProcedure = false, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            NpgsqlCommand command = connectionString.CreateCommand(sql, isStoredProcedure, parameters);

            using (command.Connection)
            {
                command.Connection.Open();
                using (var adapter = new NpgsqlDataAdapter(command))
                {
                    var table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
        }

        public static TValue Get<TValue>(this IDataRecord record, string fieldName)
        {
            try
            {
                //if ((record[fieldName] == DBNull.Value) && (typeof(TValue) == typeof(object)) )  return (TValue)null;

                return (TValue)record[fieldName];
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException(string.Format("{2} {0} нельзя привести к {1}", record[fieldName].GetType().FullName, typeof(TValue).FullName, fieldName), ex);
            }
        }
        public static  Decimal GetDecimalX(this IDataRecord record, string fieldName)
        {
            Decimal rret = 0;
            if (record[fieldName] != DBNull.Value)
            {
                if (record[fieldName].GetType().Name == "Int32")
                {
                    rret = Convert.ToDecimal(record[fieldName]);                    
                }
                else
                {
                    rret = record.Get<Decimal> (fieldName);
                }
            }

            return rret;
        }

        public static string CreateParameterListString(this IEnumerable<KeyValuePair<string, object>> parameters)
        {
            return string.Join(", ", parameters.Select(x => x.Key).ToArray());
        }


        public static TValue GetN<TValue>(this IDataRecord record, string fieldName)
        {
            return record[fieldName] != DBNull.Value ? record.Get<TValue>(fieldName) : default(TValue);
        }

        public static IEnumerable<TEntity> ExecuteSelectMany<TEntity>(this string connectionString, string sql,
            bool isStoredProcedure = false, IEnumerable<KeyValuePair<string, object>> parameters = null,
            params Action<IDataRecord, IList<TEntity>>[] converters)
        {
            if (converters == null) throw new ArgumentNullException("converters");
            NpgsqlCommand command = connectionString.CreateCommand(sql, isStoredProcedure, parameters);
            command.CommandTimeout = command.Connection.ConnectionTimeout;
            using (command.Connection)
            {
                command.Connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                {
                    IList<TEntity> selectResult = new List<TEntity>();
                    foreach (var converter in converters)
                    {
                        while (reader.Read())
                        {
                            converter(reader, selectResult);
                        }
                        reader.NextResult();
                    }

                    return selectResult;
                }
            }
        }

        public static IEnumerableWithPage<TEntity> ExecuteSelectManyWithPage<TEntity>(
            this string connectionString, 
            string sql,
            bool isStoredProcedure = false, 
            IEnumerable<KeyValuePair<string, object>> parameters = null,
            params Action<IDataRecord, IList<TEntity>>[] converters
            )
        {
            if (converters == null) throw new ArgumentNullException("converters");

            NpgsqlCommand command = connectionString.CreateCommand(sql, isStoredProcedure, parameters);
            command.CommandTimeout = command.Connection.ConnectionTimeout;
            using (command.Connection)
            {
                command.Connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                {
                    var result = new ListWithPage<TEntity>();
                    foreach (var converter in converters)
                    {
                        while (reader.Read())
                        {
                            converter(reader, result);
                        }
                        reader.NextResult();
                    }

                    // считывание данных постраничного вывода
                    while (reader.Read())
                    {
                        if (reader.HasColumn("TotalRows")) result.TotalRows = reader.Get<int>("TotalRows");
                        if (reader.HasColumn("PageNo")) result.PageNo = reader.Get<int>("PageNo");
                        if (reader.HasColumn("PageSize")) result.PageSize = reader.Get<int>("PageSize");
                        if (reader.HasColumn("Sort")) result.Sort = reader.Get<string>("Sort");
                    }

                    return result;
                }
            }
        }

        public static IEnumerableWithOffer<TEntity> ExecuteSelectManyWithOffer<TEntity>(
            this string connectionString,
            string sql,
            bool isStoredProcedure = false,
            IEnumerable<KeyValuePair<string, object>> parameters = null,
            params Action<IDataRecord, IList<TEntity>>[] converters
            )
        {
            if (converters == null) throw new ArgumentNullException("converters");

            NpgsqlCommand command = connectionString.CreateCommand(sql, isStoredProcedure, parameters);
            command.CommandTimeout = command.Connection.ConnectionTimeout;
            using (command.Connection)
            {
                command.Connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                {
                    var result = new ListWithOffer<TEntity>();
                    foreach (var converter in converters)
                    {
                        while (reader.Read())
                        {
                            converter(reader, result);
                        }
                        reader.NextResult();
                    }

                    // считывание данных постраничного вывода
                    while (reader.Read())
                    {
                        if (reader.HasColumn("TotalRows")) result.TotalRows = reader.Get<int>("TotalRows");
                        if (reader.HasColumn("PageNo")) result.Start = reader.Get<int>("Start");
                        if (reader.HasColumn("PageSize")) result.Lenght = reader.Get<int>("Length");
                        if (reader.HasColumn("Sort")) result.Sort = reader.Get<string>("Sort");
                    }

                    return result;
                }
            }
        }

        #region Add -----------------------------------------------------------



        #endregion

    }
}