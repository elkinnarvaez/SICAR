using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SICAR.Models
{
    public class Crop
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }

        public string Type { get; set; }

        public string Date { get; set; }

        public int Hectare { get; set; }

    }
}
