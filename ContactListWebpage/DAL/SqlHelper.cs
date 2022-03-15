using Microsoft.Data.SqlClient;
using System.Data;

namespace ContactListWebpage.DAL
{
    static class SqlHelper
    {
        public static Int32 ExecuteNonQuery(String connectionString, String commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static int CreateCommand( String connectionString, string commandText )
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = commandText;

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static Object ExecuteScalar(String connectionString, String commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        public static SqlDataReader ExecuteReader(String connectionString, String commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(connectionString);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = commandType;
                cmd.Parameters.AddRange(parameters);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                return reader;
            }
        }
    }
    public class SPBuilder
    {
        private static string connectionString = "Server=(localdb)\\mssqllocaldb;Database=aspnet-ContactListWebpage-53bc9b9d-9d6a-45d4-8429-2a2761773502;Trusted_Connection=True;MultipleActiveResultSets=true";
        private string command;
        private List<SqlParameter> sqlParameters = new List<SqlParameter>();
        public SPBuilder( string command )
        {
            this.command = command;
        }

        public SPBuilder Add( string key, string value )
        {
            sqlParameters.Add( new SqlParameter("@" + key, value ) );
            return this;
        }
        public SPBuilder Add( string key, int value )
        {
            sqlParameters.Add( new SqlParameter( "@" + key, value ) );
            return this;
        }
        public SPBuilder Add( string key, DateTime value )
        {
            sqlParameters.Add( new SqlParameter("@" + key, value ) );
            return this;
        }
        public SPBuilder Add(string key, object value)
        {
            sqlParameters.Add(new SqlParameter("@" + key, value));
            return this;
        }
        public void Execute()
        {
            SqlHelper.ExecuteNonQuery(SPBuilder.connectionString, this.command, CommandType.StoredProcedure, sqlParameters.ToArray());
        }
    }
}
