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
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Page
    {

        private Security crypto;
        public LoginForm()
        {

            InitializeComponent();

            crypto = new Security();
        }

        private void Click_Login(object sender, RoutedEventArgs e)
        {
            Account user = Security.AuthantificateUser(tb_username.Text, tb_password.Password);
            if (user != null)
            {
                Globals.mainWindow.LoggedIn(user);
            }
            else
            {
                tb_error.Visibility = Visibility.Visible;
            }

        }

        private void Click_Quit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
