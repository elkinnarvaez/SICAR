using SICAR.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Windows.Input;

namespace SICAR.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string username;
        private string password;

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

        public ICommand DoNotHaveAccountTapped => new Command(async () => await Shell.Current.GoToAsync($"//{nameof(SignUpPage)}"));

        public Command LoginCommand { get; }

        private async void OnLoginClicked(object obj)
        {
            Console.WriteLine(username);
            Console.WriteLine(password);
            await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
        }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

    }
}
