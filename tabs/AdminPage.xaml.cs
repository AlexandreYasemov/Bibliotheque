using Bibliotheque.forms;
using Bibliotheque.helpers;
using Bibliotheque.tabs;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bibliotheque
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private MainWindow mainWindow;
        public AdminPage()
        {
            mainWindow = Globals.mainWindow;
            InitializeComponent();
 
        }

        private void Add_Book(object sender, RoutedEventArgs e)
        {
            BookForm bookForm = new BookForm();
            mainWindow.ShowForm(bookForm);
        }

        private void Add_Location(object sender, RoutedEventArgs e)
        {
            LocationForm locationForm = new LocationForm();
            mainWindow.ShowForm(locationForm);
        }
        private void Add_Account(object sender, RoutedEventArgs e)
        {
            AccountForm accountForm = new AccountForm();
            mainWindow.ShowForm(accountForm);
        }

        private void Add_Author(object sender, RoutedEventArgs e)
        {
            VariousForm authorForm = new VariousForm(BibliotequeVariousForm.Author);
            mainWindow.ShowForm(authorForm);
        }
        private void Add_Publisher(object sender, RoutedEventArgs e)
        {
            VariousForm publisherForm = new VariousForm(BibliotequeVariousForm.Publisher);
            mainWindow.ShowForm(publisherForm);
        }
        private void Add_Type(object sender, RoutedEventArgs e)
        {
            VariousForm typeForm = new VariousForm(BibliotequeVariousForm.Type);
            mainWindow.ShowForm(typeForm);
        }

        private void Modify_Author(object sender, RoutedEventArgs e)
        {
            AdminSearchPage searchForm = new AdminSearchPage(BibliotequeDataType.Author);
            mainWindow.AddTemporaryTab(searchForm, new BitmapImage(new Uri("pack://application:,,,/icons/author.png")), "Authors");
        }

        private void Modify_Publisher(object sender, RoutedEventArgs e)
        {
            AdminSearchPage searchForm = new AdminSearchPage(BibliotequeDataType.Publisher);
            mainWindow.AddTemporaryTab(searchForm, new BitmapImage(new Uri("pack://application:,,,/icons/publisher.png")), "Publishers");
        }

        private void Modify_Location(object sender, RoutedEventArgs e)
        {
            AdminSearchPage searchForm = new AdminSearchPage(BibliotequeDataType.Location);
            mainWindow.AddTemporaryTab(searchForm, new BitmapImage(new Uri("pack://application:,,,/icons/location.png")), "Locations");
        }

        private void Modify_User(object sender, RoutedEventArgs e)
        {
            AdminSearchPage searchForm = new AdminSearchPage(BibliotequeDataType.User);
            mainWindow.AddTemporaryTab(searchForm, new BitmapImage(new Uri("pack://application:,,,/icons/user.png")), "Users");
        }

        private void Modify_Type(object sender, RoutedEventArgs e)
        {
            AdminSearchPage searchForm = new AdminSearchPage(BibliotequeDataType.Type);
            mainWindow.AddTemporaryTab(searchForm, new BitmapImage(new Uri("pack://application:,,,/icons/type.png")), "Genres");
        }

        private void Modify_Book(object sender, RoutedEventArgs e)
        {
            mainWindow.LeftTabMenu.SelectedIndex= 0;
        }

        private void Modify_Borrows(object sender, RoutedEventArgs e)
        {
            BorrowSearchPage borrowSearch = new BorrowSearchPage();
            Globals.mainWindow.AddTemporaryTab(borrowSearch, new BitmapImage(new Uri("pack://application:,,,/images/default_book.png")), "Manage");
        }

    }
}
