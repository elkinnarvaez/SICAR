using System;
using SQLite;

namespace SICAR.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string names { get; set; }
        public string lastnames { get; set; }
    }
}