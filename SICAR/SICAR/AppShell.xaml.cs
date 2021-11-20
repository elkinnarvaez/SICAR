using SICAR.ViewModels;
using SICAR.Views;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Xamarin.Forms;
using SICAR.Models;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Essentials;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Linq;
using System.Text;

using System.Net.Http;
using Newtonsoft.Json;

namespace SICAR
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        private string names;
        private string lastnames;
        private string username;
        private bool activeInternetConnection;
        private bool unactiveInternetConnection;
        private bool isSynced;
        private bool isSyncing;
        private bool isChecking;

        // private const string pathToServiceAccountKeyFile = "sicarapp-credentials.json";
        private string pathToServiceAccountKeyFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sicarapp-credentials.json");
        private const string serviceAccountEmail = "sicarapp-service-account@sicarapp.iam.gserviceaccount.com";
        // private const string pathToUploadFile = "hello.txt";
        private string pathToUploadFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "upload.txt");
        private string pathToDownloadedFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "download.txt");
        private const string directoryId = "1iC6xLmqTc1ZhQNg97UWrn2wDKqCNn5PN";
        
        private bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public string Names
        {
            get => names;
            set => SetProperty(ref names, value);
        }

        public string Lastnames
        {
            get => lastnames;
            set => SetProperty(ref lastnames, value);
        }

        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        public bool ActiveInternetConnection
        {
            get => activeInternetConnection;
            set => SetProperty(ref activeInternetConnection, value);
        }

        public bool UnactiveInternetConnection
        {
            get => unactiveInternetConnection;
            set => SetProperty(ref unactiveInternetConnection, value);
        }

        public bool IsSynced
        {
            get => isSynced;
            set => SetProperty(ref isSynced, value);
        }

        public bool IsSyncing
        {
            get => isSyncing;
            set => SetProperty(ref isSyncing, value);
        }

        public bool IsChecking
        {
            get => isChecking;
            set => SetProperty(ref isChecking, value);
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            Session session = await App.Database.GetCurrentSessionAsync();
            await App.Database.DeleteSessionAsync(session);
            await Shell.Current.GoToAsync("//LoginPage");
        }

        public async void GetUserInSession()
        {
            Session session = await App.Database.GetCurrentSessionAsync();
            if (session != null)
            {
                Names = session.Names;
                Lastnames = session.Lastnames;
                Username = session.Username;
            }
        }

        public void CheckInternetConnection()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                ActiveInternetConnection = true;
                UnactiveInternetConnection = false;
            }
            else
            {
                ActiveInternetConnection = false;
                UnactiveInternetConnection = true;
            }
        }

        public async void GetSyncStatus()
        {
            Sync sync = await App.Database.GetSyncStatusAsync();
            if(sync != null)
            {
                IsSynced = sync.isSynced;
            }
            else
            {
                Sync newSync = new Sync()
                {
                    isSynced = false
                };
                await App.Database.SaveSyncAsync(newSync);
                IsSynced = false;
            }
        }

        public async Task<bool> CheckSyncOnGoogleDrive(DriveService service)
        {
            if (activeInternetConnection == true && isSyncing == false)
            {
                IsChecking = true;
                Console.WriteLine("--------------------------------------------------------------------------------------");
                Console.WriteLine("Checking if everything is up date...");
                Console.WriteLine("--------------------------------------------------------------------------------------");
                List<String> linesUserFileGoogleDrive = GetLinesFileContentOnGoogleDrive(service, "User.txt");
                List<String> linesCropFileGoogleDrive = GetLinesFileContentOnGoogleDrive(service, "Crop.txt");
                List<String> linesAllWeatherStationDataFileGoogleDrive = GetLinesFileContentOnGoogleDrive(service, "WeatherStationData.txt");
                List<SICAR.Models.User> users = await App.Database.GetUsersAsync();
                List<SICAR.Models.Crop> crops = await App.Database.GetAllCropsAsync();
                List<SICAR.Models.WeatherStationData> allWeatherStationData = await App.Database.GetAllWeatherStationData();

                // Convert lines from Google Drive files to list objects
                List<SICAR.Models.User> usersGoogleDrive = new List<SICAR.Models.User>();
                List<SICAR.Models.Crop> cropsGoogleDrive = new List<SICAR.Models.Crop>();
                List<SICAR.Models.WeatherStationData> allWeatherStationDataGoogleDrive = new List<SICAR.Models.WeatherStationData>();

                foreach (string line in linesUserFileGoogleDrive)
                {
                    string[] subs = line.Split(',');
                    string username = subs[1];
                    string password = subs[2];
                    string names = subs[3];
                    string lastnames = subs[4];
                    SICAR.Models.User user = new SICAR.Models.User()
                    {
                        Username = username,
                        Password = password,
                        Names = names,
                        Lastnames = lastnames
                    };
                    usersGoogleDrive.Add(user);
                }

                foreach (string line in linesCropFileGoogleDrive)
                {
                    string[] subs = line.Split(',');
                    string username = subs[1];
                    string name = subs[2];
                    string type = subs[3];
                    string date = subs[4];
                    string hectare = subs[5];
                    string ground = subs[6];
                    string deep = subs[7];
                    SICAR.Models.Crop crop = new SICAR.Models.Crop()
                    {
                        Username = username,
                        Name = name,
                        Type = type,
                        Date = date,
                        Hectare = Int32.Parse(hectare),
                        Ground = ground,
                        Deep = Int32.Parse(deep)
                    };
                    cropsGoogleDrive.Add(crop);
                }

                foreach (string line in linesAllWeatherStationDataFileGoogleDrive)
                {
                    string[] subs = line.Split(',');
                    string date = subs[1];
                    string time = subs[2];
                    string aveargeTemp = subs[3];
                    string minTemp = subs[4];
                    string maxTemp = subs[5];
                    string averageHumidity = subs[6];
                    string minHumidity = subs[7];
                    string maxHumidity = subs[8];
                    string atmosphericPressure = subs[9];
                    string windSpeed = subs[10];
                    string solarRadiation = subs[11];
                    string precipitation = subs[12];
                    string evapotranspiration = subs[13];
                    SICAR.Models.WeatherStationData weatherStationData = new SICAR.Models.WeatherStationData()
                    {
                        Date = date,
                        Time = time,
                        AveargeTemp = float.Parse(aveargeTemp),
                        MinTemp = float.Parse(minTemp),
                        MaxTemp = float.Parse(maxTemp),
                        AverageHumidity = float.Parse(averageHumidity),
                        MinHumidity = float.Parse(minHumidity),
                        MaxHumidity = float.Parse(maxHumidity),
                        AtmosphericPressure = float.Parse(atmosphericPressure),
                        WindSpeed = float.Parse(windSpeed),
                        SolarRadiation = float.Parse(solarRadiation),
                        Precipitation = float.Parse(precipitation),
                        Evapotranspiration = float.Parse(evapotranspiration)
                    };
                    allWeatherStationDataGoogleDrive.Add(weatherStationData);
                }

                String usersString = "";
                String usersGoogleDriveString = "";
                foreach (SICAR.Models.User user in users)
                {
                    usersString = usersString + user.Id.ToString() + "," + user.Username + "," + user.Password + "," + user.Names + "," + user.Lastnames + Environment.NewLine;
                }
                foreach (SICAR.Models.User user in usersGoogleDrive)
                {
                    usersGoogleDriveString = usersGoogleDriveString + user.Id.ToString() + "," + user.Username + "," + user.Password + "," + user.Names + "," + user.Lastnames + Environment.NewLine;
                }

                String cropsString = "";
                String cropsGoogleDriveString = "";
                foreach (SICAR.Models.Crop crop in crops)
                {
                    cropsString = cropsString + crop.Id.ToString() + "," + crop.Username + "," + crop.Name + "," + crop.Type + "," + crop.Date + "," + crop.Hectare.ToString() + "," + crop.Ground + "," + crop.Deep.ToString() + Environment.NewLine;
                }
                foreach (SICAR.Models.Crop crop in cropsGoogleDrive)
                {
                    cropsGoogleDriveString = cropsGoogleDriveString + crop.Id.ToString() + "," + crop.Username + "," + crop.Name + "," + crop.Type + "," + crop.Date + "," + crop.Hectare.ToString() + "," + crop.Ground + "," + crop.Deep.ToString() + Environment.NewLine;
                }

                String allWeatherStationDataString = "";
                String allWeatherStationDataGoogleDriveString = "";
                foreach (SICAR.Models.WeatherStationData weatherStationData in allWeatherStationData)
                {
                    allWeatherStationDataString = allWeatherStationDataString + weatherStationData.Id.ToString() + "," + weatherStationData.Date + "," + weatherStationData.Time + "," + weatherStationData.AveargeTemp.ToString() + "," + weatherStationData.MinTemp.ToString() + "," + weatherStationData.MaxTemp.ToString() + "," + weatherStationData.AverageHumidity.ToString() + "," + weatherStationData.MinHumidity.ToString() + "," + weatherStationData.MaxHumidity.ToString() + "," + weatherStationData.AtmosphericPressure.ToString() + "," + weatherStationData.WindSpeed.ToString() + "," + weatherStationData.SolarRadiation.ToString() + "," + weatherStationData.Precipitation.ToString() + "," + weatherStationData.Evapotranspiration.ToString() + Environment.NewLine;
                }
                foreach (SICAR.Models.WeatherStationData weatherStationData in allWeatherStationDataGoogleDrive)
                {
                    allWeatherStationDataGoogleDriveString = allWeatherStationDataGoogleDriveString + weatherStationData.Id.ToString() + "," + weatherStationData.Date + "," + weatherStationData.Time + "," + weatherStationData.AveargeTemp.ToString() + "," + weatherStationData.MinTemp.ToString() + "," + weatherStationData.MaxTemp.ToString() + "," + weatherStationData.AverageHumidity.ToString() + "," + weatherStationData.MinHumidity.ToString() + "," + weatherStationData.MaxHumidity.ToString() + "," + weatherStationData.AtmosphericPressure.ToString() + "," + weatherStationData.WindSpeed.ToString() + "," + weatherStationData.SolarRadiation.ToString() + "," + weatherStationData.Precipitation.ToString() + "," + weatherStationData.Evapotranspiration.ToString() + Environment.NewLine;
                }

                if(usersString == usersGoogleDriveString && cropsString == cropsGoogleDriveString && allWeatherStationDataString == allWeatherStationDataGoogleDriveString)
                {
                    Sync sync = await App.Database.GetSyncStatusAsync();
                    if (sync != null)
                    {
                        sync.isSynced = false;
                        await App.Database.SaveSyncAsync(sync);
                    }
                    else
                    {
                        Sync newSync = new Sync()
                        {
                            isSynced = false
                        };
                        await App.Database.SaveSyncAsync(newSync);
                    }
                }
                else
                {
                    Sync sync = await App.Database.GetSyncStatusAsync();
                    if (sync != null)
                    {
                        sync.isSynced = false;
                        await App.Database.SaveSyncAsync(sync);
                    }
                    else
                    {
                        Sync newSync = new Sync()
                        {
                            isSynced = false
                        };
                        await App.Database.SaveSyncAsync(newSync);
                    }
                }
                Console.WriteLine("--------------------------------------------------------------------------------------");
                Console.WriteLine("Finished checking.");
                Console.WriteLine("--------------------------------------------------------------------------------------");
                IsChecking = false;
            }
            else
            {
                if(activeInternetConnection == false)
                {
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                    Console.WriteLine("Please verify your internet because you might be working with outdated data.");
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                }
                else
                {
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                    Console.WriteLine("Can't check right now.");
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                }
            }
            return true;
        }


        public async void SyncingProcessWithGoogleDrive(DriveService service)
        {
            if(isSynced == false)
            {
                if(activeInternetConnection == true && isChecking == false)
                {
                    IsSyncing = true;
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                    Console.WriteLine("Syncing in process...");
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                    List<String> linesUserFileGoogleDrive = GetLinesFileContentOnGoogleDrive(service, "User.txt");
                    List<String> linesCropFileGoogleDrive = GetLinesFileContentOnGoogleDrive(service, "Crop.txt");
                    List<String> linesAllWeatherStationDataFileGoogleDrive = GetLinesFileContentOnGoogleDrive(service, "WeatherStationData.txt");
                    List<SICAR.Models.User> users = await App.Database.GetUsersAsync();
                    List<SICAR.Models.Crop> crops = await App.Database.GetAllCropsAsync();
                    List<SICAR.Models.WeatherStationData> allWeatherStationData = await App.Database.GetAllWeatherStationData();

                    // Convert lines from Google Drive files to list objects
                    List<SICAR.Models.User> usersGoogleDrive = new List<SICAR.Models.User>();
                    List<SICAR.Models.Crop> cropsGoogleDrive = new List<SICAR.Models.Crop>();
                    List<SICAR.Models.WeatherStationData> allWeatherStationDataGoogleDrive = new List<SICAR.Models.WeatherStationData>();

                    foreach (string line in linesUserFileGoogleDrive)
                    {
                        string[] subs = line.Split(',');
                        string username = subs[1];
                        string password = subs[2];
                        string names = subs[3];
                        string lastnames = subs[4];
                        SICAR.Models.User user = new SICAR.Models.User()
                        {
                            Username = username,
                            Password = password,
                            Names = names,
                            Lastnames = lastnames
                        };
                        usersGoogleDrive.Add(user);
                    }

                    foreach (string line in linesCropFileGoogleDrive)
                    {
                        string[] subs = line.Split(',');
                        string username = subs[1];
                        string name = subs[2];
                        string type = subs[3];
                        string date = subs[4];
                        string hectare = subs[5];
                        string ground = subs[6];
                        string deep = subs[7];
                        SICAR.Models.Crop crop = new SICAR.Models.Crop()
                        {
                            Username = username,
                            Name = name,
                            Type = type,
                            Date = date,
                            Hectare = Int32.Parse(hectare),
                            Ground = ground,
                            Deep = Int32.Parse(deep)
                        };
                        cropsGoogleDrive.Add(crop);
                    }

                    foreach (string line in linesAllWeatherStationDataFileGoogleDrive)
                    {
                        string[] subs = line.Split(',');
                        string date = subs[1];
                        string time = subs[2];
                        string aveargeTemp = subs[3];
                        string minTemp = subs[4];
                        string maxTemp = subs[5];
                        string averageHumidity = subs[6];
                        string minHumidity = subs[7];
                        string maxHumidity = subs[8];
                        string atmosphericPressure = subs[9];
                        string windSpeed = subs[10];
                        string solarRadiation = subs[11];
                        string precipitation = subs[12];
                        string evapotranspiration = subs[13];
                        SICAR.Models.WeatherStationData weatherStationData = new SICAR.Models.WeatherStationData()
                        {
                            Date = date,
                            Time = time,
                            AveargeTemp = float.Parse(aveargeTemp),
                            MinTemp = float.Parse(minTemp),
                            MaxTemp = float.Parse(maxTemp),
                            AverageHumidity = float.Parse(averageHumidity),
                            MinHumidity = float.Parse(minHumidity),
                            MaxHumidity = float.Parse(maxHumidity),
                            AtmosphericPressure = float.Parse(atmosphericPressure),
                            WindSpeed = float.Parse(windSpeed),
                            SolarRadiation = float.Parse(solarRadiation),
                            Precipitation = float.Parse(precipitation),
                            Evapotranspiration = float.Parse(evapotranspiration)
                        };
                        allWeatherStationDataGoogleDrive.Add(weatherStationData);
                    }

                    // Delete all data from local database
                    //foreach (SICAR.Models.User user in users)
                    //{
                    //    App.Database.DeleteUserAsync(user).Wait();
                    //}
                    //foreach (SICAR.Models.Crop crop in crops)
                    //{
                    //    App.Database.DeleteCropAsync(crop).Wait();
                    //}
                    //foreach (SICAR.Models.WeatherStationData weatherStationData in allWeatherStationData)
                    //{
                    //    App.Database.DeleteWeatherStationDataAsync(weatherStationData).Wait();
                    //}

                    // Here goes the syncing part
                    List<SICAR.Models.User> usersSynced = users;
                    List<SICAR.Models.Crop> cropsSynced = crops;
                    List<SICAR.Models.WeatherStationData> allWeatherStationDataSynced = allWeatherStationData;

                    foreach (SICAR.Models.User userInGoogleDrive in usersGoogleDrive)
                    {
                        bool found = false;
                        foreach (SICAR.Models.User user in usersSynced)
                        {
                            if (user.Username == userInGoogleDrive.Username)
                            {
                                found = true;
                            }
                        }
                        if (found == false)
                        {
                            usersSynced.Add(userInGoogleDrive);
                        }
                    }

                    foreach (SICAR.Models.Crop cropInGoogleDrive in cropsGoogleDrive)
                    {
                        bool found = false;
                        foreach (SICAR.Models.Crop crop in cropsSynced)
                        {
                            if (crop.Username == cropInGoogleDrive.Username && crop.Name == cropInGoogleDrive.Name)
                            {
                                found = true;
                            }
                        }
                        if (found == false)
                        {
                            cropsSynced.Add(cropInGoogleDrive);
                        }
                    }

                    foreach (SICAR.Models.WeatherStationData weatherStationDataInGoogleDrive in allWeatherStationDataGoogleDrive)
                    {
                        bool found = false;
                        foreach (SICAR.Models.WeatherStationData weatherStationData in allWeatherStationDataSynced)
                        {
                            if (weatherStationData.Date == weatherStationDataInGoogleDrive.Date && weatherStationData.Time == weatherStationDataInGoogleDrive.Time)
                            {
                                found = true;
                            }
                        }
                        if (found == false)
                        {
                            allWeatherStationDataSynced.Add(weatherStationDataInGoogleDrive);
                        }
                    }

                    // Feeding tables in database and updating files on Google Drive
                    List<String> linesUsersSynced = new List<String>();
                    List<String> linesCropsSynced = new List<String>();
                    List<String> linesAllWeatherStationDataSynced = new List<String>();
                    foreach (SICAR.Models.User user in usersSynced)
                    {
                        if (user.Id == 0)
                        {
                            await App.Database.SaveUserAsync(user);
                        }
                        linesUsersSynced.Add(user.Id.ToString() + "," + user.Username + "," + user.Password + "," + user.Names + "," + user.Lastnames);
                    }
                    foreach (SICAR.Models.Crop crop in cropsSynced)
                    {
                        if (crop.Id == 0)
                        {
                            await App.Database.SaveCropAsync(crop);
                        }
                        linesCropsSynced.Add(crop.Id.ToString() + "," + crop.Username + "," + crop.Name + "," + crop.Type + "," + crop.Date + "," + crop.Hectare.ToString() + "," + crop.Ground + "," + crop.Deep.ToString());
                    }
                    foreach (SICAR.Models.WeatherStationData weatherStationData in allWeatherStationDataSynced)
                    {
                        if (weatherStationData.Id == 0)
                        {
                            await App.Database.SaveWeatherStationDataAsync(weatherStationData);
                        }
                        linesAllWeatherStationDataSynced.Add(weatherStationData.Id.ToString() + "," + weatherStationData.Date + "," + weatherStationData.Time + "," + weatherStationData.AveargeTemp.ToString() + "," + weatherStationData.MinTemp.ToString() + "," + weatherStationData.MaxTemp.ToString() + "," + weatherStationData.AverageHumidity.ToString() + "," + weatherStationData.MinHumidity.ToString() + "," + weatherStationData.MaxHumidity.ToString() + "," + weatherStationData.AtmosphericPressure.ToString() + "," + weatherStationData.WindSpeed.ToString() + "," + weatherStationData.SolarRadiation.ToString() + "," + weatherStationData.Precipitation.ToString() + "," + weatherStationData.Evapotranspiration.ToString());
                    }
                    UpdateFileOnGoogleDrive(service, "User.txt", linesUsersSynced);
                    UpdateFileOnGoogleDrive(service, "Crop.txt", linesCropsSynced);
                    UpdateFileOnGoogleDrive(service, "WeatherStationData.txt", linesAllWeatherStationDataSynced);
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                    Console.WriteLine("Eveything is up to date now.");
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                    Sync sync = await App.Database.GetSyncStatusAsync();
                    if (sync != null)
                    {
                        sync.isSynced = true;
                        await App.Database.SaveSyncAsync(sync);
                    }
                    else
                    {
                        Sync newSync = new Sync()
                        {
                            isSynced = true
                        };
                        await App.Database.SaveSyncAsync(newSync);
                    }
                    IsSyncing = false;
                }
                else
                {
                    if(activeInternetConnection == false)
                    {
                        Console.WriteLine("--------------------------------------------------------------------------------------");
                        Console.WriteLine("Please verify your internet connection in order to sync the changes.");
                        Console.WriteLine("--------------------------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine("--------------------------------------------------------------------------------------");
                        Console.WriteLine("Can't sync right now.");
                        Console.WriteLine("--------------------------------------------------------------------------------------");
                    }
                }
            }
            else
            {
                Console.WriteLine("--------------------------------------------------------------------------------------");
                Console.WriteLine("Everything is already up to date.");
                Console.WriteLine("--------------------------------------------------------------------------------------");
            }
        }

        /* Google Drive public methods */

        public void ListFilesOnGoogleDrive(DriveService service)
        {
            System.IO.File.WriteAllText(pathToUploadFile, @"");
            System.IO.File.WriteAllText(pathToDownloadedFile, @"");

            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            Console.WriteLine("Files:");
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Console.WriteLine("{0} ({1})", file.Name, file.Id);
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
            Console.Read();
        }

        public void DeleteAllFilesOnGoogleDrive(DriveService service)
        {
            System.IO.File.WriteAllText(pathToUploadFile, @"");
            System.IO.File.WriteAllText(pathToDownloadedFile, @"");

            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();

            // Delete files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            Console.WriteLine("Files deleted:");
            if (files != null && files.Count > 1)
            {
                foreach (var file in files)
                {
                    if (file.Id != directoryId)
                    {
                        service.Files.Delete(file.Id).Execute();
                        Console.WriteLine("{0} ({1}) -- deleted", file.Name, file.Id);
                    }
                }
            }
            else
            {
                Console.WriteLine("No files deleted.");
            }
            Console.Read();
        }

        public void DeleteFileOnGoogleDrive(DriveService service, string fileId)
        {
            System.IO.File.WriteAllText(pathToUploadFile, @"");
            System.IO.File.WriteAllText(pathToDownloadedFile, @"");

            if (fileId != directoryId)
            {
                var file = service.Files.Get(fileId).Execute();
                service.Files.Delete(file.Id).Execute();
                Console.WriteLine("{0} ({1}) -- deleted", file.Name, file.Id);
            }
            else
            {
                Console.WriteLine("You can't delete this shared folder");
            }
        }

        public List<string> GetLinesFileContentOnGoogleDrive(DriveService service, string fileName)
        {
            System.IO.File.WriteAllText(pathToUploadFile, @"");
            System.IO.File.WriteAllText(pathToDownloadedFile, @"");

            // Get fileId by file name (it is assumed that the files names are unique)
            string fileId = "";
            FilesResource.ListRequest listRequest = service.Files.List();
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            foreach (var f in files)
            {
                if (f.Name == fileName)
                {
                    fileId = f.Id;
                }
            }

            // Download file content to storage
            var fileContent = new FileStream(pathToDownloadedFile, FileMode.Open, FileAccess.Write);
            service.Files.Get(fileId).Download(fileContent);
            fileContent.Close();

            // Read content from file
            List<String> linesFileContent = new List<String>();
            using (StreamReader sr = System.IO.File.OpenText(pathToDownloadedFile))
            {
                String line = "";

                while ((line = sr.ReadLine()) != null)
                {
                    //Console.WriteLine(line);
                    linesFileContent.Add(line);
                }
            }
            return linesFileContent;
        }

        public void UpdateFileOnGoogleDrive(DriveService service, string fileName, List<string> linesNewFileContent)
        {
            System.IO.File.WriteAllText(pathToUploadFile, @"");
            System.IO.File.WriteAllText(pathToDownloadedFile, @"");

            // Get fileId by file name (it is assumed that the files names are unique)
            string fileId = "";
            FilesResource.ListRequest listRequest = service.Files.List();
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            foreach (var f in files)
            {
                if (f.Name == fileName)
                {
                    fileId = f.Id;
                }
            }

            // Create modified content
            string newFileContent = "";
            foreach(string line in linesNewFileContent)
            {
                newFileContent = newFileContent + line + Environment.NewLine;
            }
            System.IO.File.WriteAllText(pathToUploadFile, newFileContent);

            //Upload file content
            var file = service.Files.Get(fileId).Execute();
            var newFileMetadata = new Google.Apis.Drive.v3.Data.File();
            using (var fsSource = new FileStream(pathToUploadFile, FileMode.Open, FileAccess.Read))
            {
                var request = service.Files.Update(newFileMetadata, file.Id, fsSource, "text/plain");
                var results = request.Upload();
                if (results.Status == Google.Apis.Upload.UploadStatus.Failed)
                {
                    Console.WriteLine($"Error updating file: {results.Exception.Message}");
                }
            }
        }

        public void UploadFileToGoogleDrive(DriveService service, string fileContent, string fileName)
        {
            System.IO.File.WriteAllText(pathToUploadFile, @"");
            System.IO.File.WriteAllText(pathToDownloadedFile, @"");

            // Create file content
            System.IO.File.WriteAllText(pathToUploadFile, fileContent);

            // Upload file Metadata
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Parents = new List<string>() {directoryId}
            };

            // Create a new file on Google Drive
            string uploadedFileId;
            using (var fsSource = new FileStream(pathToUploadFile, FileMode.Open, FileAccess.Read))
            {
                // Create a new file, with metadata and stream
                var request = service.Files.Create(fileMetadata, fsSource, "text/plain");
                
                request.Fields = "*";
                var results = request.Upload();
                if (results.Status == Google.Apis.Upload.UploadStatus.Failed)
                {
                    Console.WriteLine($"Error uploading file: {results.Exception.Message}");
                }
                uploadedFileId = request.ResponseBody?.Id;
            }
        }

        public AppShell()
        {
            InitializeComponent();
            this.BindingContext = this;
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(CropDetailPage), typeof(CropDetailPage));
            Routing.RegisterRoute(nameof(NewCropPage), typeof(NewCropPage));
            Routing.RegisterRoute(nameof(UserProfilePage), typeof(UserProfilePage));

            /* ----------- Google Drive Initialization ----------- */

            // Copy files to storage
            System.IO.File.WriteAllText(pathToServiceAccountKeyFile, @"{
                                                                          'type': 'service_account',
                                                                          'project_id': 'sicarapp',
                                                                          'private_key_id': '7424ca320cd49fd02fce604adf0b8b3819632859',
                                                                          'private_key': '-----BEGIN PRIVATE KEY-----\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDo2o9rlvd2y5E3\nFJ2C45qtKdVjfer1ywNmpLEDb4n6albkPEmreUvigbjwlzn2hdnsrfj8QYv9ePZQ\nq9Rdf4u/rVoVvapLJMbQOgcX55yvZm+NJsxdH7RgamipaaH0a3/THEd8Iv9Ym8H3\nEoCB0S5/MRxEQRHzgEdZaNPsFXsMiT5FKrFU16R/xrWeB/4EqVzupa4OE13EWnQC\n+JqN9mUnCX5chwlju2bt9BVfeTPePCO82pVMsrH/Bnx8frFmX8gr/io6VhRaR8TF\nfGdyHsQcpa5EDAmU7P6Dh3hITbUvbVbl6agUaV+WwHOMaljCf8w8/mSZC/MRohsZ\nbHvIhbRDAgMBAAECggEAZPHS+QZiU0qesm0qd4KqHGWlT1NYF6Qh3k6JOn8RJtEQ\nyDLmkQKthg1MmmhExX1zYupspRbZVFIrHf/PuZTAhaDmC8TEXX0c/0oYpyFOSK+0\nrlFiiQBkluOqab8Uabxslp2M0+DsJ4KmjLClqyF+8b7djS1UVeaHh9gs/wy04lKN\nTcbGd19ggsafzIREbXV+ENDUwsiQZWkrE54rULpgmtxld+LMtdrnUaxqx/5udyLF\nKURfQBnRVqJgvaU2/0oxJvJogDRj0eynyQbmM54UbQiuAbWSP8DtTS9RCe99Qrxb\nkZo6ZbzCl5PfuZ+UkheYIoghEHrIoMNMppjzTY+YUQKBgQD1sfgBMl9UM0ZBesV0\nkmaelaSvP8h/wrrGcLjZJFBu5LbsXK3Jg44jnu9iHUdIrr/AQ+5dEaE9sSzus4EX\nzvMJ5COb/8MBTn1+7Tz+PSiYwWQKawdm78O8eCG2wpru+E8z/ydIPw9NPKJqt6zv\nBxYzc44wY3FBsFNkwqWiXRAtWQKBgQDynrZ72pW5rSXv41H0XAhE4dALakWcx2Rm\n5ORaR0HzSt1Um5E7QaE+6AL5oKtOJxqaWLbM2ONcdT8naBK0vGJwsG9FexLFttxp\n7Q5WUzUnLK4ARl/eM8d+FxHUSf+We/GvMA2jzPYoxHAp0HdTrVYhw8lJTDaVjzni\nkqotsPVu+wKBgQCfWHUIEatR6I9AGGfHWsvDPjo4jp1yftCzspev/KVNxnf8g38S\nmoetAn8umt6IfQ1PnL8TDUQNxsLlbPXkgwuM9rFBk3bdehJaJ3LPUMrrh0FioFeE\noyvHKAJ1jXD+W3zCtFC9wmgiJ5kOrWEzBN2ZMPCe4V+qwrjTOIpD6yd6wQKBgQC7\nD1WBHqHr+6zRTQHmFoMloLkH5BLx8uXdU0Mgu+oES8dkMWGDP3G0D6wjjRYm9o1T\nTWz7eYmqwpdDqcEqaki3u8C+4Eoz+G3umaBBPHwxzQgHHDtUFbYM6HqNo9QU0VEh\nEjqh/SgZfINCKgGmmXFcLRjnk4ROQZSOtSfSfKpuuwKBgQDpI/c41ZSeVyYwnc3+\nSMAfNhxrN5jM/q0Ezz7/0MSlm5IyhGDrOsxDIQHwrBkgi8OAzlH1XSI/bvkCsIwq\n1Q6uRGlBaC0XKlbf99U1UCZzThNBNwkppjW1tIjQvrmlnDRdy7jKNrPISsjXI1T2\nzozuELkFvKP/47ZT/nX9gTc7XA==\n-----END PRIVATE KEY-----\n',
                                                                          'client_email': 'sicarapp-service-account@sicarapp.iam.gserviceaccount.com',
                                                                          'client_id': '115271374977398663227',
                                                                          'auth_uri': 'https://accounts.google.com/o/oauth2/auth',
                                                                          'token_uri': 'https://oauth2.googleapis.com/token',
                                                                          'auth_provider_x509_cert_url': 'https://www.googleapis.com/oauth2/v1/certs',
                                                                          'client_x509_cert_url': 'https://www.googleapis.com/robot/v1/metadata/x509/sicarapp-service-account%40sicarapp.iam.gserviceaccount.com'
                                                                        }");
            System.IO.File.WriteAllText(pathToUploadFile, @"");
            System.IO.File.WriteAllText(pathToDownloadedFile, @"");

            // Load the Service account credentials and define the scope of its access
            var credential = GoogleCredential.FromFile(pathToServiceAccountKeyFile).CreateScoped(DriveService.ScopeConstants.Drive);

            // Create the Drive service
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            /* --------------------------------------------------- */

            //List<string> l = new List<string>();
            //UpdateFileOnGoogleDrive(service, "Crop.txt", l);
            //UpdateFileOnGoogleDrive(service, "User.txt", l);

            //string[] l = {"1,30/8/2021,12:0:0,30.07,29.20,31.20,61.50,51.80,70.90,90473.00,0.02,1321.27,0.00,1.30",
            //"2,30/8/2021,13:0:0,29.92,29.10,30.80,54.38,52.80,55.90,90362.00,0.03,1385.75,0.00,1.79",
            //"3,30/8/2021,14:0:0,29.72,29.10,30.30,54.23,52.70,56.90,90247.00,0.02,1398.46,0.00,1.35",
            //"4,30/8/2021,15:0:0,27.99,27.10,29.60,77.83,53.80,99.90,90169.00,0.03,1378.19,0.00,1.41",
            //"5,30/8/2021,16:0:0,27.23,26.20,28.60,82.83,68.30,97.60,90114.00,0.01,1343.24,0.00,1.21",
            //"6,30/8/2021,17:0:0,28.25,27.90,28.80,65.86,61.80,69.90,90106.00,0.01,1340.53,0.00,1.43",
            //"7,30/8/2021,18:0:0,27.00,25.90,27.90,85.61,64.40,99.90,90199.00,0.01,1361.97,0.00,1.21",
            //"8,30/8/2021,19:0:0,24.98,24.30,25.90,97.37,95.60,99.90,90265.00,0.01,1115.85,0.00,0.89",
            //"9,30/8/2021,20:0:0,23.40,21.90,24.30,98.72,96.80,99.90,90358.00,0.01,1045.25,0.00,1.03",
            //"10,30/8/2021,21:0:0,21.81,21.70,22.00,99.90,99.90,99.90,90445.00,0.01,1121.30,0.00,1.00",
            //"11,30/8/2021,22:0:0,22.62,22.00,23.00,99.90,99.90,99.90,90534.00,0.01,1116.96,0.00,1.01",
            //"12,30/8/2021,23:0:0,22.55,22.10,22.80,99.90,99.90,99.90,90555.00,0.01,1115.66,0.00,1.01",
            //"13,31/8/2021,0:0:0,21.57,21.30,22.10,99.90,99.90,99.90,90540.00,0.00,1112.03,0.00,0.99",
            //"14,31/8/2021,5:0:0,21.57,21.30,22.10,99.90,99.90,99.90,90540.00,0.00,1112.03,0.00,0.99"};
            //List<string> data = new List<string>(l);
            //UpdateFileOnGoogleDrive(service, "WeatherStationData.txt", data);

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(0.01);
            var timer = new System.Threading.Timer((e) =>
            {
                GetUserInSession();
                CheckInternetConnection();
                GetSyncStatus();
            }, null, startTimeSpan, periodTimeSpan);

            var startTimeSpan2 = TimeSpan.Zero;
            var periodTimeSpan2 = TimeSpan.FromSeconds(25);
            var timer2 = new System.Threading.Timer((e) =>
            {
                CheckSyncOnGoogleDrive(service).Wait();
                SyncingProcessWithGoogleDrive(service);
            }, null, startTimeSpan2, periodTimeSpan2);
        }
    }
}
