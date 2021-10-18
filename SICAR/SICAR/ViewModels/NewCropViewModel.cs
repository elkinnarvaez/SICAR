using SICAR.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SICAR.ViewModels
{
    public class NewCropViewModel : BaseViewModel
    {
        private string type;
        private string name;
        private string username;
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

        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        public Command AddNewCropCommand { get; }

        private int ValidateNewCrop()
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

        private async void OnAddNewCropClicked(object obj)
        {
            int newCropErrorCode = ValidateNewCrop();
            if (newCropErrorCode == -1)
            {
                Session session = await App.Database.GetCurrentSessionAsync();
                Crop newCrop = new Crop()
                {
                    Name = name,
                    Type = type,
                    Username = session.Username,
                    Date = date,
                    Hectare = Int32.Parse(hectare)
                };
                await App.Database.SaveCropAsync(newCrop);
                Name = "";
                Type = "";
                Username = "";
                Date = "";
                Hectare = "";
                await Shell.Current.GoToAsync("..");
            }
            else if(newCropErrorCode == 0)
            {
                await Shell.Current.DisplayAlert("Campos en blanco", "Por favor verifique que todos los campos estén llenos.", "OK");
            }
        }

        public NewCropViewModel()
        {
            Title = "Nuevo Cultivo";
            AddNewCropCommand = new Command(OnAddNewCropClicked);
            this.PropertyChanged +=
                (_, __) => AddNewCropCommand.ChangeCanExecute();
        }
    }
}
