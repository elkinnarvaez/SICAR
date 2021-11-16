using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SICAR.Models;
using System.Linq;

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
            database.CreateTableAsync<WeatherStationData>().Wait();
            database.CreateTableAsync<Sync>().Wait();
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
                            .Where(i => i.Id == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveUserAsync(User user)
        {
            if (user.Id != 0)
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

        public Task<List<Crop>> GetCropsOfUserAsync(string username)
        {
            // Get crops of user
            return database.Table<Crop>()
                            .Where(i => i.Username == username).ToListAsync();
        }

        public Task<Crop> GetCropAsync(int id)
        {
            // Get a specific crop.
            return database.Table<Crop>()
                            .Where(i => i.Id == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveCropAsync(Crop crop)
        {
            if (crop.Id != 0)
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
        public Task<Session> GetCurrentSessionAsync()
        {
            //Get current session.
            return database.Table<Session>().FirstOrDefaultAsync();
        }

        public Task<int> SaveSessionAsync(Session session)
        {
            if (session.Id != 0)
            {
                // Update an existing session.
                return database.UpdateAsync(session);
            }
            else
            {
                // Save a new session.
                return database.InsertAsync(session);
            }
        }

        public Task<int> DeleteSessionAsync(Session session)
        {
            // Delete a session.
            return database.DeleteAsync(session);
        }

        // WeatherStationData methods
        public Task<List<WeatherStationData>> GetAllWeatherStationData()
        {
            // Get all weather station data
            return database.Table<WeatherStationData>().ToListAsync();
        }

        public Task<List<WeatherStationData>> GetAllWeatherStationDataFromSpecificDate(string date)
        {
            return database.Table<WeatherStationData>()
                            .Where(i => i.Date == date).ToListAsync();
        }

        public Task<int> DeleteWeatherStationDataAsync(WeatherStationData weatherStationData)
        {
            // Delete a session.
            return database.DeleteAsync(weatherStationData);
        }

        public Task<int> SaveWeatherStationDataAsync(WeatherStationData weatherStationData)
        {
            if (weatherStationData.Id != 0)
            {
                // Update an existing weatherStationData.
                return database.UpdateAsync(weatherStationData);
            }
            else
            {
                // Save a new weatherStationData.
                return database.InsertAsync(weatherStationData);
            }
        }

        // Sync methods
        public Task<Sync> GetSyncStatusAsync()
        {
            //Get current sync.
            return database.Table<Sync>().FirstOrDefaultAsync();
        }

        public Task<int> SaveSyncAsync(Sync sync)
        {
            if (sync.Id != 0)
            {
                // Update an existing sync.
                return database.UpdateAsync(sync);
            }
            else
            {
                // Save a new sync.
                return database.InsertAsync(sync);
            }
        }

        public Task<int> DeleteSyncAsync(Sync sync)
        {
            // Delete a sync.
            return database.DeleteAsync(sync);
        }
    }
}
