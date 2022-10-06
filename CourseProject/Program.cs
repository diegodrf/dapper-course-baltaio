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
    }
}

