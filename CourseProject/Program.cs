using CourseProject.Exceptions;
using CourseProject.Models;
using CourseProject.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CourseProject;

public class Program
{
    private static string? _connectionString;

    public static async Task Main(string[] args)
    {
        var configurations = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        _connectionString = configurations.GetConnectionString("Blog");

        using var connection = new SqlConnection(_connectionString);

        await ReadUsers(connection);
        await ReadRoles(connection);
        await ReadTags(connection);
        await ReadCategories(connection);
        await ReadUsersWithRoles(connection);
    }

    public static async Task ReadUsers(SqlConnection connection)
    {
        var repository = new Repository<User>(connection);
        var items = await repository.GetAsync();
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }
    public static async Task ReadRoles(SqlConnection connection)
    {
        var repository = new Repository<Role>(connection);
        var items = await repository.GetAsync();
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }
    public static async Task ReadTags(SqlConnection connection)
    {
        var repository = new Repository<Tag>(connection);
        var items = await repository.GetAsync();
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }
    public static async Task ReadCategories(SqlConnection connection)
    {
        var repository = new Repository<Category>(connection);
        var items = await repository.GetAsync();
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }
    public static async Task ReadUsersWithRoles(SqlConnection connection)
    {
        var repository = new UserRepository(connection);
        var users = await repository.GetWithRoles();
        foreach (var user in users)
        {
            Console.WriteLine(user);
            if (!user.Roles.IsNullOrEmpty())
            {
                foreach (var role in user.Roles)
                {
                    Console.WriteLine($"\t- {role}");
                }
            }
        }
    }
}