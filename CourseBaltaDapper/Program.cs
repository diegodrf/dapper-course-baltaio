using CourseBaltaDapper.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Linq.Expressions;
using System.Transactions;

namespace CourseBaltaDapper;

public class Program
{
    private static readonly string _connectionString = "Data Source=BaltaDatabase.sqlite";
    private static readonly string _defaultCategoryId = Guid.NewGuid().ToString("D");
    private static readonly string _defaultStudentId = Guid.NewGuid().ToString("D");
    public static async Task Main(string[] args)
    {
        using var connection = new SqliteConnection(_connectionString);

        await CreateTables(connection);
        await Insert(connection);
        await Select(connection);
        await Update(connection);
        await InsertMany(connection);
        await Select(connection);
        await InsertScalar(connection);
        await SelectOneToOne(connection);
        await SelectOneToMany(connection);
        await SelectMultiple(connection);
        await SelectIn(connection);
        await SelectLike(connection, "digi");
        await Transaction();

    }
    public static async Task CreateTables(SqliteConnection connection)
    {
        await connection.ExecuteAsync(@"DROP TABLE IF EXISTS CategoryStudent");
        await connection.ExecuteAsync(@"DROP TABLE IF EXISTS Student");
        await connection.ExecuteAsync(@"DROP TABLE IF EXISTS Category");

        await connection.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS Category (
            Id TEXT PRIMARY KEY, 
            Title TEXT NOT NULL,
            Url TEXT NOT NULL,
            Description TEXT NOT NULL,
            Summary TEXT NOT NULL,
            Featured INTEGER NOT NULL
        );");

        await connection.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS Student (
            Id TEXT PRIMARY KEY, 
            Name TEXT NOT NULL
        );");

        await connection.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS CategoryStudent (
            CategoryId TEXT NOT NULL,
            StudentId TEXT NOT NULL
        );");
    }
    public static async Task Insert(SqliteConnection connection)
    {
        var category = new Category
        {
            Id = _defaultCategoryId,
            Title = "Amazon AWS",
            Url = "https://www.xpto.com.br/aws",
            Description = "Categoria destinada a serviços AWS",
            Summary = "AWS Cloud",
            Featured = false
        };

        var insertCategory = @"INSERT INTO Category 
            (Id, Title, Url, Description, Summary, Featured) 
            VALUES 
            (@Id, @Title, @Url, @Description, @Summary, @Featured)";

        await connection.ExecuteAsync(insertCategory, new
        {
            category.Id,
            category.Title,
            category.Url,
            category.Description,
            category.Summary,
            Featured = category.Featured == true ? 1 : 0
        });

        await connection.ExecuteAsync(@"INSERT INTO Student VALUES (@Id, @Name)", new { Id = _defaultStudentId, Name = "Diego" });
        await connection.ExecuteAsync(
            @"INSERT INTO CategoryStudent VALUES (@CategoryId, @StudentId)",
            new { CategoryId = _defaultCategoryId, StudentId = _defaultStudentId }
            );
    }
    public static async Task InsertMany(SqliteConnection connection)
    {
        var categories = new List<Category>()
        {   new() {
                Id = Guid.NewGuid().ToString("D"),
                Title = "Digital Ocean",
                Url = "https://www.xpto.com.br/do",
                Description = "Categoria destinada a serviços Digital Ocean",
                Summary = "Digital Ocean",
                Featured = false
            },

            new() {
                Id = Guid.NewGuid().ToString("D"),
                Title = "Heroku",
                Url = "https://www.xpto.com.br/Heroku",
                Description = "Categoria destinada a serviços Heroku",
                Summary = "Heroku",
                Featured = true
            },

        };

        var insertCategory = @"INSERT INTO Category 
            (Id, Title, Url, Description, Summary, Featured) 
            VALUES 
            (@Id, @Title, @Url, @Description, @Summary, @Featured)";

        await connection.ExecuteAsync(insertCategory, categories);
    }
    public static async Task InsertScalar(SqliteConnection connection)
    {
        try
        {
            var category = new Category
            {
                Id = _defaultCategoryId,
                Title = "Amazon AWS",
                Url = "https://www.xpto.com.br/aws",
                Description = "Categoria destinada a serviços AWS",
                Summary = "AWS Cloud",
                Featured = false
            };

            var insertCategory = @"INSERT INTO Category 
            (Id, Title, Url, Description, Summary, Featured) 
            OUTPUT inserted.Id
            VALUES 
            (NEWID(), @Title, @Url, @Description, @Summary, @Featured)";

            var insertedId = await connection.ExecuteScalarAsync<string>(insertCategory, new
            {
                Title = "HOST Gator",
                Url = "https://www.xpto.com.br/hg",
                Description = "Categoria destinada a serviços HOST Gator",
                Summary = "AHOST Gator",
                Featured = false
            });
            Console.WriteLine($"ExecuteScalar: {insertedId}");
        }
        catch (Microsoft.Data.Sqlite.SqliteException)
        {
            Console.WriteLine("INSERT SCALAR: \"OUTPUT inserted.Id\" inside INSERT statement don't work in SQLite.");
        }

    }
    public static async Task Select(SqliteConnection connection)
    {
        var categories = await connection.QueryAsync<Category>("SELECT Id, Title FROM Category");

        foreach (var c in categories)
        {
            Console.WriteLine($"{c.Id} - {c.Title}");

        }
    }
    public static async Task Update(SqliteConnection connection)
    {
        var rows = await connection.ExecuteAsync("UPDATE Category SET Title = @NewTitle WHERE Id = @Id",
            new { NewTitle = "Google Cloud", Id = _defaultCategoryId });

        Console.WriteLine($"UPDATE: {rows} affected row(s)");
    }
    public static async Task SelectOneToOne(SqliteConnection connection)
    {
        var items = await connection.QueryAsync<Category, Student, CategoryStudent>(@"SELECT
                c.Id,
                c.Title,
                c.Url,
                c.Summary,
                c.Description,
                c.Featured,
                s.Id,
                s.Name
            FROM 
                CategoryStudent AS cs
            INNER JOIN 
                Student AS s 
                ON s.Id = cs.StudentId
            INNER JOIN 
                Category AS c 
                ON c.Id = cs.CategoryId
            WHERE
                cs.StudentId = @StudentId;",
                (category, student) => new CategoryStudent { Student = student, Category = category },
                splitOn: "Id",
                param: new { StudentId = _defaultStudentId }
                );

        foreach (var i in items)
        {
            Console.WriteLine($"{i.Student.Name} => {i.Category.Title}");
        }
    }
    public static async Task SelectOneToMany(SqliteConnection connection)
    {
        var newStudentId = Guid.NewGuid().ToString("D");
        await connection.ExecuteAsync(@"INSERT INTO Student VALUES (@Id, @Name)", new { Id = newStudentId, Name = "Camila" });
        await connection.ExecuteAsync(
            @"INSERT INTO CategoryStudent (CategoryId, StudentId)
            SELECT
                Id,
                @StudentId
            FROM Category",
            new { StudentId = newStudentId }
            );


        var students = new List<Student>();

        var items = await connection.QueryAsync<Category, Student, Student>(@"SELECT
                c.Id,
                c.Title,
                c.Url,
                c.Summary,
                c.Description,
                c.Featured,
                s.Id,
                s.Name
            FROM 
                CategoryStudent AS cs
            INNER JOIN 
                Student AS s 
                ON s.Id = cs.StudentId
            INNER JOIN 
                Category AS c 
                ON c.Id = cs.CategoryId
            WHERE
                s.Name = @StudentName;",
                (category, student) =>
                {
                    var s = students.Where(_ => student.Equals(_)).FirstOrDefault();
                    if (s != null)
                    {
                        s.AddCategory(category);
                    }
                    else
                    {
                        s = student;
                        s.AddCategory(category);
                        students.Add(s);
                    }
                    return s;
                },
                splitOn: "Id",
                param: new { StudentName = "Camila" }
                );

        Console.WriteLine($"\nCamila's courses:");
        foreach (var i in students.First().Categories)
        {
            Console.WriteLine($"{i.Id} => {i.Title}");
        }
    }
    public static async Task SelectMultiple(SqliteConnection connection)
    {
        using var multi = await connection.QueryMultipleAsync(@"
            SELECT * FROM Category;
            SELECT * From Student;
        ");

        var categories = await multi.ReadAsync<Category>();
        var students = await multi.ReadAsync<Student>();

        Console.WriteLine("\nQuery Multiple:");

        foreach (var c in categories)
        {
            Console.WriteLine($"{c.Id} => {c.Title}");
        }

        foreach (var s in students)
        {
            Console.WriteLine($"{s.Id} => {s.Name}");
        }

    }
    public static async Task SelectIn(SqliteConnection connection)
    {
        var query = "SELECT * FROM Student WHERE Name IN @StudentNames;";
        var students = await connection.QueryAsync<Student>(query, new
        {
            StudentNames = new[] { "Diego", "Camila" }
        });

        Console.WriteLine("\nSelect IN:");
        foreach (var s in students)
        {
            Console.WriteLine($"Student({s.Id},{s.Name})");
        }
    }
    public static async Task SelectLike(SqliteConnection connection, string startsWith)
    {
        var query = "SELECT * FROM Category WHERE Title LIKE @Expression;";
        var categories = await connection.QueryAsync<Category>(query, new
        {
            Expression = $"{startsWith}%"
        }); ;

        Console.WriteLine("\nSelect LIKE:");
        foreach (var c in categories)
        {
            Console.WriteLine($"Category({c.Id},{c.Title})");
        }
    }
    public static async Task Transaction()
    {
        var _connection = new SqliteConnection(_connectionString);
        
        // It's necessary open and close connection explicitly.
        await _connection.OpenAsync();
       
        using var transaction = await _connection.BeginTransactionAsync();

        Console.WriteLine("\nTransaction:");

        var query = "DELETE FROM CategoryStudent;";

        Console.WriteLine($"Running DELETE without WHERE => {query}");
        var affectedRows = await _connection.ExecuteAsync(query, transaction);

        Console.WriteLine($"{affectedRows} affected row(s)");

        Console.WriteLine("Applying ROLLBACK");
        await transaction.RollbackAsync();

        // Close connection.
        await _connection.CloseAsync();

    }
}

