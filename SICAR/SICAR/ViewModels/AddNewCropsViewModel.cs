using SICAR.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Windows.Input;
using Xamarin.Essentials;
using SICAR.Models;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace SICAR.ViewModels
{
    public class AddNewCropsViewModel : BaseViewModel
    {
        private string type;
        private string name;
        private string user;
        private string date;
        private string hectare;


        public string Type
        {
            get => type;
            set => SetProperty(ref type, value);
        }
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Date
        {
            get => date;
            set => SetProperty(ref date, value);
        }

        public string Hectare
        {
            get => hectare;
            set => SetProperty(ref hectare, value);
        }

        public Command AddNewCropsCommand { get; }

        private async Task<int> ValidateNewCrop()
        {
            int newCropErrorCode;
            if(!String.IsNullOrWhiteSpace(name) && !String.IsNullOrWhiteSpace(type) && !String.IsNullOrWhiteSpace(date) && !String.IsNullOrWhiteSpace(hectare))
            {
                newCropErrorCode = -1;
            }
            else
            {
                newCropErrorCode = 0;
            }
            return newCropErrorCode;
        }

        private async void OnAddNewCropsClicked(object obj)
        {
            int newCropErrorCode = await ValidateNewCrop();
            if (newCropErrorCode == -1)
            {
                Crop newCrop = new Crop()
                {
                    name = name,
                    type = type,
                    user = "pruebas",
                    date = date,
                    hectare = Int32.Parse(hectare)
                };
                await App.Database.SaveCropAsync(newCrop);
                name = "";
                type = "";
                user = "";
                date = "";
                hectare = "";
                await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
            }
            else if(newCropErrorCode == 0)
            {
                await Shell.Current.DisplayAlert("Campos en blanco", "Por favor verifique que todos los campos estén llenos.", "OK");
            }
        }

        public AddNewCropsViewModel()
        {
            Title = "Agregar cultivo";
            AddNewCropsCommand = new Command(OnAddNewCropsClicked);
        }
    }
}
