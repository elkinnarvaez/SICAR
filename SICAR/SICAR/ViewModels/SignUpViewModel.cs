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

        private async Task<int> ValidateSignUp()
        {
            int signUpErrorCode;
            if(!String.IsNullOrWhiteSpace(names) && !String.IsNullOrWhiteSpace(lastnames) && !String.IsNullOrWhiteSpace(username) && !String.IsNullOrWhiteSpace(password) && !String.IsNullOrWhiteSpace(retypedPassword))
            {
                List<User> users = await App.Database.GetUsersAsync();
                bool usernameFound = false;
                foreach(User user in users)
                {
                    if(user.username == username)
                    {
                        usernameFound = true;
                    }
                }
                if (usernameFound == true)
                {
                    signUpErrorCode = 1;
                }
                else if (password != retypedPassword)
                {
                    signUpErrorCode = 2;
                }
                else if (password.Length < 6)
                {
                    signUpErrorCode = 3;
                }
                else
                {
                    signUpErrorCode = -1;
                }
            }
            else
            {
                signUpErrorCode = 0;
            }
            return signUpErrorCode;
        }

        private async void OnSignUpClicked(object obj)
        {
            Console.WriteLine(names);
            Console.WriteLine(lastnames);
            Console.WriteLine(username);
            Console.WriteLine(password);
            Console.WriteLine(retypedPassword);
            int signUpErrorCode = await ValidateSignUp();
            if(signUpErrorCode == -1)
            {
                User newUser = new User()
                {
                    username = username,
                    password = password,
                    names = names,
                    lastnames = lastnames
                };
                await App.Database.SaveUserAsync(newUser);
                Names = "";
                Lastnames = "";
                Username = "";
                Password = "";
                RetypedPassword = "";
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            else if (signUpErrorCode == 0)
            {
                await Shell.Current.DisplayAlert("Campos en blanco", "Por favor verifique que todos los campos estén llenos.", "OK");
            }
            else if(signUpErrorCode == 1)
            {
                await Shell.Current.DisplayAlert("Usuario existente", "El nombre de usuario ingresado ya existe.", "OK");
            }
            else if(signUpErrorCode == 2)
            {
                await Shell.Current.DisplayAlert("Contraseñas no coinciden", "Las contraseñas ingresadas no coinciden.", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Contraseña muy corta", "La contraseña ingresada es muy corta. Esta debe ser de al menos 6 caracteres.", "OK");
            }
        }

        public SignUpViewModel()
        {
            SignUpCommand = new Command(OnSignUpClicked);
        }
    }
}