using System;
using System.Collections.Generic;

#nullable disable

namespace Bibliotheque.DataAccess.DataObjects
{
    public partial class Borrow
    {
        public int BorrowId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int BookId { get; set; }
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Book Book { get; set; }
    }
}
