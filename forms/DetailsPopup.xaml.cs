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
    /// Interaction logic for DetailsPopup.xaml
    /// </summary>
    public partial class DetailsPopup : Page
    {
        bool isAdmin;

        BibliotequeVariousForm bibliotequeVariousForm;
        Author currentAuthor;
        Publisher currentPublisher;

        public DetailsPopup(Author author)
        {
            InitializeComponent();
            bibliotequeVariousForm = BibliotequeVariousForm.Author;
            tb_header.Text = "Détails auteur";
            currentAuthor = author;

            tb_name.Text = author.Name;
            tb_description.Text = author.Description;
            image_icon.Source = new BitmapImage(new Uri("pack://application:,,,/icons/author.png"));

            isAdmin = Globals.currentUser.IsAdmin;

            if (isAdmin)
                modify.Visibility = Visibility.Visible;
        }

        public DetailsPopup(Publisher publisher)
        {
            InitializeComponent();
            bibliotequeVariousForm = BibliotequeVariousForm.Publisher;
            tb_header.Text = "Détails éditeur";
            currentPublisher = publisher;

            tb_name.Text = publisher.Name;
            tb_description.Text = publisher.Description;
            image_icon.Source = new BitmapImage(new Uri("pack://application:,,,/icons/publisher.png"));

            isAdmin = Globals.currentUser.IsAdmin;

            if (isAdmin)
                modify.Visibility = Visibility.Visible;
        }

        private void Modify_Click(object sender, RoutedEventArgs e)
        {
            switch (bibliotequeVariousForm)
            {
                case BibliotequeVariousForm.Author:
                    Globals.mainWindow.ShowForm(new VariousForm(currentAuthor));
                    break;
                case BibliotequeVariousForm.Publisher:
                    Globals.mainWindow.ShowForm(new VariousForm(currentPublisher));
                    break;

            }
        }
    }
}
