using Bibliotheque.DataAccess.DataObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibliotheque.helpers
{
    static class Logging
    {

        static private List<EntityEntry> buffer = new List<EntityEntry>();
        static private EntityState state;

        static public void dbSave_Saving(object sender, SavingChangesEventArgs e)
        {
            try
            {
                buffer.Clear();
                buffer.AddRange(((PhpfribiblioContext)sender).ChangeTracker.Entries().Where(c => c.State == EntityState.Added).ToList());
                buffer.AddRange(((PhpfribiblioContext)sender).ChangeTracker.Entries().Where(c => c.State == EntityState.Modified).ToList());
                buffer.AddRange(((PhpfribiblioContext)sender).ChangeTracker.Entries().Where(c => c.State == EntityState.Deleted).ToList());
                state = buffer[0].State;
            }
            catch
            {

            }
        }

        static public void dbSave_Success(object sender, SavedChangesEventArgs e)
        {
            object entity = buffer[0].Entity;



            switch (state)
            {
                case EntityState.Added:
                    switch (buffer[0].Entity.GetType().Name)
                    {
                        case nameof(Book):
                            Globals.mainWindow.ShowMessage($"Book {((Book)entity).Title} has been successfully added", false);
                            break;
                        case nameof(Location):
                            Globals.mainWindow.ShowMessage($"Location {((Location)entity).RoomName}  has been successfully added", false);
                            break;
                        case nameof(Account):
                            Globals.mainWindow.ShowMessage($"User {((Account)entity).Name} has been successfully added", false);
                            break;

                        case nameof(Borrow):
                            Globals.mainWindow.ShowMessage($"Reservation {((Borrow)entity).Book.Title} from {((Borrow)entity).StartDate.ToShortDateString()} to {((Borrow)entity).DueDate.ToShortDateString()} has been successfully added", false);
                            break;

                        case nameof(DataAccess.DataObjects.Type):
                            Globals.mainWindow.ShowMessage($"Genre {((DataAccess.DataObjects.Type)entity).Name} has been successfully added", false);
                            break;

                        case nameof(Author):
                            Globals.mainWindow.ShowMessage($"Author {((Author)entity).Name} has been successfully added", false);
                            break;

                        case nameof(Publisher):
                            Globals.mainWindow.ShowMessage($"Publisher {((Publisher)entity).Name} has been successfully added", false);
                            break;
                    }
                    break;
                case EntityState.Modified:
                    switch (entity.GetType().Name)
                    {
                        case nameof(Book):
                            Globals.mainWindow.ShowMessage($"Book {((Book)entity).Title} has been successfully updated", false);
                            break;
                        case nameof(Location):
                            Globals.mainWindow.ShowMessage($"Location {((Location)entity).RoomName}  has been successfully updated", false);
                            break;
                        case nameof(Account):
                            Globals.mainWindow.ShowMessage($"User {((Account)entity).Name} has been successfully updated", false);
                            break;

                        case nameof(Borrow):
                            Globals.mainWindow.ShowMessage($"Reservation {((Borrow)entity).Book.Title} from {((Borrow)entity).StartDate.ToShortDateString()} to {((Borrow)entity).DueDate.ToShortDateString()} has been successfully updated", false);
                            break;

                        case nameof(DataAccess.DataObjects.Type):
                            Globals.mainWindow.ShowMessage($"Genre {((DataAccess.DataObjects.Type)entity).Name} has been successfully updated", false);
                            break;

                        case nameof(Author):
                            Globals.mainWindow.ShowMessage($"Author {((Author)entity).Name} has been successfully updated", false);
                            break;

                        case nameof(Publisher):
                            Globals.mainWindow.ShowMessage($"Publisher {((Publisher)entity).Name} has been successfully updated", false);
                            break;
                    }
                    break;

                case EntityState.Deleted:
                    switch (entity.GetType().Name)
                    {
                        case nameof(Book):
                            Globals.mainWindow.ShowMessage($"Book {((Book)entity).Title} has been successfully deleted", false);
                            break;
                        case nameof(Location):
                            Globals.mainWindow.ShowMessage($"Location {((Location)entity).RoomName}  has been successfully deleted", false);
                            break;
                        case nameof(Account):
                            Globals.mainWindow.ShowMessage($"User {((Account)entity).Name} has been successfully deleted", false);
                            break;

                        case nameof(Borrow):
                            Globals.mainWindow.ShowMessage($"Reservation {((Borrow)entity).Book.Title} from {((Borrow)entity).StartDate.ToShortDateString()} to {((Borrow)entity).DueDate.ToShortDateString()} has been successfully deleted", false);
                            break;

                        case nameof(DataAccess.DataObjects.Type):
                            Globals.mainWindow.ShowMessage($"Genre {((DataAccess.DataObjects.Type)entity).Name} has been successfully deleted", false);
                            break;

                        case nameof(Author):
                            Globals.mainWindow.ShowMessage($"Author {((Author)entity).Name} has been successfully deleted", false);
                            break;

                        case nameof(Publisher):
                            Globals.mainWindow.ShowMessage($"Publisher {((Publisher)entity).Name} has been successfully deleted", false);
                            break;
                    }
                    break;
            }
        }
        static public void dbSave_Fail(object sender, SaveChangesFailedEventArgs e)
        {
            Globals.mainWindow.ShowMessage($"Could not save in database", true);
        }
    }
}
