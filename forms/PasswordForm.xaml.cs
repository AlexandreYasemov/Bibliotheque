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
    /// Interaction logic for PasswordForm.xaml
    /// </summary>
    public partial class PasswordForm : Page
    {
        private PhpfribiblioContext context;
        private Account currentAccount;
        public PasswordForm()
        {
            InitializeComponent();

            currentAccount = Globals.currentUser;
            context = Globals.databaseContext;
        }

        private void Click_ModifyPassword(object sender, RoutedEventArgs e)
        {
            if (Security.AuthantificatePassword(currentAccount, tb_oldPassword.Password))
            {
                if  (tb_newPassword.Password.Length < 5)
                {
                    tb_error.Text = "Le mot de passe doit être\nau moins 5 caractères de long";
                    tb_error.Visibility = Visibility.Visible;
                    return;
                }

                if (tb_newPassword.Password == tb_repeatNewPassword.Password)
                {
                    Account user = context.Accounts.Find(currentAccount.AccountId);

                    Security crypto = new Security();
                    byte[] salt = crypto.GenerateSalt();
                    byte[] hashedPassword = crypto.HashPassword(tb_newPassword.Password, salt);

                    user.Password = hashedPassword;
                    user.Salt = salt;

                    context.Update(user);
                    context.SaveChanges();

                    Globals.mainWindow.CloseForm();
                }
                else
                {
                    tb_error.Text = "Le mot de passe répété doit\nêtre identique au nouveau";
                    tb_error.Visibility = Visibility.Visible;

                }
            }
            else
            {
                tb_error.Text = "Mot de passe incorrect";
                tb_error.Visibility = Visibility.Visible;
            }
        }
    }
}
