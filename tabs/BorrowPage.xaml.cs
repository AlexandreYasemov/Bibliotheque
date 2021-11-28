using Bibliotheque.DataAccess.DataObjects;
using Bibliotheque.forms;
using Bibliotheque.helpers;
using Bibliotheque.tabs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
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
    /// Interaction logic for BorrowPage.xaml
    /// </summary>
    public partial class BorrowPage : Page
    {
        PhpfribiblioContext context;
        Account currentUser;

        private ObservableCollection<BorrowViewBook> borrowList = new ObservableCollection<BorrowViewBook>();

        Dictionary<int, string> statusList;

        public BorrowPage()
        {
            InitializeComponent();

            context = Globals.databaseContext;

            statusList = (ConfigurationManager.GetSection("Dictionaries/StatusList") as System.Collections.Hashtable)
            .Cast<System.Collections.DictionaryEntry>()
            .ToDictionary(n => Convert.ToInt32(n.Key), n => n.Value.ToString());


            RefreshPage();
        }

        public void RefreshPage()
        {
            borrowList.Clear();

            currentUser = context.Accounts.Where(c => c.AccountId == Globals.currentUser.AccountId).First();

            List<Borrow> list = context.Borrows.Where(borrow => borrow.AccountId == currentUser.AccountId && borrow.EndDate == null).OrderBy(c => c.DueDate).ToList();
            foreach (Borrow item in list)
            {
                Book book = context.Books.Where(c => c.BookId == item.BookId).Include("Author").Include("Publisher").Include("Location").Include("Type").First();
                BorrowViewBook view = new BorrowViewBook(item,  book);

                borrowList.Add(view);
            }

            BookControl.ItemsSource = null;
            BookControl.ItemsSource = borrowList;
        }


        private void ReturnBook_Click(object sender, RoutedEventArgs e)
        {
            BorrowViewBook row = (BorrowViewBook)((Button)sender).DataContext;
            Borrow borrow = context.Borrows.Find(row.Borrow.BorrowId);

            if (MessageBox.Show($"Do you want to return {borrow.Book.Title}?", "Return", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                //do no stuff
                return;
            }

            borrow.EndDate = DateTime.Today;
            
            context.Borrows.Update(borrow);

            context.SaveChanges();

            borrowList.Remove(row);
        }

        private void Due_RowInitialized(object sender, EventArgs e)
        {
            BorrowViewBook row = (BorrowViewBook)((TextBlock)sender).DataContext;
            if (row.Borrow.EndDate == null)
                if (row.isLate)
                    ((TextBlock)sender).Text = "Late";
                else
                ((TextBlock)sender).Text += row.Borrow.DueDate.ToShortDateString();
        }

        private void Start_RowInitialized(object sender, EventArgs e)
        {
            BorrowViewBook row = (BorrowViewBook)((TextBlock)sender).DataContext;
            if (row.Borrow.EndDate == null)
                ((TextBlock)sender).Text += row.Borrow.StartDate.ToShortDateString();
        }

        private void Author_Click(object sender, RoutedEventArgs e)
        {
            SearchViewBook row = (SearchViewBook)((Hyperlink)sender).DataContext;
            DetailsPopup detailsPopup = new DetailsPopup(row.Book.Author);
            Globals.mainWindow.ShowForm(detailsPopup);
        }

        private void Publisher_Click(object sender, RoutedEventArgs e)
        {
            SearchViewBook row = (SearchViewBook)((Hyperlink)sender).DataContext;
            DetailsPopup detailsPopup = new DetailsPopup(row.Book.Publisher);
            Globals.mainWindow.ShowForm(detailsPopup);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            ((Expander)sender).BringIntoView();
        }
        private void History_Click(object sender, RoutedEventArgs e)
        {
            Globals.mainWindow.AddTemporaryTab(new BorrowSearchPage(currentUser), new BitmapImage(new Uri("pack://application:,,,/images/default_book.png")), "History");
        }
        private void BookStatus_Initialized(object sender, EventArgs e)
        {

            TextBlock tb = (TextBlock)sender;
            BorrowViewBook svb = (BorrowViewBook)tb.DataContext;
            string buffer;
            statusList.TryGetValue(svb.Book.Status, out buffer);
            tb.Text = buffer;

            switch (svb.Book.Status)
            {
                case 0:
                case 1:
                case 2:
                    tb.Foreground = Brushes.Green;
                    break;
                case 3:
                    tb.Foreground = Brushes.Orange;
                    break;
                case 4:
                    tb.Foreground = Brushes.Red;
                    break;
            }

        }

    }

    public class BorrowViewBook : Book
    {
        public Borrow Borrow { get; set; }
        public Book Book { get; set; }

        public bool isLate { get; set; }


        public BorrowViewBook(Borrow borrow, Book book)
        {
            this.Borrow = borrow;
            this.Book = book;

            if (Borrow.DueDate <= DateTime.Today && Borrow.EndDate == null)
                isLate = true;
        }
    }
}
