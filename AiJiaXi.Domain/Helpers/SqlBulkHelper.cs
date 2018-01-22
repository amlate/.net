using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace AiJiaXi.Domain.Helpers
{
    public class SqlBulkHelper
    {
        private static readonly string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private static DataTable PrepareDataTable<T>(IEnumerable<T> entities, params string[] ignores)
            where T : class 
        {
            Type type = typeof(T);
            List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
            Array.ForEach(type.GetProperties(), p => propertyInfos.Add(p));
            
            DataTable table = new DataTable();
            foreach (var propertyInfo in propertyInfos)
            {
                var p_type = propertyInfo.PropertyType;

                if (ignores.Contains(propertyInfo.Name))
                    continue;

                if (propertyInfo.PropertyType == typeof(long?))
                {
                    p_type = typeof (long);
                }

                table.Columns.Add(propertyInfo.Name, p_type);
            }

            object[] values = new object[table.Columns.Count];
            foreach (var entity in entities)
            {
                int j = 0;
                for (int i = 0; i < propertyInfos.Count; i++)
                {
                    if (ignores.Contains(propertyInfos[i].Name))
                        continue;

                    values[j] = propertyInfos[i].GetValue(entity,null);
                    j++;
                }

                table.Rows.Add(values);
            }

            return table;
        }

        public static void SqlBulkCopy<T>(IEnumerable<T> entities, string tableName, params string[] ignores)
            where T : class 
        {
            DataTable dataTable = PrepareDataTable<T>(entities, ignores);

            using (var connection = new SqlConnection(connStr))
            {
                SqlTransaction transaction = null;
                connection.Open();
                try
                {
                    transaction = connection.BeginTransaction();
                    using (var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                    {
                        sqlBulkCopy.BatchSize = dataTable.Rows.Count;

                        sqlBulkCopy.DestinationTableName = tableName;
                        Array.ForEach(typeof(T).GetProperties(),
                            p =>
                                {
                                    if (!ignores.Contains(p.Name))
                                    {
                                        sqlBulkCopy.ColumnMappings.Add(p.Name, p.Name);
                                    }
                                });
                        
                        sqlBulkCopy.WriteToServer(dataTable);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    throw;
                }
            }
        }
    }
}