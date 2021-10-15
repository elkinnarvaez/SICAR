﻿using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SICAR.Models
{
    public class Crop
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public int user { get; set; }
        public string name { get; set; }

        public string type { get; set; }

        public string date { get; set; }

        public int hectare { get; set; }

        //public string Description { get; set; }

        //public override string ToString()
        //{
        //    return Name;
        //}
    }
}
