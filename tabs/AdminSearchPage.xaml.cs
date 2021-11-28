using Bibliotheque.DataAccess.DataObjects;
using Bibliotheque.forms;
using Bibliotheque.helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for AdminSearchPage.xaml
    /// </summary>


    public enum BibliotequeDataType
    {
        Author = 0,
        Publisher = 1,
        Type = 2,
        Location = 3,
        User = 4,
        Borrow = 5,
    }


    public partial class AdminSearchPage : Page
    {
        private PhpfribiblioContext context;

        public BibliotequeDataType type;
        private ObservableCollection<OmniData> EntryView = new ObservableCollection<OmniData>();
        public List<OmniData> entriesList;
        public List<OmniData> entriesInDatabase;

        private int currentPage = 1;
        private int maxPages;
        private int entriesPerPage = 50;

        private Searching searching = new Searching();

        public AdminSearchPage(BibliotequeDataType type)
        {
            context = Globals.databaseContext;

            InitializeComponent();

            this.type = type;

            searching.TermsChanged += SearchTerms_Changed;

            entriesInDatabase = new List<OmniData>();
            entriesList = entriesInDatabase;

            switch (type)
            {
                case BibliotequeDataType.Author:
                    foreach (Author item in context.Authors.ToList())
                    {
                        entriesInDatabase.Add(new OmniData(item.AuthorId, item.Name, item.Description));
                    }
                    break;

                case BibliotequeDataType.Publisher:
                    foreach (Publisher item in context.Publishers.ToList())
                    {
                        entriesInDatabase.Add(new OmniData(item.PublisherId, item.Name, item.Description));
                    }
                    break;

                case BibliotequeDataType.Type:
                    foreach (DataAccess.DataObjects.Type item in context.Types.ToList())
                    {
                        entriesInDatabase.Add(new OmniData(item.TypeId, item.Name, item.Description));
                    }
                    break;

                case BibliotequeDataType.Location:
                    foreach (Location item in context.Locations.ToList())
                    {
                        entriesInDatabase.Add(new OmniData(item.LocationId, item.City, $"{item.StreetName} {item.RoomName}"));
                    }
                    break;

                case BibliotequeDataType.User:
                    foreach (Account item in context.Accounts.ToList())
                    {
                        entriesInDatabase.Add(new OmniData(item.AccountId, item.Name, ""));
                    }
                    
                    break;
            }
            entriesInDatabase.Reverse();

            RefreshPage();
        }

        private void KeyDown_Search(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;

            Search();
        }
        private void Search()
        {


            currentPage = 1;
            entriesList = entriesInDatabase;

            string searchTerm = SearchBar.Text;
            SearchBar.Text = null;

            if (!String.IsNullOrWhiteSpace(searchTerm))
                searching.AddSearchTerm(searchTerm, sp_TermsArea);


        }
        private void SearchTerms_Changed(object sender, EventArgs e)
        {
            entriesList = searching.SearchThroughAllTerms(entriesInDatabase.Cast<object>().ToList()).Cast<OmniData>().ToList();
            RefreshPage();
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

        private void ModifyEntry_Click(object sender, RoutedEventArgs e)
        {

            OmniData data = (OmniData)((Button)sender).DataContext;

            switch (type)
            {
                case BibliotequeDataType.Author:
                    Globals.mainWindow.ShowForm(new VariousForm(context.Authors.Find(data.id)));
                    break;

                case BibliotequeDataType.Publisher:
                    Globals.mainWindow.ShowForm(new VariousForm(context.Publishers.Find(data.id)));
                    break;

                case BibliotequeDataType.Type:
                    Globals.mainWindow.ShowForm(new VariousForm(context.Types.Find(data.id)));
                    break;

                case BibliotequeDataType.Location:
                    Globals.mainWindow.ShowForm(new LocationForm(context.Locations.Find(data.id)));
                    break;

                case BibliotequeDataType.User:
                    Globals.mainWindow.ShowForm(new AccountForm(context.Accounts.Find(data.id)));
                    break;
            }
        }

        private void DeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            OmniData data = (OmniData)((Button)sender).DataContext;


            switch (type)
            {
                case BibliotequeDataType.Author:
                    if (context.Books.Where(c => c.AuthorId == data.id).Count() == 0)
                    {
                        if (MessageBox.Show($"Supprimer {data.text}?", "Supprimer", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            //do no stuff
                            return;
                        }
                        context.Remove(context.Authors.Find(data.id));
                    }
                    else
                    {
                        MessageBox.Show($"Impossible de supprimer {data.text} car il est utilisé dans multiples livres.");
                        return;
                    }
                    break;

                case BibliotequeDataType.Publisher:
                    if (context.Books.Where(c => c.PublisherId == data.id).Count() == 0)
                    {
                        if (MessageBox.Show($"Supprimer {data.text}?", "Supprimer", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            //do no stuff
                            return;
                        }
                        context.Remove(context.Publishers.Find(data.id));
                    }
                    else
                    {
                        MessageBox.Show($"Impossible de supprimer {data.text} car il est utilisé dans multiples livres.");
                        return;
                    }
                    break;

                case BibliotequeDataType.Type:
                    if (context.Books.Where(c => c.TypeId == data.id).Count() == 0)
                    {
                        if (MessageBox.Show($"Supprimer {data.text}?", "Supprimer", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            //do no stuff
                            return;
                        }
                        context.Remove(context.Types.Find(data.id));
                    }
                    else
                    {
                        MessageBox.Show($"Impossible de supprimer {data.text} car il est utilisé dans multiples livres.");
                        return;
                    }
                    break;

                case BibliotequeDataType.Location:
                    if (context.Books.Where(c => c.LocationId == data.id).Count() == 0)
                    {
                        if (MessageBox.Show($"Supprimer {data.text}?", "Supprimer", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            //do no stuff
                            return;
                        }
                        context.Remove(context.Locations.Find(data.id));
                    }
                    else
                    {
                        MessageBox.Show($"Impossible de supprimer {data.text} car il est utilisé dans multiples livres.");
                        return;
                    }
                    break;

                case BibliotequeDataType.User:
                    if (context.Borrows.Where(c => c.AccountId == data.id).Count() == 0)
                    {
                        if (MessageBox.Show($"Supprimer {data.text}?", "Supprimer", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            //do no stuff
                            return;
                        }
                        context.Remove(context.Accounts.Find(data.id));
                    }
                    else
                    {
                        MessageBox.Show($"Impossible de supprimer {data.text} car plusieurs emprunts sont à leur nom.");
                        return;
                    }
                    break;
            }

            
            context.SaveChanges();

            EntryView.Remove(data);
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

        private void Booking_Click(object sender, RoutedEventArgs e)
        {
            OmniData account = (OmniData)((Button)sender).DataContext;
            Globals.mainWindow.AddTemporaryTab(new BorrowSearchPage(context.Accounts.Find(account.id)), new BitmapImage(new Uri("pack://application:,,,/images/default_book.png")), "Gestion");
        }

        private void Button_Initialized(object sender, EventArgs e)
        {
            if (type == BibliotequeDataType.User)
                ((Button)sender).Visibility = Visibility.Visible;
        }

        private void PreviewKeyDown_Search(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Back)
                return;

            searching.RemoveLastTerm();

            Search();
        }

    }

    public class OmniData
    {

        public int id;
        public string text { get; set; }
        public string description { get; set; }

        //object attachedObject;

        public OmniData(int id, string text, string description)
        {
            this.text = text;
            this.description = description;
            this.id = id;


        }
    }
}
