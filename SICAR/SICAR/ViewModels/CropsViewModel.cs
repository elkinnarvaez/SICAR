using SICAR.Models;
using SICAR.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SICAR.ViewModels
{
    public class CropsViewModel : BaseViewModel
    {
        private Crop _selectedCrop;
        private bool emptyCropsList;

        public ObservableCollection<Crop> Crops { get; }
        public Command LoadCropsCommand { get; }
        public Command AddCropCommand { get; }
        public Command<Crop> CropTapped { get; }

        public CropsViewModel()
        {
            Title = "Mis cultivos";
            Crops = new ObservableCollection<Crop>();
            LoadCropsCommand = new Command(async () => await ExecuteLoadCropsCommand());

            CropTapped = new Command<Crop>(OnCropSelected);

            AddCropCommand = new Command(OnAddCrop);
        }

        async Task ExecuteLoadCropsCommand()
        {
            IsBusy = true;

            try
            {
                Crops.Clear();
                Session session = await App.Database.GetCurrentSessionAsync();
                var crops = await App.Database.GetCropsOfUserAsync(session.Username);
                if (crops.Count == 0)
                {
                    EmptyCropsList = true;
                }
                else
                {
                    EmptyCropsList = false;
                }
                foreach (var crop in crops)
                {
                    Crops.Add(crop);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedCrop = null;
        }

        public Crop SelectedCrop
        {
            get => _selectedCrop;
            set
            {
                SetProperty(ref _selectedCrop, value);
                OnCropSelected(value);
            }
        }

        private async void OnAddCrop(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewCropPage));
        }

        async void OnCropSelected(Crop crop)
        {
            if (crop == null)
                return;

            // This will push the CropDetailPage onto the navigation stack
            // await Shell.Current.GoToAsync($"{nameof(CropDetailPage)}?{nameof(CropDetailViewModel.CropId)}={crop.id}");
            await Shell.Current.GoToAsync(nameof(CropDetailPage));
        }

        public bool EmptyCropsList
        {
            get => emptyCropsList;
            set => SetProperty(ref emptyCropsList, value);
        }
    }
}