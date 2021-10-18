using System;
using SQLite;

namespace SICAR.Models
{
    public class Session
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        // User info
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Names { get; set; }
        public string Lastnames { get; set; }
        public DateTime LoginTime { get; set; }
    }
}
