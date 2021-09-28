using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;
using SQLite;
using SICAR.Models;

namespace SICAR.Data
{
    class UserDatabase
    {
        readonly SQLiteAsyncConnection database;

        public UserDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<User>().Wait();
        }

        public Task<List<User>> GetUsersAsync()
        {
            //Get all users.
            return database.Table<User>().ToListAsync();
        }

        public Task<User> GetUserAsync(int id)
        {
            // Get a specific user.
            return database.Table<User>()
                            .Where(i => i.id == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveUserAsync(User user)
        {
            if (user.id != 0)
            {
                // Update an existing user.
                return database.UpdateAsync(user);
            }
            else
            {
                // Save a new user.
                return database.InsertAsync(user);
            }
        }

        public Task<int> DeleteUserAsync(User user)
        {
            // Delete an user.
            return database.DeleteAsync(user);
        }
    }
}
