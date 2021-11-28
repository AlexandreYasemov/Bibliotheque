using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Bibliotheque.DataAccess.DataObjects
{
    public partial class PhpfribiblioContext : DbContext
    {
        public PhpfribiblioContext()
        {
            /*using (var context = new PhpfribiblioContext())
            {
                context.Database.EnsureCreated();
            }*/
        }

        public PhpfribiblioContext(DbContextOptions<PhpfribiblioContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Borrow> Borrows { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Publisher> Publishers { get; set; }
        public virtual DbSet<Type> Types { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"data source=database.db");
            }
        }

        //Since we switched from MySQL to Sqlite, discard that part

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("account");

                entity.Property(e => e.AccountId)
                    .HasColumnType("int(64)");
                    

                entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");

                entity.Property(e => e.LocationId)
                    .HasColumnType("int(64)")
                    .HasColumnName("locationId")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(512)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasColumnType("binary(32)")
                    .HasColumnName("password")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Salt)
                    .HasColumnType("binary(255)")
                    .HasColumnName("salt")
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<Author>(entity =>
            {
                entity.ToTable("author");

                entity.Property(e => e.AuthorId)
                    .HasColumnType("int(64)")
                    .HasColumnName("authorId");

                entity.Property(e => e.Description)
                    .HasMaxLength(512)
                    .HasColumnName("description")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("book");

                entity.HasIndex(e => e.TypeId, "book_ibfk_1");

                entity.HasIndex(e => e.AuthorId, "book_ibfk_2");

                entity.HasIndex(e => e.LocationId, "book_ibfk_3");

                entity.HasIndex(e => e.PublisherId, "book_ibfk_4");

                entity.Property(e => e.BookId)
                    .HasColumnType("int(64)")
                    .HasColumnName("bookId");

                entity.Property(e => e.AuthorId)
                    .HasColumnType("int(64)")
                    .HasColumnName("authorId");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(512)
                    .HasColumnName("description");

                entity.Property(e => e.IsArchived)
                    .HasColumnType("bit(1)")
                    .HasColumnName("isArchived");

                entity.Property(e => e.Isbn)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("isbn");

                entity.Property(e => e.Language)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("language");

                entity.Property(e => e.LocationId)
                    .HasColumnType("int(64)")
                    .HasColumnName("locationId");

                entity.Property(e => e.PublisherId)
                    .HasColumnType("int(64)")
                    .HasColumnName("publisherId");

                entity.Property(e => e.Status)
                    .HasColumnType("int(4)")
                    .HasColumnName("status");

                entity.Property(e => e.Tags)
                    .IsRequired()
                    .HasMaxLength(512)
                    .HasColumnName("tags");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("title");

                entity.Property(e => e.TypeId)
                    .HasColumnType("int(64)")
                    .HasColumnName("typeId");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("book_ibfk_2");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("book_ibfk_3");

                entity.HasOne(d => d.Publisher)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.PublisherId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("book_ibfk_4");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("book_ibfk_1");
            });

            modelBuilder.Entity<Borrow>(entity =>
            {
                entity.ToTable("borrow");

                entity.HasIndex(e => e.AccountId, "accountId");

                entity.HasIndex(e => e.BookId, "bookId");

                entity.Property(e => e.BorrowId)
                    .HasColumnType("int(64)")
                    .HasColumnName("borrowId");

                entity.Property(e => e.AccountId)
                    .HasColumnType("int(64)")
                    .HasColumnName("accountId");

                entity.Property(e => e.BookId)
                    .HasColumnType("int(64)")
                    .HasColumnName("bookId");

                entity.Property(e => e.DueDate)
                    .HasColumnType("date")
                    .HasColumnName("due_Date");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_Date")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_Date");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Borrows)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("borrow_ibfk_2");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.Borrows)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("borrow_ibfk_1");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("location");

                entity.Property(e => e.LocationId)
                    .HasColumnType("int(64)")
                    .HasColumnName("locationId");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(512)
                    .HasColumnName("city");

                entity.Property(e => e.RoomName)
                    .IsRequired()
                    .HasMaxLength(512)
                    .HasColumnName("roomName");

                entity.Property(e => e.StreetName)
                    .IsRequired()
                    .HasMaxLength(512)
                    .HasColumnName("streetName");
            });

            modelBuilder.Entity<Publisher>(entity =>
            {
                entity.ToTable("publisher");

                entity.Property(e => e.PublisherId)
                    .HasColumnType("int(64)")
                    .HasColumnName("publisherId");

                entity.Property(e => e.Description)
                    .HasMaxLength(512)
                    .HasColumnName("description")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Type>(entity =>
            {
                entity.ToTable("type");

                entity.Property(e => e.TypeId)
                    .HasColumnType("int(64)")
                    .HasColumnName("typeId");

                entity.Property(e => e.Description)
                    .HasMaxLength(512)
                    .HasColumnName("description")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }*/

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
