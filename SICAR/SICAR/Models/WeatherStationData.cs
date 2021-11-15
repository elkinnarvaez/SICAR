using System;
using SQLite;

namespace SICAR.Models
{
    public class WeatherStationData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public float AveargeTemp { get; set; }
        public float MinTemp { get; set; }
        public float MaxTemp { get; set; }
        public float AverageHumidity { get; set; }
        public float MinHumidity { get; set; }
        public float MaxHumidity { get; set; }
        public float AtmosphericPressure { get; set; }
        public float WindSpeed { get; set; }
        public float SolarRadiation { get; set; }
        public float Precipitation { get; set; }
        public float Evapotranspiration { get; set; }
    }
}
