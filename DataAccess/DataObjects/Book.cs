using System;
using System.Collections.Generic;

#nullable disable

namespace Bibliotheque.DataAccess.DataObjects
{
    public partial class Book
    {
        public Book()
        {
            Borrows = new HashSet<Borrow>();
        }

        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public string Language { get; set; }
        public int Status { get; set; }
        public string Tags { get; set; }
        public string Isbn { get; set; }
        public int LocationId { get; set; }
        public int AuthorId { get; set; }
        public int PublisherId { get; set; }
        public int TypeId { get; set; }
        public bool IsArchived { get; set; }

        public virtual Author Author { get; set; }
        public virtual Location Location { get; set; }
        public virtual Publisher Publisher { get; set; }
        public virtual Type Type { get; set; }
        public virtual ICollection<Borrow> Borrows { get; set; }
    }
}
