using Bibliotheque.DataAccess.DataObjects;
using Bibliotheque.tabs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Bibliotheque.helpers
{
    class Searching
    {
        public List<Button> searchTerms = new List<Button>();

        /// <summary>
        /// Event sent every time a term is added/removed
        /// </summary>
        public event EventHandler TermsChanged;

        /// <summary>
        /// Adds a term to search and adds an interactable rectangle in the SearchBar
        /// </summary>
        /// <param name="term"></param>
        /// <param name="stackPanel"></param>
        /// <returns></returns>
        public void AddSearchTerm(string term, StackPanel stackPanel)
        {

            Button button = new Button();

            button.Style = (Style)Application.Current.Resources["ButtonSearchTerm"];
            button.Content = term;
            button.Click += SearchTerm_Click;

            searchTerms.Add(button);
            stackPanel.Children.Add(button);

            if (TermsChanged != null)
            {
                TermsChanged(this, EventArgs.Empty);
            }

        }

        /// <summary>
        /// Adds a term to search for this specific field and adds an interactable rectangle in the SearchBar
        /// </summary>
        /// <param name="term"></param
        /// <param name="filter"></param>
        /// <param name="stackPanel"></param>
        /// <returns></returns>
        public void AddSearchTerm(string term, string filter, StackPanel stackPanel)
        {

            Button button = new Button();

            button.Style = (Style)Application.Current.Resources["ButtonSearchTerm"];
            button.Content = term;
            button.Tag = filter;
            button.Click += SearchTerm_Click;

            searchTerms.Add(button);
            stackPanel.Children.Add(button);

            if (TermsChanged != null)
            {
                TermsChanged(this, EventArgs.Empty);
            }

        }

        private void SearchTerm_Click(object sender, RoutedEventArgs e)
        {
            RemoveSearchTerm((Button)sender);
        }

        /// <summary>
        /// Removes a term
        /// </summary>
        /// <param name="button"></param>
        public void RemoveSearchTerm(Button button)
        {
            StackPanel sp = (StackPanel)button.Parent;
            sp.Children.Remove(button);
            searchTerms.Remove(button);

            if (TermsChanged != null)
            {
                TermsChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Removes the last term added
        /// </summary>
        public void RemoveLastTerm()
        {
            if (searchTerms.Count > 0)
            {
                Button last = searchTerms[searchTerms.Count - 1];
                StackPanel sp = (StackPanel)last.Parent;
                sp.Children.Remove(last);
                searchTerms.Remove(last);

                if (TermsChanged != null)
                {
                    TermsChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Goes through all the terms and returns the matching results
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<object> SearchThroughAllTerms(List<object> data)
        {
            if (data.Count == 0)
                return data;

            foreach (Button button in searchTerms)
            {
                string searchTerm = button.Content.ToString();
                string filter = null;
                if (button.Tag != null)
                {
                    filter = button.Tag.ToString();
                    filter = filter.Remove(filter.Length-1);
                    filter = filter.Remove(0,1);
                }


                //var type = bookList.GetType().GetGenericArguments()[0];
                var properties = data[0].GetType().GetProperties();
                data = data.Where(x => properties
                            .Any(p =>
                            {

                                if (p.Name != filter && filter != null) { return false; }

                                var value = p.GetValue(x);
                                if (value == null) { return false; }
                                //1 extra layer of properties inside classes
                                var extraProperties = value.GetType().GetProperties();
                                foreach (var property in extraProperties)
                                {
                                    value = property.GetValue(value);
                                    switch (x.GetType().Name)
                                    {
                                        case "Book":
                                            if ((p.PropertyType != typeof(string) || p.PropertyType.Name == "Language") && (p.Name != filter && filter != null)) { return false; }
                                            return value != null && InvarientCultureCompare(value.ToString(), searchTerm) || CompareAgainstRelatedTables((Book)x, searchTerm);
                                        case "Borrow":
                                            return value != null && InvarientCultureCompare(value.ToString(), searchTerm) || CompareAgainstRelatedTables((Borrow)x, searchTerm);
                                        case "OmniData":
                                            return value != null && InvarientCultureCompare(value.ToString(), searchTerm) || CompareAgainstRelatedTables((OmniData)x, searchTerm);
                                        default:
                                            return value != null && InvarientCultureCompare(value.ToString(), searchTerm);
                                    }
                                }
                                switch (x.GetType().Name)
                                {
                                    case "Book":
                                        if ((p.PropertyType != typeof(string) || p.PropertyType.Name == "Language") && (p.Name != filter && filter != null)) { return false; }
                                        return value != null && InvarientCultureCompare(value.ToString(), searchTerm) || CompareAgainstRelatedTables((Book)x, searchTerm);
                                    case "Borrow":
                                        return value != null && InvarientCultureCompare(value.ToString(), searchTerm) || CompareAgainstRelatedTables((Borrow)x, searchTerm);
                                    case "OmniData":
                                        return value != null && InvarientCultureCompare(value.ToString(), searchTerm) || CompareAgainstRelatedTables((OmniData)x, searchTerm);
                                    default:
                                        return value != null && InvarientCultureCompare(value.ToString(), searchTerm);
                                }

                            }
                            )).ToList();
                //  bookList = ObservableCollection < SearchViewBook >
            }

            return data;
        }

        private bool CompareAgainstRelatedTables(Book book, string term)
        {
            if (book.Author != null) { if (InvarientCultureCompare(book.Author.Name, term)) return true; }
            if (book.Publisher != null) { if (InvarientCultureCompare(book.Publisher.Name, term)) return true; }
            if (book.Author != null) { if (InvarientCultureCompare(book.Type.Name, term)) return true; }

            return false;
        }

        private bool CompareAgainstRelatedTables(Borrow borrow, string term)
        {
            if (borrow.Book != null) { if (InvarientCultureCompare(borrow.Book.Title, term)) return true; }
            if (borrow.Account != null) { if (InvarientCultureCompare(borrow.Account.Name, term)) return true; }

            return false;
        }

        private bool CompareAgainstRelatedTables(OmniData data, string term)
        {
            if (data.text != null) { if (InvarientCultureCompare(data.text, term)) return true; }
            if (data.description!= null) { if (InvarientCultureCompare(data.description, term)) return true; }

            return false;
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

        /// <summary>
        /// Tries to match the text and suggests the closest result from an array of strings 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="textBox"></param>
        /// <param name="stackPanel"></param>
        public void HookSearchBar(string[] data, TextBox textBox, StackPanel stackPanel)
        {

            string query = textBox.Text;

            stackPanel.Visibility = System.Windows.Visibility.Visible;

            // Clear the list   
            stackPanel.Children.Clear();


            bool hasResults = false;

            List<string> result = data.Where(c => c.ToLower().StartsWith(query.ToLower())).OrderBy(c => c).ToList();

            // Add the result   
            foreach (string obj in data)
            {
                if (obj.ToLower().Contains(query.ToLower()))
                {

                    // The word starts with this... Autocomplete must work   
                    result.Add(obj);
                    hasResults = true;
                }
            }
            result = result.Distinct().ToList();

            foreach (string obj in result)
            {
                this.AddItem(obj, textBox, stackPanel);
            }
        }




        private void AddItem(string text, TextBox textBox, StackPanel stackPanel)
        {
            TextBlock block = new TextBlock();

            // Add the text   
            block.Text = text;

            // A little style...   
            block.Margin = new Thickness(2, 3, 2, 3);
            block.Cursor = Cursors.Hand;

            // Mouse events   
            block.MouseLeftButtonDown += (sender, e) =>
            {
                textBox.Text = (sender as TextBlock).Text;
            };

            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.LightSkyBlue;
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };

            textBox.LostFocus += (sender, e) =>
            {
                stackPanel.Visibility = System.Windows.Visibility.Collapsed;
            };

            // Add to the panel   
            stackPanel.Children.Add(block);
        }
    }
}
