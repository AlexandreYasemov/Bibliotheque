using System;
using System.Collections.Generic;

#nullable disable

namespace Bibliotheque.DataAccess.DataObjects
{
    public partial class Account
    {
        public Account()
        {
            Borrows = new HashSet<Borrow>();
        }

        public int AccountId { get; set; }
        public string Name { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public bool IsAdmin { get; set; }
        public int? LocationId { get; set; }

        public virtual ICollection<Borrow> Borrows { get; set; }
    }
}
