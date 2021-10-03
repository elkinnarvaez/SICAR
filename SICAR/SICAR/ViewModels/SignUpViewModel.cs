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
        public ICommand AlreadyHaveAccountTapped => new Command(async () => await Shell.Current.GoToAsync($"//{nameof(LoginPage)}"));

        public SignUpViewModel()
        {

        }
    }
}