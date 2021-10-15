using System;
using SQLite;
using SICAR.Models;

namespace SICAR.Models
{
    public class Session
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public User user { get; set; }
        public DateTime loginTime { get; set; }
    }
}
