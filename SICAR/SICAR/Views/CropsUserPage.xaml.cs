using SICAR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SICAR.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CropsUserPage : ContentPage
    {
        public CropsUserPage()
        {
            InitializeComponent();
            this.BindingContext = new CropsUserPage();
        }
    }
}