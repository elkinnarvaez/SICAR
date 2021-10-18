using SICAR.Services;
using SICAR.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using SICAR.Data;

namespace SICAR
{
    public partial class App : Application
    {
        static SICARDatabase database;

        // Create the database connection as a singleton.
        public static SICARDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new SICARDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SICAR.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
            //MainPage = new AddNewCropsPage();
            // MainPage = new list_crops();
            // MainPage = new SignUpPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
