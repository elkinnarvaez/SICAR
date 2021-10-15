using System;
using System.Collections.Generic;
using System.Text;
using SICAR.Models;
using SICAR.Views;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SICAR.ViewModels
{
    public class AddNewCropsViewModel : BaseViewModel
    {
        private string typeCrop;
        private string nameCrop;
        private string user;
        private string selectedDate;
        private int hectaresCrop;

        public string TypeCrop
        {
            get => typeCrop;
            set => SetProperty(ref typeCrop, value);
        }
        public string NameCrop
        {
            get => nameCrop;
            set => SetProperty(ref nameCrop, value);
        }

        public string SelectedDate
        {
            get => selectedDate;
            set => SetProperty(ref selectedDate, value);
        }

        public int HectaresCrop
        {
            get => hectaresCrop;
            set => SetProperty(ref hectaresCrop, value);
        }
    }
}
