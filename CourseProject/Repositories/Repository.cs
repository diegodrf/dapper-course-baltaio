using CourseProject.Exceptions;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;

namespace CourseProject.Repositories
{
    public class Repository<T> where T : class
    {
        private readonly SqlConnection _connection;

        public Repository(SqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<T>> GetAsync()
        {
            try
            {
                return await _connection.GetAllAsync<T>();
            }
            catch (SqlException e)
            {
                throw new DatabaseException(e.Message);
            }
        }
        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                return await _connection.GetAsync<T>(id) ?? throw new NotFoundException($"{nameof(T)} Id {id} not found.");
            }
            catch (SqlException e)
            {
                throw new DatabaseException(e.Message);
            }
        }

        public async Task<int> CreateAsync(T model)
        {
            try
            {
                return await _connection.InsertAsync<T>(model);
            }
            catch (SqlException e)
            {
                throw new DatabaseException(e.Message);
            }
        }
        public async Task<bool> UpdateAsync(T model)
        {
            try
            {
                return await _connection.UpdateAsync<T>(model);
            }
            catch (SqlException e)
            {
                throw new DatabaseException(e.Message);
            }
        }
        public async Task<bool> DeleteAsync(T model)
        {
            try
            {
                return await _connection.DeleteAsync<T>(model);
            }
            catch (SqlException e)
            {
                throw new DatabaseException(e.Message);
            }
        }
        public async Task<bool> DeleteByIdAsync(int id)
        {
            try
            {
                if (id == 0)
                {
                    return false;
                }
                var model = await GetByIdAsync(id);
                return await _connection.DeleteAsync<T>(model);
            }
            catch (SqlException e)
            {
                throw new DatabaseException(e.Message);
            }
            catch (NotFoundException)
            {
                throw;
            }
        }
    }
}
