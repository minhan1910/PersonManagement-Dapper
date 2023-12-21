using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DotnetAPI_Project.Data
{
    public class DataContextDapper
    {
        private readonly IConfiguration _configuration;
        private readonly string CONNECTION_STRING;

        public DataContextDapper(IConfiguration configuration)
        {
            _configuration = configuration;
            CONNECTION_STRING = _configuration.GetConnectionString("DefaultConnection")!;
        }

        public IEnumerable<T> LoadData<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(CONNECTION_STRING);
            return dbConnection.Query<T>(sql);
        }

        public T LoadDataSingle<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(CONNECTION_STRING);
            return dbConnection.QuerySingle<T>(sql);
        }

        public bool ExecuteSql(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(CONNECTION_STRING);
            return dbConnection.Execute(sql) > 0;
        }

        public int ExecuteSqlWithRowCount(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(CONNECTION_STRING);
            return dbConnection.Execute(sql);
        }

        // code like ADO.NET
        public bool ExecuteSqlWithParamaters(string sql, params SqlParameter[] parameters)
        {
            SqlConnection dbConnection = new(CONNECTION_STRING);

            dbConnection.Open();

            SqlCommand commandWithParams = new(sql);
            parameters.ToList()
                      .ForEach(parameter => commandWithParams.Parameters.Add(parameter));  
            commandWithParams.Connection = dbConnection;

            int rowAffected = commandWithParams.ExecuteNonQuery();

            dbConnection.Close();

            return rowAffected > 0;
        }
    }
}
