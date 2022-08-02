using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static ParserLesegaisRu.DbWork.DbCommand;

namespace ParserLesegaisRu.DbWork
{
    public class DbWorker
    {
        public readonly string _connectionString = "Server=.\\; Trusted_Connection=True;";
        public const string _dbTitle = "Database =";

        public DbWorker()
        {
            EnsureDbCreation();
            _connectionString = _connectionString + _dbTitle + DbName;
            InitializeDateBase();
        }

        private void EnsureDbCreation()
        {
            using var connection = new SqlConnection(_connectionString);
            ExecuteSqlQuery(connection, CreateDataBaseQuery);
        }

        private void InitializeDateBase()
        {
            using var connection = new SqlConnection(_connectionString);

            ExecuteSqlQuery(connection, CreateDeclarationTableQuery);

            if (!IsExistStoredProcedure(connection))
            {
                ExecuteSqlQuery(connection, CreateAddDeclarationSoredProcedure);
            }
        }


        private void ExecuteSqlQuery(SqlConnection connection, string query)
        {
            using var command = new SqlCommand(query, connection);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            command.ExecuteNonQuery();
        }

        private bool IsExistStoredProcedure(SqlConnection connection)
        {
            using var command = new SqlCommand(CheckIsStoredProcedureExist, connection);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return (bool)command.ExecuteScalar();
        }

        private void AddDeal(Declaration declaration, SqlConnection connection)
        {

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand(AddDeclarationProcedureName, connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(DealNumber, SqlDbType.NVarChar).Value = declaration.DealNumber ?? "";
            command.Parameters.Add(SellerName, SqlDbType.NVarChar).Value = declaration.SellerName ?? "";
            command.Parameters.Add(SellerINN, SqlDbType.NVarChar).Value = declaration.SellerINN ?? "";
            command.Parameters.Add(BuyerName, SqlDbType.NVarChar).Value = declaration.BuyerName ?? "";
            command.Parameters.Add(BuyerINN, SqlDbType.NVarChar).Value = declaration.BuyerINN ?? "";
            command.Parameters.Add(DealDate, SqlDbType.Date).Value = declaration.DealDate ?? "";
            command.Parameters.Add(BuyerVolume, SqlDbType.NVarChar).Value = declaration.BuyerVolume ?? "";
            command.Parameters.Add(SellerVolume, SqlDbType.NVarChar).Value = declaration.SellerVolume ?? "";

            //command.ExecuteNonQuery();
            command.ExecuteScalar();
            //Console.WriteLine(declaration.SellerName+" Result = "+ command.ExecuteScalar());


        }

        public void AddDealRange(List<Declaration> declarations)
        {
            Console.WriteLine(Environment.NewLine + "Старт добавления полученных строк в базу: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

            using var connection = new SqlConnection(_connectionString);
            try
            {
                foreach (var declaration in declarations)
                {
                    AddDeal(declaration, connection);
                }
            }
            finally
            {
                connection.Close();
            }

            Console.WriteLine("Финиш добавления полученных строк в базу: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
        }
    }
}
