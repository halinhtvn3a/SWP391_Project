using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace API.Helper
{
    public class TimeslotCleanupManager
    {
        private readonly string _connectionString;


        public TimeslotCleanupManager(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CourtCallerDb");
        }


        public async Task CleanupPendingTimeslots()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        SqlCommand command = new SqlCommand(
                            "DELETE FROM timeslots WHERE status = 'Pending' AND DATEDIFF(minute, Created_at, GETDATE()) > 10",
                            connection, transaction);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        Console.WriteLine($"{rowsAffected} pending timeslots have been deleted.");

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }
    }
}
