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
    public partial class AddNewCropsPage : ContentPage
    {
        public AddNewCropsPage()
        {
            InitializeComponent();
            this.BindingContext = new AddNewCropsViewModel();
        }
    }
}