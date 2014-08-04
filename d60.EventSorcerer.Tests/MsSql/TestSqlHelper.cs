﻿using System;
using System.Data.SqlClient;
using System.Linq;
using d60.EventSorcerer.MsSql;

namespace d60.EventSorcerer.Tests.MsSql
{
    public class TestSqlHelper
    {
        public const string ConnectionStringName = "testdb";

        public static void EnsureTestDatabaseExists()
        {
            var connectionString = SqlHelper.GetConnectionString(ConnectionStringName);
            var databaseName = SqlHelper.GetDatabaseName(connectionString);
            var masterConnectionString = connectionString.Replace(databaseName, "master");

            try
            {
                using (var conn = new SqlConnection(masterConnectionString))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = string.Format(@"
BEGIN
    CREATE DATABASE [{0}]
END

", databaseName);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException exception)
            {
                if (exception.Errors.Cast<SqlError>().Any(e => e.Number == 1801))
                {
                    Console.WriteLine("Test database '{0}' already existed", databaseName);
                    return;
                }
                throw;
            }
        }

        public static void DropTable(string connectionString, string tableName)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = string.Format(@"
BEGIN
    DROP TABLE [{0}]
END

", tableName);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException exception)
            {
                if (exception.Errors.Cast<SqlError>().Any(e => e.Number == 3701))
                {
                    Console.WriteLine("Table '{0}' was already gone", tableName);
                    return;
                }
                throw;
            }
        }

    }
}