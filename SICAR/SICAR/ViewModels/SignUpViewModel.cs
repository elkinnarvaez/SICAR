using SICAR.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Windows.Input;
using Xamarin.Essentials;


namespace SICAR.ViewModels
{
    public class SignUpViewModel : BaseViewModel
    {
        private string names;
        private string lastnames;
        private string username;
        private string password;
        private string retypedPassword;

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

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public string RetypedPassword
        {
            get => retypedPassword;
            set => SetProperty(ref retypedPassword, value);
        }

        public ICommand AlreadyHaveAccountTapped => new Command(async () => await Shell.Current.GoToAsync($"//{nameof(LoginPage)}"));

        public Command SignUpCommand { get; }

        private async void OnSignUpClicked(object obj)
        {
            Console.WriteLine(names);
            Console.WriteLine(lastnames);
            Console.WriteLine(username);
            Console.WriteLine(password);
            Console.WriteLine(retypedPassword);
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        public SignUpViewModel()
        {
            SignUpCommand = new Command(OnSignUpClicked);
        }
    }
}