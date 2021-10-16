using System;
using SQLite;

namespace SICAR.Models
{
    public class Session
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        // User info
        public int userId { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string names { get; set; }
        public string lastnames { get; set; }
        public DateTime loginTime { get; set; }
    }
}
