using System;
using System.Collections.Generic;

#nullable disable

namespace Bibliotheque.DataAccess.DataObjects
{
    public partial class Location
    {
        public Location()
        {
            Books = new HashSet<Book>();
        }

        public int LocationId { get; set; }
        public string RoomName { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
