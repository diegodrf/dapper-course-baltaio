using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CourseProject;

public class Program
{
    public static void Main(string[] args)
    {
        var configurations = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var connectionString = configurations.GetConnectionString("Blog");
        Console.WriteLine(connectionString);

        using var conn = new SqlConnection(connectionString);
        var result = conn.Query("SELECT * FROM Post");
        result.ToList().ForEach(Console.WriteLine);
    }
}

