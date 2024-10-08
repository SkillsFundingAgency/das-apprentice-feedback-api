using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Helpers;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Models;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Services
{
    public class DatabaseService
    {
        private const string DatabaseName = "SFA.DAS.ApprenticeFeedback.Database";
        private const string TestDatabaseName = DatabaseName + ".Test";

        public DatabaseService()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("connectionStrings.Local.json")
                .Build();

            Configuration = new Configuration
            {
                SqlConnectionString = configuration.GetConnectionString("SqlConnectionString"),
                SqlConnectionStringTest = configuration.GetConnectionString("SqlConnectionStringTest")
            };
        }

        public ApprenticeFeedbackDataContext TestContext
        {
            get
            {
                var applicationSettings = Options.Create(new ApplicationSettings { DbConnectionString = Configuration.SqlConnectionStringTest });
                var optionsBuilder = new DbContextOptionsBuilder<ApprenticeFeedbackDataContext>()
                    .UseSqlServer(applicationSettings.Value.DbConnectionString);

                return new ApprenticeFeedbackDataContext(
                    applicationSettings, 
                    optionsBuilder.Options,
                    null);
            }
        }

        public Configuration Configuration;

        public async Task CloneDatabase()
        {
            DropDatabase();

            using (var connection = new SqlConnection(Configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var comm = new SqlCommand
                {
                    Connection = connection,
                    CommandText =
                        @$"DBCC CLONEDATABASE ('{DatabaseName}', '{TestDatabaseName}'); " +
                         $"ALTER DATABASE [{TestDatabaseName}] SET READ_WRITE;"
                };
                var reader = comm.ExecuteReader();
                reader.Close();
            }

            await LookupDataHelper.AddLookupData();
        }

        public void DropDatabase()
        {
            using (var connection = new SqlConnection(Configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var comm = new SqlCommand
                {
                    Connection = connection,
                    CommandText =
                        @$"IF EXISTS(SELECT * FROM sys.databases WHERE Name = '{TestDatabaseName}') " +
                         $"BEGIN ALTER DATABASE [{TestDatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                         $"DROP DATABASE [{TestDatabaseName}]; END"
                };
                var reader = comm.ExecuteReader();
                reader.Close();
            }
        }

        public void Execute(string sql)
        {
            using (var connection = new SqlConnection(Configuration.SqlConnectionStringTest))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                connection.Execute(sql);
                connection.Close();
            }
        }

        public T Get<T>(string sql)
        {
            using (var connection = new SqlConnection(Configuration.SqlConnectionStringTest))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var result = connection.Query<T>(sql);
                connection.Close();
                return result.FirstOrDefault();
            }    
        }

        public IEnumerable<T> GetList<T>(string sql)
        {
            using (var connection = new SqlConnection(Configuration.SqlConnectionStringTest))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var result = connection.Query<T>(sql);
                connection.Close();
                return result;
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T, M>(string sql, M model) where T : TestModel
        {
            using (var connection = new SqlConnection(Configuration.SqlConnectionStringTest))
            {
                return await connection.QueryFirstOrDefaultAsync<T>(sql, param: model);
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql)
        {
            using (var connection = new SqlConnection(Configuration.SqlConnectionStringTest))
            {
                return await connection.QueryFirstOrDefaultAsync<T>(sql);
            }
        }

        public object ExecuteScalar(string sql)
        {
            using (var connection = new SqlConnection(Configuration.SqlConnectionStringTest))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var result = connection.ExecuteScalar(sql);
                connection.Close();

                return result;
            }
        }

        public void Execute<T>(string sql, T model)
        {
            using (var connection = new SqlConnection(Configuration.SqlConnectionStringTest))
            {
                connection.Execute(sql, model);
            }
        }
    }
}