using SICAR.Models;
using SICAR.ViewModels;
using SICAR.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SICAR.Views
{
    public partial class CropsPage : ContentPage
    {
        CropsViewModel _viewModel;

        public CropsPage()
        {
            InitializeComponent();
            this.BindingContext = _viewModel = new CropsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}