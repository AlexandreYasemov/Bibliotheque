using System;
using System.Collections.Generic;

#nullable disable

namespace Bibliotheque.DataAccess.DataObjects
{
    public partial class Type
    {
        public Type()
        {
            Books = new HashSet<Book>();
        }

        public int TypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
