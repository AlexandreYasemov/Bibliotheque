using Bibliotheque.DataAccess.DataObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.ObjectModel;
using System.Configuration;
using Bibliotheque.forms;
using Bibliotheque.helpers;

namespace Bibliotheque
{
    /// <summary>
    /// Interaction logic for ModifyBookPage.xaml
    /// </summary>
    public partial class BookForm : Page
    {
        ObservableCollection<DataAccess.DataObjects.Type> typeList = new ObservableCollection<DataAccess.DataObjects.Type>();
        ObservableCollection<LocationView> locationList = new ObservableCollection<LocationView>();
        IOrderedEnumerable<KeyValuePair<int, string>> statusList;

        private PhpfribiblioContext context;
        private Book currentBook;

        string[] authorsData;
        string[] publishersData;
        string[] locationsData;
        string[] typesData;
        public BookForm()
        {
            InitializeComponent();

            context = Globals.databaseContext;

            InitializeSelection();
        }

        public BookForm(Book modifyBook)
        {
            InitializeComponent();

            context = Globals.databaseContext;

            currentBook = modifyBook;
            InitializeSelection();
            FillFields(currentBook);

            Header.Text = $"Modify {modifyBook.Title}";
            validate.Content = "Save";

        }


        private void InitializeSelection()
        {

            typeList = new ObservableCollection<DataAccess.DataObjects.Type>(context.Types.Select(c => c));

            foreach(Location item in (context.Locations.Select(c => c)))
            {
                locationList.Add(new LocationView(item));
            }


            authorsData = context.Authors.Select(c => c.Name).ToArray();
            publishersData = context.Publishers.Select(c => c.Name).ToArray();
            locationsData = locationList.Select(c => c.viewText).ToArray();
            typesData = context.Types.Select(c => c.Name).ToArray();

            statusList = (ConfigurationManager.GetSection("Dictionaries/StatusList") as System.Collections.Hashtable)
                 .Cast<System.Collections.DictionaryEntry>()
                 .ToDictionary(n => Convert.ToInt32(n.Key), n => n.Value.ToString()).OrderBy(c => c.Key);

            tb_status.ItemsSource = statusList;
        }

        private void Click_AddModifyBook(object sender, RoutedEventArgs e)
        {

            if (!AssertData())
                return;

            else
            {
                //Add
                if (currentBook == null)
                {
                    currentBook = new Book();
                    context.Add(currentBook);
                    FillData();

                }
                //Update
                else
                {
                    currentBook = context.Books.Find(currentBook.BookId);
                    FillData();
                    context.Update(currentBook);
                }

                //Show a new page to add a new author/publisher/location/type
                if (FindAuthorId(tb_author.Text) == 0 || FindPublisherId(tb_publisher.Text) == 0 || FindLocationId(tb_location.Text) == 0 || FindTypeId(tb_type.Text) == 0)
                {
                    Page page = new BookFormExtra(currentBook, tb_author.Text, tb_publisher.Text, tb_location.Text, tb_type.Text);
                    Globals.mainWindow.ShowForm(page);
                }
                else
                {

                    context.SaveChanges();


                    Globals.mainWindow.CloseForm();
                }
            }
        }

        private bool AssertData()
        {
            bool isReady = true;

            string[] format = new string[] { "yyyy-MM-dd HH:mm:ss" };


            if (String.IsNullOrWhiteSpace(tb_location.Text))
            {
                tb_location_error.Visibility = Visibility.Visible;
                tb_location_error.Foreground = Brushes.Red;
                tb_location_error.Text = "Mandatory field";
                isReady = false;
            }
            else
            {
                tb_location_error.Visibility = Visibility.Hidden;
            }

            if (String.IsNullOrWhiteSpace(tb_location.Text))
            {
                tb_type_error.Visibility = Visibility.Visible;
                tb_type_error.Foreground = Brushes.Red;
                tb_type_error.Text = "Mandatory field";
                isReady = false;
            }
            else
            {
                tb_location_error.Visibility = Visibility.Hidden;
            }
            if (String.IsNullOrWhiteSpace(tb_title.Text))
            {
                tb_title_error.Visibility = Visibility.Visible;
                tb_title_error.Foreground = Brushes.Red;
                tb_title_error.Text = "Mandatory field";
                isReady = false;
            }
            else
            {
                tb_title_error.Visibility = Visibility.Hidden;
            }

            if (String.IsNullOrWhiteSpace(tb_author.Text))
            {
                tb_author_info.Visibility = Visibility.Visible;
                tb_author_info.Foreground = Brushes.Red;
                tb_author_info.Text = "Mandatory field";
                isReady = false;
            }
            else
            {
                tb_author_info.Visibility = Visibility.Hidden;
            }

            if (String.IsNullOrWhiteSpace(tb_publisher.Text))
            {
                tb_publisher_info.Visibility = Visibility.Visible;
                tb_publisher_info.Foreground = Brushes.Red;
                tb_publisher_info.Text = "Mandatory field";
                isReady = false;
            }
            else
            {
                tb_publisher_info.Visibility = Visibility.Hidden;
            }
            if (String.IsNullOrWhiteSpace(tb_status.Text))
            {
                tb_status_error.Visibility = Visibility.Visible;
                isReady = false;
            }
            else
            {
                tb_status_error.Visibility = Visibility.Hidden;
            }

            return isReady;


        }

        /// <summary>
        /// Insert View data to be sent to the database
        /// </summary>
        private void FillData()
        {
            currentBook.Title = tb_title.Text;
            currentBook.Description = tb_description.Text;
            currentBook.Date = tb_date.SelectedDate;
            currentBook.Author = context.Authors.Find(FindAuthorId(tb_author.Text));
            currentBook.Publisher = context.Publishers.Find(FindPublisherId(tb_publisher.Text));
            currentBook.Type = context.Types.Find(FindTypeId(tb_type.Text));
            currentBook.Location = context.Locations.Find(FindLocationId(tb_location.Text));
            currentBook.Isbn = tb_isbn.Text;
            currentBook.Language = tb_languages.Text;
            currentBook.Tags = tb_tags.Text;
            currentBook.Status = tb_status.SelectedIndex;
        }

        /// <summary>
        /// Fill fields with data
        /// </summary>
        /// <param name="book"></param>
        private void FillFields(Book book)
        {
            tb_title.Text = book.Title;
            tb_description.Text = book.Description;
            tb_date.SelectedDate = book.Date;

            if (context.Authors.Find(book.AuthorId) != null) { tb_author.Text = context.Authors.Find(book.AuthorId).Name; }
            if (context.Publishers.Find(book.PublisherId) != null) { tb_publisher.Text = context.Publishers.Find(book.PublisherId).Name; }
            if (context.Types.Find(book.TypeId) != null) { tb_type.Text = context.Types.Find(book.TypeId).Name; }
            if (context.Locations.Find(book.LocationId) != null) { tb_location.Text = locationList.Where(c => c.LocationId == book.LocationId).FirstOrDefault().viewText; }

            tb_isbn.Text = book.Isbn;
            tb_languages.Text = book.Language;
            tb_tags.Text = book.Tags;
            tb_status.SelectedIndex = book.Status;
        }

        private int FindAuthorId(string authorName)
        {
            return context.Authors.Where(author => author.Name.ToLower() == authorName.ToLower()).Select(a => a.AuthorId).FirstOrDefault();
        }

        private int FindPublisherId(string publisherName)
        {
            return context.Publishers.Where(publisher => publisher.Name.ToLower() == publisherName.ToLower()).Select(a => a.PublisherId).FirstOrDefault();
        }

        private int FindLocationId(string location)
        {
            return locationList.Where(c => c.viewText.ToLower() == location.ToLower()).Select(c => c.LocationId).FirstOrDefault();
        }
        private int FindTypeId(string type)
        {
            return context.Types.Where(x => x.Name.ToLower() == type.ToLower()).Select(a => a.TypeId).FirstOrDefault();
        }

        private void tb_author_Focus(object sender, KeyboardFocusChangedEventArgs e)
        {
            helpers.Searching search = new helpers.Searching();
            search.HookSearchBar(authorsData, tb_author, resultStack_author);
        }

        private void tb_publisher_Focus(object sender, KeyboardFocusChangedEventArgs e)
        {
            helpers.Searching search = new helpers.Searching();
            search.HookSearchBar(publishersData, tb_publisher, resultStack_publisher);
        }

        private void tb_location_Focus(object sender, KeyboardFocusChangedEventArgs e)
        {
            helpers.Searching search = new helpers.Searching();
            search.HookSearchBar(locationsData, tb_location, resultStack_location);
        }

        private void tb_type_Focus(object sender, KeyboardFocusChangedEventArgs e)
        {
            helpers.Searching search = new helpers.Searching();
            search.HookSearchBar(typesData, tb_type, resultStack_type);
        }

        private void tb_author_KeyUp(object sender, KeyEventArgs e)
        {
            helpers.Searching search = new helpers.Searching();
            search.HookSearchBar(authorsData, tb_author, resultStack_author);
        }
        private void tb_publisher_KeyUp(object sender, KeyEventArgs e)
        {
            helpers.Searching search = new helpers.Searching();
            search.HookSearchBar(publishersData, tb_publisher, resultStack_publisher);
        }
        private void tb_location_KeyUp(object sender, KeyEventArgs e)
        {
            helpers.Searching search = new helpers.Searching();
            search.HookSearchBar(locationsData, tb_location, resultStack_location);
        }
        private void tb_type_KeyUp(object sender, KeyEventArgs e)
        {
            helpers.Searching search = new helpers.Searching();
            search.HookSearchBar(typesData, tb_type, resultStack_type);
        }

        /// <summary>
        /// Used if you return to this page and you don't want your user to type all the fields again
        /// </summary>
        /// <param name="book"></param>
        /// <param name="author"></param>
        /// <param name="publisher"></param>
        /// <param name="location"></param>
        /// <param name="type"></param>
        public void ReturnPassData(Book book, string author, string publisher, string location, string type)
        {
            tb_author.Text = author;
            tb_publisher.Text = publisher;
            tb_location.Text = location;
            tb_type.Text = type;
            currentBook = book;
            FillFields(book);
        }
    }

    public class LocationView : Location
    {
        public string viewText { get; set; }

        public LocationView(Location location)
        {
            viewText = $"{location.RoomName}, {location.StreetName}, {location.City}";
            this.LocationId = location.LocationId;
        }

    }
}
