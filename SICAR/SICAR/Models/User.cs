using System;
using SQLite;

namespace SICAR.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Names { get; set; }
        public string Lastnames { get; set; }
    }
}