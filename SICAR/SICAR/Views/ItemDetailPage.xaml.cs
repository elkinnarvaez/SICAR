using SICAR.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace SICAR.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}