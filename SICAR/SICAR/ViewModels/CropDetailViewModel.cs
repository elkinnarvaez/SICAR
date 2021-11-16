using SICAR.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Threading;

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

        Dictionary<string, string> cropCoefficients;
        Dictionary<string, string> growthStage;

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

        public int determineStage(string[] stages, int timeFromSeedtime)
        {
            int numDaysAcum = 0;
            int i = 0;
            while(i < 4)
            {
                if(numDaysAcum > timeFromSeedtime)
                {
                    return i;
                }
                numDaysAcum = numDaysAcum + int.Parse(stages[i]);
                i = i + 1;
            }
            return 4;
        }

        public float calculateCropCoefficient()
        {
            int timeFromSeedtime = DateTime.Now.Subtract(Convert.ToDateTime(date)).Days;
            string[] stages = growthStage[Type].Split(',');
            int currentStage = determineStage(stages, timeFromSeedtime);
            string[] k = cropCoefficients[Type].Split(',');
            float kc;
            if (currentStage == 1)
            {
                kc = float.Parse(k[0]);
            }
            else if (currentStage == 2)
            {
                Tuple<int, float> p1 = new Tuple<int, float>(int.Parse(stages[0]), float.Parse(k[0]));
                Tuple<int, float> p2 = new Tuple<int, float>(int.Parse(stages[0]) + int.Parse(stages[1]), float.Parse(k[1]));
                int x1 = p1.Item1;
                float y1 = p1.Item2;
                int x2 = p2.Item1;
                float y2 = p2.Item2;
                // kc = ((float.Parse(k[1]) - float.Parse(k[0])) / int.Parse(stages[1])) * (timeFromSeedtime - int.Parse(stages[0])) + float.Parse(k[0]);
                kc = ((y2 - y1) / (x2 - x1)) * (timeFromSeedtime - x1) + y1;
            }
            else if (currentStage == 3)
            {
                kc = float.Parse(k[1]);
            }
            else
            {
                Tuple<int, float> p1 = new Tuple<int, float>(int.Parse(stages[0]) + int.Parse(stages[1]) + int.Parse(stages[2]), float.Parse(k[1]));
                Tuple<int, float> p2 = new Tuple<int, float>(int.Parse(stages[0]) + int.Parse(stages[1]) + int.Parse(stages[2]) + int.Parse(stages[3]), float.Parse(k[2]));
                int x1 = p1.Item1;
                float y1 = p1.Item2;
                int x2 = p2.Item1;
                float y2 = p2.Item2;
                kc = ((y2 - y1) / (x2 - x1)) * (timeFromSeedtime - x1) + y1;
            }
            return kc;
        }

        public async Task<float> retrieveAverageET0()
        {
            //List<WeatherStationData> allWeatherStationData = await App.Database.GetAllWeatherStationDataFromSpecificDate(DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"));
            //List<WeatherStationData> allWeatherStationData = await App.Database.GetAllWeatherStationDataFromSpecificDate("30/8/21");
            List<WeatherStationData> allWeatherStationData = await App.Database.GetAllWeatherStationData();
            float et0 = 0;
            int n = 0;
            foreach(var weatherStationData in allWeatherStationData)
            {
                et0 = et0 + weatherStationData.Evapotranspiration;
                n = n + 1;
            }
            return et0/n;
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
                float kc = calculateCropCoefficient();
                //Console.WriteLine("------------------------------------------");
                //Console.WriteLine(kc);
                //Console.WriteLine("------------------------------------------");
                float et0 = await retrieveAverageET0();
                //Console.WriteLine("------------------------------------------");
                //Console.WriteLine(et0);
                //Console.WriteLine("------------------------------------------");
                float etc = kc * et0;
                //Console.WriteLine("------------------------------------------");
                //Console.WriteLine(etc);
                //Console.WriteLine("------------------------------------------");
                float litersPerHectarePerDay = etc * 10 * 1000;
                float litersPerDay = litersPerHectarePerDay * Hectare;
                Console.WriteLine("------------------------------------------");
                Console.WriteLine(litersPerHectarePerDay);
                Console.WriteLine("------------------------------------------");
                Console.WriteLine("------------------------------------------");
                Console.WriteLine(litersPerDay);
                Console.WriteLine("------------------------------------------");
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Crop");
            }
        }

        private void initializeData()
        {
            cropCoefficients = new Dictionary<string, string>()
            {
                {"Zanahoria", "0.7,1.05,0.95"},
                {"Cebolla verde", "0.7,1.0,1.0"},
                {"Tomate", "0.6,1.15,0.70"},
                {"Papa", "0.5,1.15,0.75"},
                {"Frijol verde", "0.5,1.05,0.90"},
                {"Frijol seco", "0.4,1.15,0.35"},
                {"Maíz", "0.3,1.20,0.35"},
                {"Caña de Azúcar (virgen)", "0.40,1.25,0.75"},
                {"Caña de Azúcar (soca)", "0.40,1.25,0.75"},
                {"Piña", "0.50,0.30,0.30"}
            };

            growthStage = new Dictionary<string, string>()
            {
                {"Zanahoria", "20,30,30,20"},
                {"Cebolla verde", "20,45,20,10"},
                {"Tomate", "30,40,40,25"},
                {"Papa", "25,30,45,30"},
                {"Frijol verde", "20,30,30,10"},
                {"Frijol seco", "20,30,40,20"},
                {"Maíz", "20,35,40,30"},
                {"Caña de Azúcar (virgen)", "50,70,220,140"},
                {"Caña de Azúcar (soca)", "30,50,180,60"},
                {"Piña", "60,120,600,10"}
            };
        }

        public CropDetailViewModel()
        {
            initializeData();
        }
    }
}
