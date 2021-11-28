using Bibliotheque.DataAccess.DataObjects;
using Bibliotheque.forms;
using Bibliotheque.helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Bibliotheque.tabs
{
    /// <summary>
    /// Interaction logic for BorrowSearchPage.xaml
    /// </summary>
    public partial class BorrowSearchPage : Page
    {

        private PhpfribiblioContext context;

        public BibliotequeDataType type;
        private ObservableCollection<Borrow> EntryView = new ObservableCollection<Borrow>();
        public List<Borrow> entriesList = new List<Borrow>();
        public List<Borrow> entriesInDatabase = new List<Borrow>();

        private int currentPage = 1;
        private int maxPages;
        private int entriesPerPage = 50;

        private System.Reflection.PropertyInfo[] filterList = typeof(Borrow).GetProperties();
        private Searching searching = new Searching();

        public BorrowSearchPage()
        {
            context = Globals.databaseContext;

            InitializeComponent();

            searching.TermsChanged += SearchTerms_Changed;

            this.type = BibliotequeDataType.Borrow;

            foreach (Borrow borrow in context.Borrows.Include("Account").Include("Book").ToList())
            {
                entriesInDatabase.Add(borrow);
            }

            Search();
        }

        /// <summary>
        /// List of all the reservations for a book
        /// </summary>
        /// <param name="book"></param>
        public BorrowSearchPage(Book book)
        {
            context = Globals.databaseContext;

            InitializeComponent();

            this.type = BibliotequeDataType.Borrow;

            foreach (Borrow borrow in context.Borrows.Include("Account").ToList())
            {
                entriesInDatabase.Add(borrow);
            }

            searching.AddSearchTerm(book.BookId.ToString(), "[BookId]", sp_TermsArea);

            Search();
        }

        public BorrowSearchPage(Account account)
        {
            context = Globals.databaseContext;

            InitializeComponent();

            this.type = BibliotequeDataType.Borrow;

            if (account.IsAdmin)
            {
                foreach (Borrow borrow in context.Borrows.Include("Book").ToList())
                {
                    entriesInDatabase.Add(borrow);
                }
            }
            else
            {
                cb_history.IsChecked = true;
                foreach (Borrow borrow in context.Borrows.Where(c => c.Account == account).Include("Book").ToList())
                {
                    entriesInDatabase.Add(borrow);
                }
            }

            searching.AddSearchTerm(account.AccountId.ToString(), "[AccountId]", sp_TermsArea);

            Search();
        }

        public void RefreshPage()
        {
            EntryView.Clear();
            maxPages = entriesList.Count / entriesPerPage;
            if ((entriesList.Count % entriesPerPage) > 0)
                maxPages++;
            for (int i = entriesPerPage * (currentPage - 1); i < entriesPerPage * currentPage; i++)
            {
                if (i > entriesList.Count - 1)
                    break;

                EntryView.Add(entriesList[i]);
            }

            RefreshPaging();

            ListControl.ItemsSource = null;
            ListControl.ItemsSource = EntryView;
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

        private void CurrentPage_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void ModifyEntry_Click(object sender, RoutedEventArgs e)
        {
            Borrow borrow = (Borrow)((Button)sender).DataContext;

            Globals.mainWindow.ShowForm(new BorrowForm(context.Borrows.Find(borrow.BorrowId)));

        }

        private void DeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            Borrow borrow = (Borrow)((Button)sender).DataContext;

            if (MessageBox.Show($"Supprimer l'emprunt de {borrow.Account.Name} du {borrow.StartDate.ToShortDateString()} au {borrow.DueDate.ToShortDateString()}?", "Supprimer", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                //do no stuff
                return;
            }
            else
            {
                context.Remove(context.Borrows.Find(borrow.BorrowId));
                EntryView.Remove(borrow);
            }

            context.SaveChanges();
        }

        private void PreviewKeyDown_Search(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                if (String.IsNullOrWhiteSpace(SearchBar.Text))
                    searching.RemoveLastTerm();

                currentPage = 1;
                //Search Engine

                entriesList = searching.SearchThroughAllTerms(entriesInDatabase.Cast<object>().ToList()).Cast<Borrow>().ToList();
                RefreshPage();
            }
        }

        private void KeyDown_Search(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;

            Search();
        }

        private void SearchTerms_Changed(object sender, EventArgs e)
        {
            entriesList = searching.SearchThroughAllTerms(entriesInDatabase.Cast<object>().ToList()).Cast<Borrow>().ToList();
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

            entriesList = searching.SearchThroughAllTerms(entriesInDatabase.Cast<object>().ToList()).Cast<Borrow>().ToList();
            RefreshPage();
        }

        private void Search()
        {
            currentPage = 1;
            entriesList = entriesInDatabase;

            DateTime? startDate = dp_start.SelectedDate;
            if (startDate == null) { startDate = DateTime.MinValue; }
            DateTime? dueDate = dp_end.SelectedDate;
            if (dueDate == null) { dueDate = DateTime.MaxValue; }

            string searchTerm = SearchBar.Text;
            SearchBar.Text = null;

            if (!String.IsNullOrWhiteSpace(searchTerm))
            {
                searching.AddSearchTerm(searchTerm, sp_TermsArea);
            }

            currentPage = 1;

            //Search Engine

            entriesList = searching.SearchThroughAllTerms(entriesInDatabase.Cast<object>().ToList()).Cast<Borrow>().ToList();

            entriesList = entriesList.Where(c => c.StartDate >= startDate || c.DueDate <= dueDate).ToList();

            if ((bool)cb_late.IsChecked)
                entriesList = entriesList.Where(c => c.DueDate < c.EndDate || (c.DueDate <= DateTime.Today && c.EndDate == null)).ToList();

            if (!(bool)cb_history.IsChecked)
                entriesList = entriesList.Where(c => c.EndDate == null).ToList();

            RefreshPage();
        }


        private void Value_Changed(object sender, SelectionChangedEventArgs e)
        {
            Search();
        }

        private void Checked_History(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Late_Initialized(object sender, EventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            Borrow borrow = (Borrow)textBlock.DataContext;

            if (borrow.DueDate < DateTime.Today && borrow.EndDate == null)
                textBlock.Visibility = Visibility.Visible;
        }

        private void Checked_Late(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Return_Initialized(object sender, EventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            Borrow borrow = (Borrow)textBlock.DataContext;

            if (borrow.EndDate != null)
                textBlock.Visibility = Visibility.Visible;
        }
    }
}
