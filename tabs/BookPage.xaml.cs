using Bibliotheque.DataAccess.DataObjects;
using System.Linq;
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
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Bibliotheque.forms;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Configuration;
using Bibliotheque.helpers;
using Bibliotheque.tabs;

namespace Bibliotheque
{
    /// <summary>
    /// Interaction logic for BookPage.xaml
    /// </summary>
    public partial class BookPage : Page
    {
        private ObservableCollection<SearchViewBook> bookView = new ObservableCollection<SearchViewBook>();
        private PhpfribiblioContext context;

        private List<Book> bookList;
        private List<Book> booksInDatabase;

        private int currentPage = 1;
        private int maxPages;
        private int entriesPerPage = 50;

        private Searching searching = new Searching();

        private System.Reflection.PropertyInfo[] filterList = typeof(Book).GetProperties();

        Dictionary<int, string> statusList;

        public BookPage()
        {
            context = Globals.databaseContext;

            statusList = (ConfigurationManager.GetSection("Dictionaries/StatusList") as System.Collections.Hashtable)
            .Cast<System.Collections.DictionaryEntry>()
            .ToDictionary(n => Convert.ToInt32(n.Key), n => n.Value.ToString());

            booksInDatabase = context.Books.Include("Author").Include("Publisher").Include("Location").Include("Type").Include("Borrows").ToList();
            booksInDatabase.Reverse();
            bookList = booksInDatabase.Where(c => !c.IsArchived).ToList();

            InitializeComponent();

            searching.TermsChanged += SearchTerms_Changed;


            if (Globals.currentUser.IsAdmin)
            {
                AddBook.IsEnabled = true;
                AddBook.Visibility = Visibility.Visible;
                cb_archived.IsEnabled = true;
                cb_archived.Visibility = Visibility.Visible;
            }

            RefreshPage();
        }

        public void RefreshPage()
        {
            bookView.Clear();
            maxPages = bookList.Count / entriesPerPage;
            if ((bookList.Count % entriesPerPage) > 0)
                maxPages++;

            RefreshPaging();

            for (int i = entriesPerPage * (currentPage - 1); i < entriesPerPage * currentPage; i++)
            {
                if (i > bookList.Count - 1)
                    break;

                Borrow borrow;
                SearchViewBook bookEntry = new SearchViewBook(bookList[i], "Available", Brushes.Green);
                //Is it reserved? If yes, when will it be free
                borrow = bookList[i].Borrows.Where(c => c.StartDate <= DateTime.Today && c.EndDate == null).FirstOrDefault();
                if (borrow != null)
                {
                    //check first date available
                    foreach (Borrow borrowBuffer in bookList[i].Borrows)
                    {
                        if (borrowBuffer.StartDate == borrow.DueDate.AddDays(1) && borrowBuffer.EndDate == null)
                            borrow = borrowBuffer;
                    }

                    
                    if (borrow.DueDate > DateTime.Today.AddDays(1) && borrow.EndDate == null)
                        bookEntry = new SearchViewBook(bookList[i], "Borrowed until ", borrow.DueDate.AddDays(1), Brushes.Red);
                    else //Book not returned
                        bookEntry = new SearchViewBook(bookList[i], "Book missing", Brushes.Red);
                }
                else
                {
                    //Will it be reserved in the future? If yes find the closest date
                    borrow = bookList[i].Borrows.Where(c => c.StartDate > DateTime.Today && c.EndDate == null).OrderBy(c => c.StartDate).FirstOrDefault();
                    if (borrow != null)
                    {
                        bookEntry = new SearchViewBook(bookList[i], "Reserved from ", borrow.StartDate, Brushes.Orange);
                    }
                }


                bookView.Add(bookEntry);
            }

            BookControl.ItemsSource = null;
            BookControl.ItemsSource = bookView;
        }

        private void RefreshPaging()
        {
            tb_MaxPages.Text = maxPages.ToString();
            tb_CurrentPage.Text = currentPage.ToString();
            if (maxPages <= 1)
            {
                pagingArea.Height = new GridLength(0.0);
            }
            else
            {
                pagingArea.Height = new GridLength(64.0);
            }
        }

        private void PreviewKeyDown_Search(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                if (String.IsNullOrWhiteSpace(SearchBar.Text))
                    searching.RemoveLastTerm();

                currentPage = 1;
                //Search Engine

                bookList = searching.SearchThroughAllTerms(booksInDatabase.Cast<object>().ToList()).Cast<Book>().ToList();

                if ((bool)cb_archived.IsChecked)
                {
                    bookList = bookList.Where(c => c.IsArchived).ToList();
                }
                else
                {
                    bookList = bookList.Where(c => !c.IsArchived).ToList();
                }

                RefreshPage();
            }
        }

        private void KeyDown_Search(object sender, KeyEventArgs e)
        {

            if (e.Key != Key.Return)
               return;

            string searchTerm = SearchBar.Text;
            SearchBar.Text = null;

            if (!String.IsNullOrWhiteSpace(searchTerm))
            {
                searching.AddSearchTerm(searchTerm, sp_TermsArea);
            }

            currentPage = 1;
            //Search Engine

            bookList = searching.SearchThroughAllTerms(booksInDatabase.Cast<object>().ToList()).Cast<Book>().ToList();
            RefreshPage();
        }

        private void SearchTerms_Changed(object sender, EventArgs e)
        {
            bookList = searching.SearchThroughAllTerms(booksInDatabase.Cast<object>().ToList()).Cast<Book>().ToList();
            RefreshPage();
        }
        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            FilterForm page = new FilterForm(filterList);
            Globals.mainWindow.ShowForm(page);
            page.FilterAdded += AddFilter;
        }

        public void AddFilter(object sender, EventArgs e)
        {
            searching.AddSearchTerm(((FilterForm)sender).GetText(), ((FilterForm)sender).GetFilter(), sp_TermsArea);
            Globals.mainWindow.CloseForm();

            bookList = searching.SearchThroughAllTerms(booksInDatabase.Cast<object>().ToList()).Cast<Book>().ToList();
            RefreshPage();
        }

        private void Add_Book(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            BookForm bookForm = new BookForm();
            mainWindow.ShowForm(bookForm);
        }

        private void ModifyBook_Click(object sender, RoutedEventArgs e)
        {
            SearchViewBook view = (SearchViewBook)((Button)sender).DataContext;

            BookForm bookForm = new BookForm(view.Book);
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.ShowForm(bookForm);
        }

        private void BorrowBook_Click(object sender, RoutedEventArgs e)
        {
            SearchViewBook view = (SearchViewBook)((Button)sender).DataContext;

            BorrowForm borrowForm = new BorrowForm(view.Book);
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.ShowForm(borrowForm);
        }

        private void ManageBorrows_Click(object sender, RoutedEventArgs e)
        {
            SearchViewBook row = (SearchViewBook)((Button)sender).DataContext;
            BorrowSearchPage borrowSearch = new BorrowSearchPage(row.Book);
            Globals.mainWindow.AddTemporaryTab(borrowSearch, new BitmapImage(new Uri("pack://application:,,,/images/default_book.png")), "Gestion");
        }

        private void Textblock_RowInitialized(object sender, EventArgs e)
        {
            SearchViewBook row = (SearchViewBook)((TextBlock)sender).DataContext;
            string date = "";
            if (row.InfoDate != null) date = ((DateTime)row.InfoDate).ToShortDateString();
               ((TextBlock)sender).Text = $"{row.InfoText}{date}";

            ((TextBlock)sender).Foreground = row.InfoColor;

        }

        private void ModifyBook_Initialized(object sender, EventArgs e)
        {
            if (Globals.currentUser.IsAdmin == true)
            {
                ((Button)sender).Visibility = Visibility.Visible;
                ((Button)sender).IsEnabled = true;
            }
        }

        private void ArchiveBook_Initialized(object sender, EventArgs e)
        {
            SearchViewBook row = (SearchViewBook)((Button)sender).DataContext;
            if (Globals.currentUser.IsAdmin == true && !row.Book.IsArchived)
            {
                ((Button)sender).Visibility = Visibility.Visible;
                ((Button)sender).IsEnabled = true;
            }

        }

        private void RestoreBook_Initialized(object sender, EventArgs e)
        {
            SearchViewBook row = (SearchViewBook)((Button)sender).DataContext;
            if (Globals.currentUser.IsAdmin == true && row.Book.IsArchived)
            {
                ((Button)sender).Visibility = Visibility.Visible;
                ((Button)sender).IsEnabled = true;
            }
        }

        private void BookStatus_Initialized(object sender, EventArgs e)
        {

            TextBlock tb = (TextBlock)sender;
            SearchViewBook svb = (SearchViewBook)tb.DataContext;
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

        private bool InvarientCultureCompare(string s1, string s2)
        {
            return RemoveDiacritics(s1).Contains(RemoveDiacritics(s2), StringComparison.InvariantCultureIgnoreCase);
        }

        private string RemoveDiacritics(string text)
        {
            string formD = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            
            foreach (char ch in formD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            currentPage = 1;
            tb_CurrentPage.Text = "1";
            RefreshPage();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                tb_CurrentPage.Text = currentPage.ToString();
                RefreshPage();
            }
        }

        private void CurrentPage_KeyEnter(object sender, KeyEventArgs e)
        {
            if (!String.IsNullOrEmpty(tb_CurrentPage.Text) && e.Key == Key.Return)
            {
                currentPage = Convert.ToInt32(tb_CurrentPage.Text);
                RefreshPage();
            }
        }

        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {

            if (currentPage < maxPages)
            {
                currentPage++;
                tb_CurrentPage.Text = currentPage.ToString();
                RefreshPage();
            }
            
        }

        private void End_Click(object sender, RoutedEventArgs e)
        {
            currentPage = maxPages;
            tb_CurrentPage.Text = maxPages.ToString();
            RefreshPage();
        }

        private void CurrentPage_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
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

        private void ArchiveBook_Click(object sender, RoutedEventArgs e)
        {
            SearchViewBook row = (SearchViewBook)((Button)sender).DataContext;
            if (MessageBox.Show($"Archive {row.Book.Title}?", "Archive", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                //do no stuff
                return;
            }

            row.Book.IsArchived = true;

            context.Update(row.Book);
            context.SaveChanges();
            bookView.Remove(row);
                
            }

        private void RestoreBook_Click(object sender, RoutedEventArgs e)
        {
            SearchViewBook row = (SearchViewBook)((Button)sender).DataContext;
            if (MessageBox.Show($"Restaure {row.Book.Title}?", "Restaure", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                //do no stuff
                return;
            }

            row.Book.IsArchived = false;

            context.Update(row.Book);
            context.SaveChanges();
            bookView.Remove(row);

        }

        private void Checked_Archived(object sender, RoutedEventArgs e)
        {
            currentPage = 1;
            //Search Engine

            bookList = searching.SearchThroughAllTerms(booksInDatabase.Cast<object>().ToList()).Cast<Book>().ToList();

            if ((bool)cb_archived.IsChecked)
            {
                bookList = bookList.Where(c => c.IsArchived).ToList();
            }
            else
            {
                bookList = bookList.Where(c => !c.IsArchived).ToList();
            }

            RefreshPage();
        }
    }


    public class SearchViewBook
    {
        public DateTime? InfoDate;
        public string InfoText;
        public Brush InfoColor;

        public Book Book { get; set;}

        public SearchViewBook(Book book, string infoText, DateTime infoDate, Brush infoColor)
        {
            this.Book = book;
            InfoText = infoText;
            InfoDate = infoDate;
            InfoColor = infoColor;
        }

        public SearchViewBook(Book book, string infoText, Brush infoColor)
        {
            this.Book = book;
            InfoText = infoText;
            InfoColor = infoColor;
        }
    }
}
