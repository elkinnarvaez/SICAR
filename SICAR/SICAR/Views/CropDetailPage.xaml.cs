using SICAR.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace SICAR.Views
{
    public partial class CropDetailPage : ContentPage
    {
        public CropDetailPage()
        {
            InitializeComponent();
            BindingContext = new CropDetailViewModel();
        }
    }
}