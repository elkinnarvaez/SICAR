using System;
using SQLite;

namespace SICAR.Models
{
    public class Sync
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public bool isSynced { get; set; }
    }
}
