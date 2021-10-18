using SICAR.Models;
using SICAR.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SICAR.Views
{
    public partial class NewCropPage : ContentPage
    {
        public Crop Crop { get; set; }

        public NewCropPage()
        {
            InitializeComponent();
            BindingContext = new NewCropViewModel();
        }
    }
}