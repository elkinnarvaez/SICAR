using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SICAR.Models;

namespace SICAR.Data
{
    public class SICARDatabase
    {
        readonly SQLiteAsyncConnection database;

        public SICARDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<User>().Wait();
            database.CreateTableAsync<Crop>().Wait();
            database.CreateTableAsync<Session>().Wait();
        }

        // User methods
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

        // Crops methods
        public Task<List<Crop>> GetAllCropsAsync()
        {
            // Get all crops
            return database.Table<Crop>().ToListAsync();
        }
        public Task<List<Crop>> GetCropsOfUserAsync(string userId)
        {
            // Get crops of user
            return database.Table<Crop>()
                            .Where(i => i.user == userId).ToListAsync();
        }

        public Task<Crop> GetCropAsync(int id)
        {
            // Get a specific crop.
            return database.Table<Crop>()
                            .Where(i => i.id == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveCropAsync(Crop crop)
        {
            if (crop.id != 0)
            {
                // Update an existing crop.
                return database.UpdateAsync(crop);
            }
            else
            {
                // Save a new crop.
                return database.InsertAsync(crop);
            }
        }

        public Task<int> DeleteCropAsync(Crop crop)
        {
            // Delete a crop.
            return database.DeleteAsync(crop);
        }

        // Session methods
        //public Task<Session> GetCurrentSessionAsync()
        //{
        //    //Get current session.
        //    return database.Table<Session>().FirstOrDefaultAsync();
        //}

        //public Task<int> SaveSessionAsync(Session session)
        //{
        //    if (session.id != 0)
        //    {
        //        // Update an existing session.
        //        return database.UpdateAsync(session);
        //    }
        //    else
        //    {
        //        // Save a new session.
        //        return database.InsertAsync(session);
        //    }
        //}

        //public Task<int> DeleteSessionAsync(Session session)
        //{
        //    // Delete a session.
        //    return database.DeleteAsync(session);
        //}
    }
}
