using Bibliotheque.DataAccess.DataObjects;
using Bibliotheque.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bibliotheque.forms
{
    /// <summary>
    /// Interaction logic for BookFormExtra.xaml
    /// </summary>
    public partial class BookFormExtra : Page
    {
        Book currentBook;
        PhpfribiblioContext context;
        public BookFormExtra(Book book, string authorName, string publisherName, string location, string typeName)
        {
            InitializeComponent();


            currentBook = book;
            context = Globals.databaseContext;

            tb_author_name.Text = authorName;
            tb_publisher_name.Text = publisherName;
            tb_type_name.Text = typeName;
            tb_location_room.Text = location;

            //Show only the new Authors/Publishers
            if (currentBook.Author == null)
            {
                sp_author.Visibility = Visibility.Visible;
            }
            else if (currentBook.Author.Name != authorName)
            {
                sp_author.Visibility = Visibility.Visible;

            }
            if (currentBook.Publisher == null)
            {
                sp_publisher.Visibility = Visibility.Visible;
            }
            else if (currentBook.Publisher.Name != publisherName)
            {
                sp_publisher.Visibility = Visibility.Visible;
            }
            if (currentBook.Type == null)
            {
                sp_type.Visibility = Visibility.Visible;
            }
            else if (currentBook.Type.Name != typeName)
            {
                sp_type.Visibility = Visibility.Visible;
            }
            if (currentBook.Location == null)
            {
                sp_location.Visibility = Visibility.Visible;
            }
            else if ($"{currentBook.Location.RoomName}, {currentBook.Location.StreetName}, {currentBook.Location.City}" != location)
            {
                sp_location.Visibility = Visibility.Visible;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            if (FindAuthorId(tb_author_name.Text) == 0)
            {
                Author author = new Author();
                author.Name = tb_author_name.Text;
                author.Description = tb_author_description.Text;
                currentBook.Author = author;
            }
            else
            {
                currentBook.AuthorId = FindAuthorId(tb_author_name.Text);
            }
            if (FindPublisherId(tb_publisher_name.Text) == 0)
            {
                Publisher publisher = new Publisher();
                publisher.Name = tb_publisher_name.Text;
                publisher.Description = tb_publisher_description.Text;
                currentBook.Publisher = publisher;
            }
            else
            {
                currentBook.PublisherId = FindPublisherId(tb_publisher_name.Text);
            }
            if (FindLocationId(tb_location_room.Text, tb_location_street.Text, tb_location_city.Text) == 0)
            {
                Location location = new Location();
                location.RoomName = tb_location_room.Text;
                location.StreetName = tb_location_street.Text;
                location.City = tb_location_city.Text;
                currentBook.Location = location;
            }
            else
            {
                currentBook.LocationId = FindLocationId(tb_location_room.Text, tb_location_street.Text, tb_location_city.Text);
            }
            if (FindTypeId(tb_type_name.Text) == 0)
            {
                Bibliotheque.DataAccess.DataObjects.Type type = new Bibliotheque.DataAccess.DataObjects.Type();
                type.Name = tb_type_name.Text;
                type.Description = tb_type_description.Text;
                currentBook.Type = type;
            }
            else
            {
                currentBook.TypeId = FindTypeId(tb_type_name.Text);
            }


            if (currentBook.BookId != 0)
                context.Update(currentBook);

            context.SaveChanges();

            Globals.mainWindow.CloseForm();
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            BookForm page = new BookForm(currentBook);
            Globals.mainWindow.ShowForm(page);
            page.ReturnPassData(currentBook, tb_author_name.Text, tb_publisher_name.Text, tb_location_room.Text, tb_type_name.Text);
        }

        private int FindAuthorId(string authorName)
        {
            return context.Authors.Where(author => author.Name.ToLower() == authorName.ToLower()).Select(a => a.AuthorId).FirstOrDefault();
        }

        private int FindPublisherId(string publisherName)
        {
            return context.Publishers.Where(publisher => publisher.Name.ToLower() == publisherName.ToLower()).Select(a => a.PublisherId).FirstOrDefault();
        }

        private int FindLocationId(string locationRoom, string locationStreet, string locationCity)
        {
            return context.Locations.Where(c => c.RoomName.ToLower() == locationRoom.ToLower() && c.StreetName == locationStreet.ToLower() && c.City == locationCity.ToLower()).Select(c => c.LocationId).FirstOrDefault();
        }
        private int FindTypeId(string type)
        {
            return context.Types.Where(x => x.Name.ToLower() == type.ToLower()).Select(a => a.TypeId).FirstOrDefault();
        }
    }
}
