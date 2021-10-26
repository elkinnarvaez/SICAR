using SICAR.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SICAR.ViewModels
{
    [QueryProperty(nameof(CropId), nameof(CropId))]
    public class CropDetailViewModel : BaseViewModel
    {
        private int cropId;
        private string name;
        private string type;
        private string date;
        private int hectare;
        private string ground;
        private int deep;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Type
        {
            get => type;
            set => SetProperty(ref type, value);
        }

        public string Date
        {
            get => date;
            set => SetProperty(ref date, value);
        }

        public int Hectare
        {
            get => hectare;
            set => SetProperty(ref hectare, value);
        }

        public string Ground
        {
            get => ground;
            set => SetProperty(ref ground, value);
        }

        public int Deep
        {
            get => deep;
            set => SetProperty(ref deep, value);
        }

        public int CropId
        {
            get
            {
                return cropId;
            }
            set
            {
                cropId = value;
                LoadCropId(value);
            }
        }

        public async void LoadCropId(int cropId)
        {
            try
            {
                var crop = await App.Database.GetCropAsync(cropId);
                Name = crop.Name;
                Type = crop.Type;
                Date = crop.Date;
                Hectare = crop.Hectare;
                Ground = crop.Ground;
                Deep = crop.Deep;
                Title = Name;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Crop");
            }
        }

        public CropDetailViewModel()
        {
         
        }
    }
}
