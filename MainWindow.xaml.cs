using Bibliotheque.DataAccess.DataObjects;
using Bibliotheque.forms;
using Bibliotheque.helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bibliotheque
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            formsFrame.NavigationService.RemoveBackEntry();
            MainFrame.NavigationService.RemoveBackEntry();
                

            Globals.mainWindow = this;

            ShowForm(new LoginForm());
        }

        /// <summary>
        /// Show a form while freezing the main window
        /// </summary>
        /// <param name="page"></param>
        public void ShowForm(Page page)
        {
            formsFrame.Navigate(page);
            windowFreeze.Visibility = Visibility.Visible;
            formsFrame.LoadCompleted += FormInitialized;

            this.Focusable = false;
        }

        /// <summary>
        /// Go to a temporary tab that will disappear when unselected
        /// </summary>
        /// <param name="page"></param>
        /// <param name="image"></param>
        /// <param name="text"></param>
        public void AddTemporaryTab(Page page, BitmapImage image, string text)
        {
            MainFrame.Navigate(page);
            TempTab.Visibility = Visibility.Visible;

            TempTab_Text.Text = text;
            TempTab_Image.Source = image;

            LeftTabMenu.SelectedItem = TempTab;
        }

        private void LeftTabMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selection = (ListBoxItem)LeftTabMenu.SelectedItem;
            if (selection == null)
                return;

            string tab = selection.Name;
            Page page = new Page();
            switch (tab)
            {
                case "admin":
                    page = new AdminPage();
                    break;
                case "books":
                    page = new BookPage();
                    break;
                case "borrows":
                    page = new BorrowPage();
                    break;
                case "settings":
                    page = new SettingsPage();
                    break;
                default:
                    return;
            }

            if (selection != TempTab)
            {
                TempTab.Visibility = Visibility.Collapsed;
            }

            MainFrame.Navigate(page);
        }

        /// <summary>
        /// Find the cancel button and unfreeze if form closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormInitialized(object sender, EventArgs e)
        {
            if (((Frame)sender).Content == null)
                return;
            Page page = (Page)((Frame)sender).Content;
            if (page.FindName("cancel") == null)
                return;

            if (page.FindName("cancel").GetType() != typeof(Button))
                return;
            Button cancel = (Button)page.FindName("cancel");
            if (cancel == null)
                return;

            cancel.Click += ShowForm_Closed;
        }

        private void ShowForm_Closed(object sender, RoutedEventArgs e)
        {
            CloseForm();
        }
        private void ShowLogin_Closed(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void CloseForm()
        {
            ListBoxItem selection = (ListBoxItem)LeftTabMenu.SelectedItem;
            string tab = selection.Name;
            //Update lists?
            switch (tab)
            {

            }

            windowFreeze.Visibility = Visibility.Hidden;
            formsFrame.Content = null;

            this.Focusable = true;

        }

        public void LoggedIn(Account user)
        {
            Globals.currentUser = user;

            Globals.databaseContext = new PhpfribiblioContext();

            Globals.databaseContext.SaveChangesFailed += Logging.dbSave_Fail;
            Globals.databaseContext.SavedChanges += Logging.dbSave_Success;
            Globals.databaseContext.SavingChanges += Logging.dbSave_Saving;

            windowFreeze.Visibility = Visibility.Hidden;
            formsFrame.Content = null;

            LeftTabMenu.SelectedIndex = 1;

            tb_account_name.Text = user.Name;

            border_login.Visibility = Visibility.Visible;

            AuthantificateAdminAccess();
        }

        private void AuthantificateAdminAccess()
        {
            if (Globals.currentUser.IsAdmin == true)
            {
                admin.Visibility = Visibility.Visible;
                admin.IsEnabled = true;
            }
        }

        /// <summary>
        /// Global message system to send feedback to the client
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isError"></param>
        public void ShowMessage(string message, bool isError)
        {
            messageBox.Text = message;

            DoubleAnimation dbCanvasYOpen = new DoubleAnimation();
            dbCanvasYOpen.From = 0;
            dbCanvasYOpen.To = 96;
            dbCanvasYOpen.DecelerationRatio = 0.5;
            dbCanvasYOpen.Duration = new Duration(TimeSpan.FromSeconds(.25));

            DoubleAnimation dbCanvasYClose = new DoubleAnimation();
            dbCanvasYClose.BeginTime = TimeSpan.FromSeconds(5);
            dbCanvasYClose.From = 96;
            dbCanvasYClose.To = 0;
            dbCanvasYClose.Duration = new Duration(TimeSpan.FromSeconds(.25));
            dbCanvasYClose.DecelerationRatio = 0.5;

            Storyboard story = new Storyboard();

            story.Children.Add(dbCanvasYOpen);
            Storyboard.SetTargetName(dbCanvasYOpen, messageBox.Name);
            Storyboard.SetTargetProperty(dbCanvasYOpen, new PropertyPath(MediaElement.HeightProperty));

            story.Children.Add(dbCanvasYClose);
            Storyboard.SetTargetName(dbCanvasYClose, messageBox.Name);
            Storyboard.SetTargetProperty(dbCanvasYClose, new PropertyPath(MediaElement.HeightProperty));

            story.Begin(this);
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            Globals.databaseContext.DisposeAsync();
            Globals.currentUser = null;

            MainFrame.Content = null;

            border_login.Visibility = Visibility.Hidden;

            admin.Visibility = Visibility.Collapsed;
            TempTab.Visibility = Visibility.Collapsed;

            LeftTabMenu.SelectedItem = null;

            ShowForm(new LoginForm());
        }


            
    }
}
