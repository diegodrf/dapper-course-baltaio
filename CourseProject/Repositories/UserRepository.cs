using CourseProject.Exceptions;
using CourseProject.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.Repositories
{
    public class UserRepository : Repository<User>
    {
        private readonly SqlConnection _connection;

        public UserRepository(SqlConnection connection) : base(connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<User>> GetWithRoles()
        {
            var users = new List<User>();
            var query = @"SELECT 
                [User].*, 
                [Role].* 
                FROM [User]
                LEFT JOIN [UserRole] ON [UserRole].[UserId] = [User].[Id]
                LEFT JOIN [Role] ON [UserRole].[RoleId] = [Role].[Id];";

            User mapUserRole(User user, Role role)
            {
                var _user = users
                    .Where(u => u.Id == user.Id)
                    .FirstOrDefault();

                if (_user is null)
                {
                    _user = user;
                    users.Add(_user);
                }

                _user.Roles.Add(role);
                return _user;
            }

            var _ = await _connection.QueryAsync<User, Role, User>(query, mapUserRole, splitOn: "Id");

            return users;
        }
    }
}
