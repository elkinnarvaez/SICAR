using SICAR.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Windows.Input;
using SICAR.Models;
using System.Threading.Tasks;

namespace SICAR.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string username;
        private string password;
        private string names;
        private string lastnames;

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

        private async Task<int> ValidateLogin()
        {
            int loginErrorCode;
            if (!String.IsNullOrWhiteSpace(username) && !String.IsNullOrWhiteSpace(password))
            {
                List<User> users = await App.Database.GetUsersAsync();
                bool usernameFound = false;
                bool passwordIsCorrect = true;
                foreach (User user in users)
                {
                    if (user.Username == username)
                    {
                        usernameFound = true;
                        if (user.Password != password)
                        {
                            passwordIsCorrect = false;
                        }
                        else
                        {
                            names = user.Names;
                            lastnames = user.Lastnames;
                        }
                    }
                }
                if (usernameFound == false)
                {
                    loginErrorCode = 1;
                }
                else if(passwordIsCorrect == false)
                {
                    loginErrorCode = 2;
                }
                else
                {
                    loginErrorCode = -1;
                }
            }
            else
            {
                loginErrorCode = 0;
            }
            return loginErrorCode;
        }

        private async void OnLoginClicked(object obj)
        {
            //Console.WriteLine(username);
            //Console.WriteLine(password);
            int loginErrorCode = await ValidateLogin();
            if (loginErrorCode == -1)
            {
                // TODO: need to save the user info for further user in the application
                Session newSession = new Session()
                {
                    Username = username,
                    Password = password,
                    Names = names,
                    Lastnames = lastnames,
                    LoginTime = DateTime.Now
                };
                await App.Database.SaveSessionAsync(newSession);
                Username = "";
                Password = "";
                await Shell.Current.GoToAsync($"//{nameof(CropsPage)}");
            }
            else if (loginErrorCode == 0)
            {
                await Shell.Current.DisplayAlert("Campos en blanco", "Por favor verifique que todos los campos estén llenos.", "OK");
            }
            else if(loginErrorCode == 1)
            {
                await Shell.Current.DisplayAlert("Usuario no existe", "El nombre de usuario ingresado no existe.", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Contraseña incorrecta", "La contraseña ingresada es incorrecta.", "OK");
            }
        }

        private async void verifyExistingSession()
        {
            Session session = await App.Database.GetCurrentSessionAsync();
            if(session != null)
            {
                if(DateTime.Now.Subtract(session.LoginTime).Days <= 10)
                {
                    await Shell.Current.GoToAsync($"//{nameof(CropsPage)}");
                }
                else
                {
                    await App.Database.DeleteSessionAsync(session);
                }
            }
        }

        public LoginViewModel()
        {
            verifyExistingSession();
            LoginCommand = new Command(OnLoginClicked);
        }

    }
}
