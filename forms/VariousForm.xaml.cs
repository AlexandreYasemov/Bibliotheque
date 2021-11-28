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
    /// Interaction logic for VariousForm.xaml
    /// </summary>
    /// 
    public enum BibliotequeVariousForm
    {
        Author = 0,
        Publisher = 1,
        Type = 2,
    }

    public partial class VariousForm : Page
    {

        BibliotequeVariousForm currentForm;
        Author currentAuthor;
        Publisher currentPublisher;
        DataAccess.DataObjects.Type currentType;
        private MainWindow mainWindow;
        private PhpfribiblioContext context;


        public VariousForm(BibliotequeVariousForm form)
        {
            InitializeComponent();
            SetupForm(form);

            mainWindow = (MainWindow)Application.Current.MainWindow;
            context = Globals.databaseContext;
        }

        public VariousForm(Author author)
        {
            InitializeComponent();
            currentForm = BibliotequeVariousForm.Author;
            tb_header.Text = "Modify author";
            currentAuthor = author;

            FillFields(author);

            mainWindow = (MainWindow)Application.Current.MainWindow;
            context = Globals.databaseContext;
        }

        public VariousForm(Publisher publisher)
        {
            InitializeComponent();
            currentForm = BibliotequeVariousForm.Publisher;
            tb_header.Text = "Modify publisher";
            currentPublisher = publisher;

            FillFields(publisher);

            mainWindow = (MainWindow)Application.Current.MainWindow;
            context = Globals.databaseContext;
        }

        public VariousForm(DataAccess.DataObjects.Type type)
        {
            InitializeComponent();
            currentForm = BibliotequeVariousForm.Type;
            tb_header.Text = "Modify genre";
            currentType = type;

            FillFields(type);

            mainWindow = (MainWindow)Application.Current.MainWindow;
            context = Globals.databaseContext;
        }


        private void SetupForm(BibliotequeVariousForm form)
        {
            currentForm = form;

            switch (form)
            {
                case BibliotequeVariousForm.Author:
                    tb_header.Text = "Add author";
                    break;
                case BibliotequeVariousForm.Publisher:
                    tb_header.Text = "Add publisher";
                    break;
                case BibliotequeVariousForm.Type:
                    tb_header.Text = "Add genre";
                    break;
            }
        }

        private void Click_AddModifyVarious(object sender, RoutedEventArgs e)
        {
            switch (currentForm)
            {
                case BibliotequeVariousForm.Author:
                    //Add
                    if (currentAuthor == null)
                    {
                        currentAuthor = new Author();
                        context.Add(currentAuthor);
                        FillData();
                    }
                    //Update
                    else
                    {
                        FillData();
                        context.Update(currentAuthor);
                    }
                    break;
                case BibliotequeVariousForm.Publisher:
                    //Add
                    if (currentPublisher == null)
                    {
                        currentPublisher = new Publisher();
                        context.Add(currentPublisher);
                        FillData();
                    }
                    //Update
                    else
                    {
                        FillData();
                        context.Update(currentPublisher);
                    }
                    break;
                case BibliotequeVariousForm.Type:
                    //Add
                    if (currentType == null)
                    {
                        currentType = new DataAccess.DataObjects.Type();
                        context.Add(currentType);
                        FillData();
                    }
                    //Update
                    else
                    {
                        FillData();
                        context.Update(currentType);
                    }
                    break;
            }

            context.SaveChanges();
            mainWindow.CloseForm();
        }

        /// <summary>
        /// Prepare data before sending to the database
        /// </summary>
        private void FillData()
        {
            switch (currentForm)
            {
                case BibliotequeVariousForm.Author:
                    currentAuthor.Name = tb_item.Text;
                    currentAuthor.Description = tb_description.Text;
                    break;
                case BibliotequeVariousForm.Publisher:
                    currentPublisher.Name = tb_item.Text;
                    currentPublisher.Description = tb_description.Text;
                    break;
                case BibliotequeVariousForm.Type:
                    currentType.Name = tb_item.Text;
                    currentType.Description = tb_description.Text;
                    break;
            }
        }


        /// <summary>
        /// Entity data to the form textblocks
        /// </summary>
        private void FillFields(Author author)
        {
            tb_item.Text = author.Name;
            tb_description.Text = author.Description;
        }
        private void FillFields(Publisher publisher)
        {
            tb_item.Text = publisher.Name;
            tb_description.Text = publisher.Description;
        }
        private void FillFields(DataAccess.DataObjects.Type type)
        {
            tb_item.Text = type.Name;
            tb_description.Text = type.Description;
        }
    }
}
