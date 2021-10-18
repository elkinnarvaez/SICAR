using SICAR.ViewModels;
using SICAR.Views;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Xamarin.Forms;
using SICAR.Models;
using System.Threading.Tasks;
using System.Threading;

namespace SICAR
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        private string names;
        private string lastnames;
        private string username;

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

        public AppShell()
        {
            InitializeComponent();
            this.BindingContext = this;
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(NewCropPage), typeof(NewCropPage));
            //GetUserInSession();
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(0.01);
            var timer = new System.Threading.Timer((e) =>
            {
                GetUserInSession();
            }, null, startTimeSpan, periodTimeSpan);
        }
    }
}
