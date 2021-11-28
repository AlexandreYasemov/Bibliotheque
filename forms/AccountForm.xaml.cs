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
    /// Interaction logic for AccountForm.xaml
    /// </summary>
    public partial class AccountForm : Page
    {
        private PhpfribiblioContext context;
        private Account currentAccount;
        public AccountForm()
        {
            InitializeComponent();

            context = Globals.databaseContext;
        }

        public AccountForm(Account modifyAccount)
        {
            InitializeComponent();

            context = Globals.databaseContext;

            currentAccount = modifyAccount;
            FillFields(modifyAccount);

            tb_header.Text = "Modify user";
            tb_password_header.Text = "Change password";
        }

        private void Click_AddModifyAccount(object sender, RoutedEventArgs e)
        {

            if (!AssertData())
                return;

            //Add
            if (currentAccount == null)
            {
                currentAccount = new Account();
                context.Add(currentAccount);
                FillData();
            }
            //Update
            else
            {
                currentAccount = context.Accounts.Where(c => c.AccountId == currentAccount.AccountId).FirstOrDefault();
                FillData();
                context.Update(currentAccount);
            }

            context.SaveChanges();

            Globals.mainWindow.CloseForm();
        }

        private bool AssertData()
        {
            bool isReady = true;

            if (String.IsNullOrEmpty(tb_password.Text) && currentAccount != null)
            {
                tb_password_error.Visibility = Visibility.Hidden;
            }
            else
            if (tb_password.Text.Length > 4)
            {
                tb_password_error.Visibility = Visibility.Hidden;
            }
            else
            {
                tb_password_error.Visibility = Visibility.Visible;
                isReady = false;
            }


            Account account = context.Accounts.Where(c => c.Name == tb_name.Text).FirstOrDefault();
            if (currentAccount == null)
            {
                tb_name_error.Visibility = Visibility.Hidden;
            }
            else if (account.AccountId == currentAccount.AccountId)
            {
                tb_name_error.Visibility = Visibility.Hidden;
            }
            else
            {
                tb_name_error.Visibility = Visibility.Visible;
                isReady = false;
            }

            return isReady;

        }

        private void FillData()
        {

            currentAccount.Name = tb_name.Text;
            currentAccount.IsAdmin = (bool)cb_adminPermisisons.IsChecked;

            if (!String.IsNullOrEmpty(tb_password.Text))
            {
                Security crypto = new Security();
                byte[] salt = crypto.GenerateSalt();
                currentAccount.Salt = salt;
                currentAccount.Password = crypto.HashPassword(tb_password.Text, salt);
            }
        }

        private void FillFields(Account account)
        {
            tb_name.Text = account.Name;
            cb_adminPermisisons.IsChecked = account.IsAdmin;

        }
    }
}
