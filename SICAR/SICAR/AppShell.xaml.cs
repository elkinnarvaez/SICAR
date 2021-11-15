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

        public void checkInternetConnection()
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

        /* Google Drive public methods */

        public void ListFilesOnGoogleDrive(DriveService service)
        {
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
            if(fileId != directoryId)
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

        public List<string> getLinesFileContentOnGoogleDrive(DriveService service, string fileName)
        {
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

        public async void UpdateFileOnGoogleDrive(DriveService service, string fileName, string newFileContent)
        {
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
            System.IO.File.WriteAllText(pathToUploadFile, newFileContent);

            //Upload file content
            var file = service.Files.Get(fileId).Execute();
            var newFileMetadata = new Google.Apis.Drive.v3.Data.File();
            using (var fsSource = new FileStream(pathToUploadFile, FileMode.Open, FileAccess.Read))
            {
                var request = service.Files.Update(newFileMetadata, file.Id, fsSource, "text/plain");
                var results = await request.UploadAsync();
                if (results.Status == Google.Apis.Upload.UploadStatus.Failed)
                {
                    Console.WriteLine($"Error updating file: {results.Exception.Message}");
                }
            }
        }

        public async void UploadFileToGoogleDrive(DriveService service, string fileContent, string fileName)
        {
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
                var results = await request.UploadAsync(CancellationToken.None);

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

            UploadFileToGoogleDrive(service, "", "Crop.txt");
            UploadFileToGoogleDrive(service, "", "Item.txt");
            UploadFileToGoogleDrive(service, "", "Session.txt");
            UploadFileToGoogleDrive(service, "", "User.txt");

            /* --------------------------------------------------- */

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(0.01);
            var timer = new System.Threading.Timer((e) =>
            {
                GetUserInSession();
                checkInternetConnection();
            }, null, startTimeSpan, periodTimeSpan);
        }
    }
}
