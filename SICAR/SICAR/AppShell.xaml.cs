using SICAR.ViewModels;
using SICAR.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SICAR
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            //List<Crop> crops = await App.Database.GetAllCropsAsync();
            //foreach (Crop crop in crops)
            //{
            //    Console.WriteLine(crop.name);
            //}
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
